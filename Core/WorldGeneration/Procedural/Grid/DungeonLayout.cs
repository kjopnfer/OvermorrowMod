using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using System.Threading.Tasks;
using Terraria.DataStructures;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Where a linked dungeon sits relative to this one.
    /// </summary>
    public enum LayoutDirection
    {
        East,
        West,
        North,
        South,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest
    }

    public static class LayoutDirections
    {
        /// <summary>
        /// Coarse grid step for a direction, each axis in {-1, 0, 1}.
        /// </summary>
        public static Point Delta(this LayoutDirection dir) => dir switch
        {
            LayoutDirection.East => new Point(1, 0),
            LayoutDirection.West => new Point(-1, 0),
            LayoutDirection.North => new Point(0, -1),
            LayoutDirection.South => new Point(0, 1),
            LayoutDirection.NorthEast => new Point(1, -1),
            LayoutDirection.NorthWest => new Point(-1, -1),
            LayoutDirection.SouthEast => new Point(1, 1),
            LayoutDirection.SouthWest => new Point(-1, 1),
            _ => Point.Zero,
        };

        /// <summary>
        /// The direction whose <see cref="Delta"/> is <paramref name="delta"/>. Each axis must be in {-1, 0, 1}, not both 0.
        /// </summary>
        public static LayoutDirection FromDelta(Point delta) => Enum.GetValues<LayoutDirection>().First(dir => dir.Delta() == delta);

        public static LayoutDirection Opposite(this LayoutDirection dir) => dir switch
        {
            LayoutDirection.East => LayoutDirection.West,
            LayoutDirection.West => LayoutDirection.East,
            LayoutDirection.North => LayoutDirection.South,
            LayoutDirection.South => LayoutDirection.North,
            LayoutDirection.NorthEast => LayoutDirection.SouthWest,
            LayoutDirection.NorthWest => LayoutDirection.SouthEast,
            LayoutDirection.SouthEast => LayoutDirection.NorthWest,
            LayoutDirection.SouthWest => LayoutDirection.NorthEast,
            _ => dir,
        };

        /// <summary>
        /// True if a dungeon with doors toward <paramref name="dirs"/> has its spine march top to bottom: it has a North
        /// and a South door.
        /// </summary>
        public static bool MarchesDown(IEnumerable<LayoutDirection> dirs) => dirs.Contains(LayoutDirection.North) && dirs.Contains(LayoutDirection.South);

        /// <summary>
        /// Which spine end a door in this direction uses: -1 for the start (west, or north when the spine
        /// <paramref name="marchesDown"/>), 1 for the end (east, or south), 0 for a fork.
        /// </summary>
        public static int SpineEnd(this LayoutDirection dir, bool marchesDown)
        {
            Point d = dir.Delta();
            if (marchesDown) return d.X == 0 ? d.Y : 0;
            return d.Y == 0 ? d.X : 0;
        }
    }

    /// <summary>
    /// Declares a set of dungeons and the directional connections between them,
    /// then generates, positions, and links them. Placement is derived from the
    /// graph: a dungeon linked east of another is built east of it, so a door
    /// always leads toward the dungeon drawn in that direction.
    /// </summary>
    public class DungeonLayout
    {
        private sealed class Node
        {
            public int Id;
            public DungeonContent Content;
            public Point CoarseCoord;
            public bool Placed;
            public Point WorldOrigin;
            public Point SpawnTile;
            public int Zone;
            public readonly Dictionary<LayoutDirection, IDungeonDoor> Doors = new();
        }

        private readonly struct Edge
        {
            public readonly int From;
            public readonly LayoutDirection Dir;
            public readonly int To;

            public Edge(int from, LayoutDirection dir, int to)
            {
                From = from;
                Dir = dir;
                To = to;
            }
        }

        private readonly List<Node> _nodes = new();
        private readonly List<Edge> _edges = new();
        private readonly List<(int Node, Func<GridRoom> Factory)> _extraRooms = new();
        private readonly List<(int Node, LayoutDirection Dir, Func<GridRoom> Factory)> _exits = new();
        private int _rootId = -1;

        /// <summary>Adds a dungeon and returns its handle.</summary>
        public int Add(DungeonContent content)
        {
            int id = _nodes.Count;
            _nodes.Add(new Node { Id = id, Content = content });
            return id;
        }

        /// <summary>
        /// Connects two dungeons given  <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        public void Connect(int from, LayoutDirection dir, int to) => _edges.Add(new Edge(from, dir, to));

        /// <summary>
        /// Adds a feature room the generator must place in <paramref name="node"/>'s dungeon, such as
        /// a subworld portal door.
        /// </summary>
        public void AddRoom(int node, Func<GridRoom> factory) => _extraRooms.Add((node, factory));

        /// <summary>
        /// Places a door to the next biome in <paramref name="node"/>'s dungeon toward <paramref name="dir"/>: a spine end when
        /// <see cref="LayoutDirections.SpineEnd"/> gives one, a fork otherwise.
        /// </summary>
        public void AddExit(int node, LayoutDirection dir, Func<GridRoom> factory) => _exits.Add((node, dir, factory));

        /// <summary>
        /// Sets the wall zone of <paramref name="node"/>'s dungeon (0 by default). Rooms near a door into a dungeon of
        /// another zone take that zone.
        /// </summary>
        public void SetZone(int node, int zone) => _nodes[node].Zone = zone;

        /// <summary>Sets the dungeon the player spawns in.</summary>
        public void SetRoot(int node) => _rootId = node;

        /// <summary>
        /// Plans every dungeon without placing tiles. Each dungeon plans from its own seed drawn from
        /// <paramref name="rand"/>, so the result is the same in parallel and sequential mode.
        /// </summary>
        public DungeonPlan[] Plan(Random rand, PlanOptions options)
        {
            var seeds = new int[_nodes.Count];
            for (int i = 0; i < seeds.Length; i++)
                seeds[i] = rand.Next();

            var plans = new DungeonPlan[_nodes.Count];
            void PlanDungeon(int i) => plans[i] = GridGenerator.Plan(_nodes[i].Content, new Random(seeds[i]), DoorDirectionsFor(i), ExtraRoomsFor(i), i, options, ExitsFor(i), _nodes[i].Zone, DoorZonesFor(i));

            if (options.Sequential)
            {
                for (int i = 0; i < _nodes.Count; i++)
                    PlanDungeon(i);
            }
            else
            {
                Parallel.For(0, _nodes.Count, PlanDungeon);
            }
            return plans;
        }

        /// <summary>
        /// Generates every dungeon, positions them from the graph, pairs the doors, and sets the spawn.
        /// </summary>
        public void Build(Point worldCenter, Random rand)
        {
            if (_nodes.Count == 0) return;
            if (_rootId < 0) _rootId = 0;
            WarnOnSpineEndConflicts();
            WarnOnBackwardEdges();

            // 1. Plan each tile placement without placing tiles.
            var plans = Plan(rand, new PlanOptions { Diagnostics = true });

            // 2. Position from the graph using the measured footprints.
            AssignCoarseCoords();
            AssignWorldOrigins(worldCenter, plans);

            // 3. Render each at its origin and resolve its doors.
            for (int i = 0; i < _nodes.Count; i++)
            {
                GridGenerator.Render(plans[i], _nodes[i].WorldOrigin, rand, out Point spawnTile, out var placements);
                _nodes[i].SpawnTile = spawnTile;

                foreach (var kv in placements)
                {
                    var te = FindNearestDoor(kv.Value.DoorTile);
                    if (te != null) _nodes[i].Doors[kv.Key] = te;
                }
            }

            var treasureDoors = BuildTreasureRooms(plans, rand);

            // 4. Assign unique door ids, then pair each edge reciprocally.
            int nextDoorId = 9001;
            foreach (var node in _nodes)
            {
                foreach (var te in node.Doors.Values)
                {
                    te.DoorID = nextDoorId++;
                    te.PairedDoor = -1;
                }
            }

            foreach (var edge in _edges)
            {
                var fromDoors = _nodes[edge.From].Doors;
                var toDoors = _nodes[edge.To].Doors;
                if (fromDoors.TryGetValue(edge.Dir, out var a) && toDoors.TryGetValue(edge.Dir.Opposite(), out var b))
                {
                    a.PairedDoor = b.DoorID;
                    b.PairedDoor = a.DoorID;
                }
                else
                {
                    Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: connection {edge.From} -{edge.Dir}-> {edge.To} has no door pair; left unpaired.");
                }
            }

            foreach (var (entrance, exit) in treasureDoors)
            {
                entrance.DoorID = nextDoorId++;
                entrance.PairedDoor = -1;
                if (exit == null) continue;
                exit.DoorID = nextDoorId++;
                exit.PairedDoor = entrance.DoorID;
                entrance.PairedDoor = exit.DoorID;
            }

            var root = _nodes[_rootId];
            Main.spawnTileX = root.SpawnTile.X;
            Main.spawnTileY = root.SpawnTile.Y;
        }

        /// <summary>Walks the graph from the root, assigning each dungeon a coarse grid cell by following edge directions.</summary>
        private void AssignCoarseCoords()
        {
            foreach (var n in _nodes) n.Placed = false;

            var root = _nodes[_rootId];
            root.CoarseCoord = Point.Zero;
            root.Placed = true;
            var occupied = new Dictionary<Point, int> { [Point.Zero] = root.Id };
            var queue = new Queue<int>();
            queue.Enqueue(root.Id);

            while (queue.Count > 0)
            {
                int cur = queue.Dequeue();
                foreach (var edge in _edges)
                {
                    int other;
                    LayoutDirection step;
                    if (edge.From == cur) { other = edge.To; step = edge.Dir; }
                    else if (edge.To == cur) { other = edge.From; step = edge.Dir.Opposite(); }
                    else continue;

                    if (_nodes[other].Placed) continue;  // chord edge: don't move an already-placed dungeon

                    Point coord = _nodes[cur].CoarseCoord + step.Delta();
                    if (occupied.TryGetValue(coord, out int taken) && taken != other)
                        Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: layout collision at cell ({coord.X},{coord.Y}); dungeons {taken} and {other} overlap. Fix the connection directions.");

                    _nodes[other].CoarseCoord = coord;
                    _nodes[other].Placed = true;
                    occupied[coord] = other;
                    queue.Enqueue(other);
                }
            }

            foreach (var n in _nodes)
                if (!n.Placed)
                    Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: dungeon {n.Id} is not connected to the root; placed at the origin.");
        }

        private const int DungeonGapX = 2 * DungeonGrid.HorizontalSpacing;
        private const int DungeonGapY = 2 * DungeonGrid.VerticalSpacing;

        /// <summary>
        /// Tiles kept clear of the world's edges when placing treasure rooms.
        /// </summary>
        private const int WorldEdgeMargin = 50;

        /// <summary>
        /// Tiles kept between a treasure room and its dungeon's rooms and void strips.
        /// </summary>
        private const int TreasureRoomClearance = 8;

        /// <summary>
        /// Tiles kept between two treasure rooms.
        /// </summary>
        private const int TreasureRoomSpacing = 60;

        /// <summary>
        /// How far past its dungeon's footprint a treasure room may extend.
        /// </summary>
        private const int TreasureRoomReach = 120;

        private const int TreasureRoomSearchStep = 4;

        /// <summary>
        /// Paints one treasure room per treasure door, at the free spot closest to the door within reach of the door's
        /// dungeon: clear of that dungeon's rooms and void strips, every other dungeon, and the other treasure rooms.
        /// </summary>
        /// <returns>
        /// Each treasure door with its room's exit door, or with null when the room has no exit door or found no spot.
        /// </returns>
        private List<(IDungeonDoor Entrance, IDungeonDoor Exit)> BuildTreasureRooms(DungeonPlan[] plans, Random rand)
        {
            var footprints = new Rectangle[_nodes.Count];
            for (int i = 0; i < _nodes.Count; i++)
                footprints[i] = new Rectangle(_nodes[i].WorldOrigin.X, _nodes[i].WorldOrigin.Y, plans[i].FootprintWidth, plans[i].FootprintHeight);

            var placedRooms = new List<Rectangle>();
            var pairs = new List<(IDungeonDoor Entrance, IDungeonDoor Exit)>();
            for (int i = 0; i < _nodes.Count; i++)
            {
                var grid = plans[i].Grid;
                var content = _nodes[i].Content;
                var blocked = DungeonAreas(grid, footprints[i]);
                for (int col = 0; col < grid.Cols; col++)
                {
                    for (int row = 0; row < grid.Rows; row++)
                    {
                        var slot = grid.GetSlot(col, row);
                        if (slot.IsEmpty || slot.SubCol != 0 || slot.SubRow != 0 || slot.Room.Type != RoomType.TreasureDoor) continue;
                        Point cell = grid.GridToWorld(col, row);
                        Point doorTile = new Point(cell.X + DungeonGrid.CellTileWidth / 2, cell.Y + DungeonGrid.CellTileHeight / 2);
                        var entrance = FindNearestDoor(doorTile);

                        var room = content.CreateTreasureRoom();
                        var (width, height) = room.Size;
                        Point? origin = FindTreasureRoomSpot(i, footprints, blocked, placedRooms, width, height, doorTile);
                        if (origin == null)
                        {
                            Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: no spot for the treasure room of the door at ({doorTile.X},{doorTile.Y}); its door is left unpaired.");
                            pairs.Add((entrance, null));
                            continue;
                        }

                        placedRooms.Add(new Rectangle(origin.Value.X, origin.Value.Y, width, height));
                        var exit = room.Generate(origin.Value, content.Palette, rand);
                        if (exit == null) Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: treasure room at ({origin.Value.X},{origin.Value.Y}) has no exit door; its door is left unpaired.");
                        pairs.Add((entrance, exit));
                    }
                }
            }
            return pairs;
        }

        /// <summary>
        /// The world areas of a rendered dungeon's cells with their padding, and of its void strips over the footprint's height,
        /// each grown by <see cref="TreasureRoomClearance"/>.
        /// </summary>
        private static List<Rectangle> DungeonAreas(DungeonGrid grid, Rectangle footprint)
        {
            var areas = new List<Rectangle>();
            for (int col = 0; col < grid.Cols; col++)
            {
                for (int row = 0; row < grid.Rows; row++)
                {
                    if (grid.GetSlot(col, row).IsEmpty) continue;
                    Point cell = grid.GridToWorld(col, row);
                    var area = new Rectangle(cell.X - DungeonGrid.HorizontalPadding, cell.Y - DungeonGrid.VerticalPadding, DungeonGrid.CellTileWidth + 2 * DungeonGrid.HorizontalPadding, DungeonGrid.CellTileHeight + 2 * DungeonGrid.VerticalPadding);
                    area.Inflate(TreasureRoomClearance, TreasureRoomClearance);
                    areas.Add(area);
                }
            }
            foreach (var strip in grid.VoidStrips)
            {
                int x = grid.GridToWorld(strip.Col, 0).X - DungeonGrid.HorizontalPadding;
                var area = new Rectangle(x, footprint.Y, strip.Width * DungeonGrid.HorizontalSpacing + DungeonGrid.HorizontalPadding, footprint.Height);
                area.Inflate(TreasureRoomClearance, 0);
                areas.Add(area);
            }
            return areas;
        }

        /// <summary>
        /// Searches outward in square rings from a room centered on <paramref name="doorTile"/> and returns the first free
        /// top-left corner, or null if none lies within <see cref="TreasureRoomReach"/> of dungeon <paramref name="node"/>'s footprint.
        /// </summary>
        private static Point? FindTreasureRoomSpot(int node, Rectangle[] footprints, List<Rectangle> blocked, List<Rectangle> placedRooms, int width, int height, Point doorTile)
        {
            Rectangle reach = footprints[node];
            reach.Inflate(TreasureRoomReach, TreasureRoomReach);
            var world = new Rectangle(WorldEdgeMargin, WorldEdgeMargin, Main.maxTilesX - 2 * WorldEdgeMargin, Main.maxTilesY - 2 * WorldEdgeMargin);
            reach = Rectangle.Intersect(reach, world);

            int centerX = doorTile.X - width / 2;
            int centerY = doorTile.Y - height / 2;
            int maxRadius = Math.Max(reach.Width, reach.Height);

            bool Fits(int x, int y)
            {
                var area = new Rectangle(x, y, width, height);
                if (!reach.Contains(area)) return false;
                for (int i = 0; i < footprints.Length; i++)
                    if (i != node && footprints[i].Intersects(area)) return false;
                foreach (var b in blocked)
                    if (b.Intersects(area)) return false;
                var spaced = area;
                spaced.Inflate(TreasureRoomSpacing, TreasureRoomSpacing);
                foreach (var placed in placedRooms)
                    if (placed.Intersects(spaced)) return false;
                return true;
            }

            if (Fits(centerX, centerY)) return new Point(centerX, centerY);
            for (int r = TreasureRoomSearchStep; r <= maxRadius; r += TreasureRoomSearchStep)
            {
                for (int t = -r; t < r; t += TreasureRoomSearchStep)
                {
                    if (Fits(centerX + t, centerY - r)) return new Point(centerX + t, centerY - r);
                    if (Fits(centerX + r, centerY + t)) return new Point(centerX + r, centerY + t);
                    if (Fits(centerX - t, centerY + r)) return new Point(centerX - t, centerY + r);
                    if (Fits(centerX - r, centerY - t)) return new Point(centerX - r, centerY - t);
                }
            }
            return null;
        }

        /// <summary>Converts coarse cells to centered world origins, spaced by the largest measured footprint plus a gap.</summary>
        private void AssignWorldOrigins(Point worldCenter, DungeonPlan[] plans)
        {
            int minX = int.MaxValue, maxX = int.MinValue, minY = int.MaxValue, maxY = int.MinValue;
            int maxWidth = 0, maxHeight = 0;
            foreach (var n in _nodes)
            {
                if (n.CoarseCoord.X < minX) minX = n.CoarseCoord.X;
                if (n.CoarseCoord.X > maxX) maxX = n.CoarseCoord.X;
                if (n.CoarseCoord.Y < minY) minY = n.CoarseCoord.Y;
                if (n.CoarseCoord.Y > maxY) maxY = n.CoarseCoord.Y;

                int w = plans[n.Id].FootprintWidth;
                int h = plans[n.Id].FootprintHeight;
                if (w > maxWidth) maxWidth = w;
                if (h > maxHeight) maxHeight = h;
            }

            int pitchX = maxWidth + DungeonGapX;
            int pitchY = maxHeight + DungeonGapY;
            float bboxCenterX = (minX + maxX) / 2f;
            float bboxCenterY = (minY + maxY) / 2f;

            foreach (var n in _nodes)
            {
                float cellCenterX = worldCenter.X + (n.CoarseCoord.X - bboxCenterX) * pitchX;
                float cellCenterY = worldCenter.Y + (n.CoarseCoord.Y - bboxCenterY) * pitchY;
                n.WorldOrigin = new Point(
                    (int)(cellCenterX - plans[n.Id].FootprintWidth / 2f),
                    (int)(cellCenterY - plans[n.Id].FootprintHeight / 2f));
            }
        }

        /// <summary>The door directions this dungeon needs: one per incident edge, from this dungeon's perspective.</summary>
        private List<LayoutDirection> DoorDirectionsFor(int nodeId)
        {
            var dirs = new List<LayoutDirection>();
            foreach (var edge in _edges)
            {
                if (edge.From == nodeId) dirs.Add(edge.Dir);
                else if (edge.To == nodeId) dirs.Add(edge.Dir.Opposite());
            }
            return dirs;
        }

        private Dictionary<LayoutDirection, int> DoorZonesFor(int nodeId)
        {
            var zones = new Dictionary<LayoutDirection, int>();
            foreach (var edge in _edges)
            {
                if (edge.From == nodeId) zones[edge.Dir] = _nodes[edge.To].Zone;
                else if (edge.To == nodeId) zones[edge.Dir.Opposite()] = _nodes[edge.From].Zone;
            }
            return zones;
        }

        private void WarnOnSpineEndConflicts()
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                var dirs = DoorDirectionsFor(i).Concat(ExitsFor(i).Select(e => e.Dir)).ToList();
                bool marchesDown = LayoutDirections.MarchesDown(dirs);
                var ends = dirs.Select(dir => dir.SpineEnd(marchesDown)).ToList();
                if (ends.Count(e => e < 0) > 1 || ends.Count(e => e > 0) > 1)
                    Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: dungeon {i} has more than one door at the same spine end; the extras become forks or spine rooms.");
            }
        }

        /// <summary>The extra feature rooms assigned to a dungeon.</summary>
        private List<Func<GridRoom>> ExtraRoomsFor(int nodeId)
        {
            var rooms = new List<Func<GridRoom>>();
            foreach (var (node, factory) in _extraRooms)
                if (node == nodeId) rooms.Add(factory);
            return rooms;
        }

        /// <summary>
        /// Warns about edges that break "an exit's direction always leads toward it": an edge on the way from the root to an
        /// exit's dungeon that moves against the exit's direction, or an edge leading on out of that dungeon in the exit's direction.
        /// </summary>
        private void WarnOnBackwardEdges()
        {
            var parentEdge = ParentEdgesFromRoot();
            foreach (var (node, exitDir, _) in _exits)
            {
                Point forward = exitDir.Delta();
                for (int cur = node; parentEdge[cur] >= 0;)
                {
                    var edge = _edges[parentEdge[cur]];
                    bool towardCur = edge.To == cur;
                    Point step = towardCur ? edge.Dir.Delta() : edge.Dir.Opposite().Delta();
                    if (step.X * forward.X < 0 || step.Y * forward.Y < 0)
                        Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: connection {edge.From} -{edge.Dir}-> {edge.To} leads from the root against the {exitDir} exit of dungeon {node}.");
                    cur = towardCur ? edge.From : edge.To;
                }

                foreach (var edge in _edges)
                {
                    if (edge.From != node && edge.To != node) continue;
                    Point outward = edge.From == node ? edge.Dir.Delta() : edge.Dir.Opposite().Delta();
                    if (outward.X * forward.X > 0 || outward.Y * forward.Y > 0)
                        Terraria.ModLoader.Logging.PublicLogger.Warn($"OvermorrowDungeon: connection {edge.From} -{edge.Dir}-> {edge.To} leads on out of dungeon {node} toward its {exitDir} exit.");
                }
            }
        }

        /// <summary>
        /// For each dungeon, the index of the edge it was first reached by in a breadth-first walk from the root, ignoring edge
        /// direction; -1 for the root and for dungeons not connected to it.
        /// </summary>
        private int[] ParentEdgesFromRoot()
        {
            var parentEdge = new int[_nodes.Count];
            Array.Fill(parentEdge, -1);
            var reached = new bool[_nodes.Count];
            reached[_rootId] = true;
            var queue = new Queue<int>();
            queue.Enqueue(_rootId);
            while (queue.Count > 0)
            {
                int cur = queue.Dequeue();
                for (int e = 0; e < _edges.Count; e++)
                {
                    var edge = _edges[e];
                    int other = edge.From == cur ? edge.To : edge.To == cur ? edge.From : -1;
                    if (other < 0 || reached[other]) continue;
                    reached[other] = true;
                    parentEdge[other] = e;
                    queue.Enqueue(other);
                }
            }
            return parentEdge;
        }

        private List<(LayoutDirection Dir, Func<GridRoom> Factory)> ExitsFor(int nodeId) => _exits.Where(e => e.Node == nodeId).Select(e => (e.Dir, e.Factory)).ToList();

        private static IDungeonDoor FindNearestDoor(Point tile)
        {
            IDungeonDoor best = null;
            int bestDistSq = int.MaxValue;
            foreach (var te in TileEntity.ByID.Values)
            {
                if (te is not IDungeonDoor door) continue;
                int dx = te.Position.X - tile.X;
                int dy = te.Position.Y - tile.Y;
                int distSq = dx * dx + dy * dy;
                if (distSq < bestDistSq)
                {
                    bestDistSq = distSq;
                    best = door;
                }
            }
            return best;
        }
    }
}
