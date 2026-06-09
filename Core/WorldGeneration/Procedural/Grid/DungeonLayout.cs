using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
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

        /// <summary>Sets the dungeon the player spawns in.</summary>
        public void SetRoot(int node) => _rootId = node;

        /// <summary>Generates every dungeon, positions them from the graph, pairs the doors, and sets the spawn.</summary>
        public void Build(Point worldCenter, Random rand)
        {
            if (_nodes.Count == 0) return;
            if (_rootId < 0) _rootId = 0;

            // 1. Plan each tile placement without placing tiles.
            var plans = new DungeonPlan[_nodes.Count];
            for (int i = 0; i < _nodes.Count; i++)
                plans[i] = GridGenerator.Plan(_nodes[i].Content, rand, DoorDirectionsFor(i), ExtraRoomsFor(i));

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

            const int Gap = 240;
            int pitchX = maxWidth + Gap;
            int pitchY = maxHeight + Gap;
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

        /// <summary>The extra feature rooms assigned to a dungeon.</summary>
        private List<Func<GridRoom>> ExtraRoomsFor(int nodeId)
        {
            var rooms = new List<Func<GridRoom>>();
            foreach (var (node, factory) in _extraRooms)
                if (node == nodeId) rooms.Add(factory);
            return rooms;
        }

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
