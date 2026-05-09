using Microsoft.Xna.Framework;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    /// <summary>
    /// Mandatory-encounter room. Same 2x1 footprint and bookshelf-sandwich
    /// connection rules as <see cref="LoungeRoom"/>, currently rendered as a
    /// fully cleared, wall-less space so it visually pops in the dungeon
    /// during placement testing. Decoration and enemy spawns will be added
    /// once placement is dialled in.
    /// </summary>
    public class CombatRoom : GridRoom
    {
        public override int CellWidth => 3;
        public override int CellHeight => 1;

        // Two combat rooms must sit at least this many cells apart
        // (Chebyshev distance). Prevents combat-corridor-combat clusters
        // near the spine end where A* would otherwise pack them tightly.
        private const int MinSpacingBetweenCombatRooms = 3;
        // Up to 3 per dungeon: one mandatory on the spine and at most
        // two more spread across branches. MinSpacingBetweenCombatRooms
        // keeps them from clustering.
        private const int MaxInstancesPerDungeon = 3;
        // Reject combat placements within this Chebyshev distance of any
        // door. Without this, A* segments / branches cheerfully drop a
        // CombatRoom 3-4 columns from a door because CombatRoom's cost
        // weight (0.7) makes it the cheapest non-bookshelf option.
        private const int MinDistanceFromDoor = 5;

        // Painted last so combat's corridor-style entry passages win the
        // shared strip against neighboring bookshelves' wood-panel padding.
        public override int PaddingPriority => 10;

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = FootprintWidth;
            int height = FootprintHeight;

            // Clear footprint to a void box.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    WorldGenUtils.ClearWall(origin.X + x, origin.Y + y);
                }
            }

            // Place a CombatDoor_TE 2 tiles outside each side, on the floor
            // row, and link them as siblings so the pair opens/closes together.
            int floorRow = origin.Y + height - 1;
            int leftTile = origin.X - 2;
            int rightTile = origin.X + width + 1;

            int leftId  = ModContent.GetInstance<CombatDoor_TE>().Place(leftTile,  floorRow);
            int rightId = ModContent.GetInstance<CombatDoor_TE>().Place(rightTile, floorRow);

            if (TileEntity.ByID.TryGetValue(leftId,  out var leftTE)  && leftTE  is Common.RoomManager.CombatDoor_TE leftDoor)
                leftDoor.SiblingTEID  = rightId;

            if (TileEntity.ByID.TryGetValue(rightId, out var rightTE) && rightTE is Common.RoomManager.CombatDoor_TE rightDoor)
                rightDoor.SiblingTEID = leftId;
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            // Sides get corridor-style entry passages (or wood-panel on
            // dead-end faces); top/bottom get a wood floor.
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();

            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                    {
                        // Wipe and refill the shared strip plus the vertical
                        // padding above it, overriding whatever the neighbor's
                        // BuildPadding wrote.
                        ushort fill = (ushort)ctx.FillTileType;
                        int trim = DungeonGrid.VerticalPadding;
                        for (int lx = 0; lx < ctx.Width; lx++)
                        {
                            for (int ly = 0; ly < ctx.Height; ly++)
                            {
                                WorldGenUtils.ClearWall(ctx.X + lx, ctx.Y + ly);
                                WorldGenUtils.PlaceTile(ctx.X + lx, ctx.Y + ly, fill);
                            }
                            for (int ly = 0; ly < trim; ly++)
                            {
                                WorldGenUtils.ClearWall(ctx.X + lx, ctx.Y - trim + ly);
                                WorldGenUtils.PlaceTile(ctx.X + lx, ctx.Y - trim + ly, fill);
                            }
                        }

                        int neighborCol = ctx.Side == Direction.Left
                            ? ctx.Col - 1
                            : ctx.Col + CellWidth;
                        var neighborSlot = ctx.Grid?.GetSlot(neighborCol, ctx.Row);
                        bool hasNeighbor = neighborSlot != null && !neighborSlot.IsEmpty;

                        // skipAbove kills the wood ceiling above the opening
                        // so it reads as stone, matching the surrounding fill.
                        if (hasNeighbor)
                        {
                            PaddingBuilder.PlaceCorridorPadding(
                                ctx.X, ctx.Y, ctx.Width, ctx.Height,
                                woodWall, blueWall, skipAbove: true);
                        }
                        else
                        {
                            PaddingBuilder.PlaceWoodPanelPadding(
                                ctx.X, ctx.Y, ctx.Width, ctx.Height,
                                woodWall, blueWall, skipAbove: true);
                        }
                        break;
                    }
                case Direction.Top:
                case Direction.Bottom:
                    PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
                    break;
            }
        }

        // Combat rooms accept the standard architectural neighbors so the
        // spine and branches have flexibility in routing through them.
        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return false;
            if (subCol == 0 && side == Direction.Left) return true;
            if (subCol == 2 && side == Direction.Right) return true;
            return false;
        }

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && (side == Direction.Left || side == Direction.Right)) return true;
            if (subCol == 2 && side == Direction.Left) return true;
            return false;
        }

        /// <summary>
        /// Enforce minimum spacing between combat rooms and a per-dungeon
        /// instance cap. Bounded proximity scan around the candidate
        /// anchor (radius = MinSpacingBetweenCombatRooms) covers spacing;
        /// a cheap grid-only pass counts total combat instances for the cap.
        /// Considers both committed grid cells and the in-progress A* path
        /// via <paramref name="pendingLookup"/>.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor,
                                              Func<int, int, GridRoom> pendingLookup = null)
        {
            // Door-proximity check.
            for (int dc = 0; dc < grid.Cols; dc++)
            {
                for (int dr = 0; dr < grid.Rows; dr++)
                {
                    var dSlot = grid.GetSlot(dc, dr);
                    if (dSlot == null || dSlot.IsEmpty) continue;
                    if (dSlot.Room is not DoorRoom) continue;

                    for (int sc = 0; sc < CellWidth; sc++)
                    {
                        for (int sr = 0; sr < CellHeight; sr++)
                        {
                            int dist = Math.Max(
                                Math.Abs((anchor.X + sc) - dc),
                                Math.Abs((anchor.Y + sr) - dr));
                            if (dist < MinDistanceFromDoor) return false;
                        }
                    }
                }
            }

            int radius = MinSpacingBetweenCombatRooms;
            int cMin = Math.Max(0, anchor.X - radius);
            int cMax = Math.Min(grid.Cols - 1, anchor.X + radius);
            int rMin = Math.Max(0, anchor.Y - radius);
            int rMax = Math.Min(grid.Rows - 1, anchor.Y + radius);

            for (int c = cMin; c <= cMax; c++)
            {
                for (int r = rMin; r <= rMax; r++)
                {
                    var room = GetEffectiveRoomAt(grid, pendingLookup, c, r);
                    if (room is CombatRoom)
                    {
                        int dist = Math.Max(Math.Abs(c - anchor.X), Math.Abs(r - anchor.Y));
                        if (dist < MinSpacingBetweenCombatRooms) return false;
                    }
                }
            }

            // Total combat-room count cap. Cheap grid-only scan; pending
            // path placements aren't counted, but the spacing constraint
            // already prevents clustering within a single path.
            var seen = new HashSet<GridRoom>();
            int combatCount = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.Room is CombatRoom && seen.Add(slot.Room)) combatCount++;
                }
            }
            return combatCount < MaxInstancesPerDungeon;
        }
    }
}
