using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.Tiles.Misc;
using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class WritingRoom : GridRoom
    {
        public override int CellWidth => 3;
        public override int CellHeight => 2;

        private const int MaxInstancesPerDungeon = 2;
        private const int MinSpacingBetweenWritingRooms = 5;
        private const int MinDistanceFromDoor = 2;

        public override int PaddingPriority => 10;
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

        /// <summary>
        /// Connects only on the bottom-left and bottom-right subcells.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side)) return false;

            if (subCol == 0 && subRow == 1 && side == Direction.Left) return true;
            if (subCol == 2 && subRow == 1 && side == Direction.Right) return true;

            return false;
        }

        /// <summary>Cursor sits at the bottom row where the ports are.</summary>
        public override Point AnchorOffsetFromCursor => new Point(0, -1);

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            // Horizontal internal edges between columns.
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && (side == Direction.Left || side == Direction.Right)) return true;
            if (subCol == 2 && side == Direction.Left) return true;

            // Vertical internal edges between rows.
            if (subRow == 0 && side == Direction.Bottom) return true;
            if (subRow == 1 && side == Direction.Top) return true;

            return false;
        }

        /// <summary>
        /// Cap total writing rooms, enforce minimum spacing between them,
        /// and keep them away from doors. Counts unique room instances
        /// across the committed grid AND in-progress placements.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, Func<int, int, GridRoom> pendingLookup = null)
        {
            int radius = Math.Max(MinSpacingBetweenWritingRooms, MinDistanceFromDoor);
            int cMin = Math.Max(0, anchor.X - radius);
            int cMax = Math.Min(grid.Cols - 1, anchor.X + radius);
            int rMin = Math.Max(0, anchor.Y - radius);
            int rMax = Math.Min(grid.Rows - 1, anchor.Y + radius);

            for (int c = cMin; c <= cMax; c++)
            {
                for (int r = rMin; r <= rMax; r++)
                {
                    var room = GetEffectiveRoomAt(grid, pendingLookup, c, r);
                    if (room == null) continue;
                    int dist = Math.Max(Math.Abs(c - anchor.X), Math.Abs(r - anchor.Y));
                    if (room is WritingRoom && dist < MinSpacingBetweenWritingRooms) return false;
                    if (room is DoorRoom && dist < MinDistanceFromDoor) return false;
                }
            }

            // Lounge count cap: cheap grid-only pass.
            var seen = new HashSet<GridRoom>();
            int count = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.Room is WritingRoom && seen.Add(slot.Room)) count++;
                }
            }
            return count < MaxInstancesPerDungeon;
        }

        private const string AsepritePath = AssetDirectory.GrandArchives + "WritingRoom.aseprite";

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(134, 42, 104)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>()),
            [(131, 42, 134)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>(), direction: -1),
            [(171, 73, 94)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArchSmall>()),
            [(31, 224, 164)] = TexPlaceAction.CustomPlaceObject((x, y) => PlaceTableAndChair(x, y, facingRight: true)),
            [(98, 224, 31)] = TexPlaceAction.CustomPlaceObject((x, y) => PlaceTableAndChair(x, y, facingRight: false)),
            [(180, 179, 163)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                for (int i = 0; i < 11; i++)
                    WorldGen.PlaceObject(x, y - i * 3, ModContent.TileType<WoodenLadder>(), true);
            }),
        };

        private static void PlaceTableAndChair(int x, int y, bool facingRight)
        {
            int tableType = ModContent.TileType<ArchiveTablePink>();
            if (Main.rand.NextBool())
                tableType = ModContent.TileType<ArchiveTable1>();

            bool specialType = false;
            if (Main.rand.NextBool(5))
            {
                int[] specials = new[]
                {
                    ModContent.TileType<ArchiveTable2>(),
                    ModContent.TileType<ArchiveTable3>(),
                    ModContent.TileType<ArchiveTable4>(),
                    ModContent.TileType<ArchiveTable5>(),
                    ModContent.TileType<ArchiveTable6>(),
                };
                tableType = Main.rand.Next(specials);
                specialType = true;
            }

            int chairType = ModContent.TileType<SmallChair>();

            if (facingRight)
            {
                // Chair on the left facing right, table on the right also facing right.
                WorldGen.PlaceObject(x, y, chairType, true, 0, 0, -1, 1);
                WorldGen.PlaceObject(x + 2, y, tableType, true, 0, 0, 0, 1);
            }
            else
            {
                // Table on the left facing left, chair on the right facing left.
                WorldGen.PlaceObject(x, y, tableType, true, 0, 0, 0, -1);
                WorldGen.PlaceObject(x + 3, y, chairType, true, 0, 0, -1, -1);
            }

            if (!specialType)
            {
                if (Main.rand.NextBool())
                    WorldGen.PlaceObject(x, y - 2, ModContent.TileType<BookPileTable>(), true);
                if (Main.rand.NextBool())
                    WorldGen.PlaceObject(x + 2, y - 2, ModContent.TileType<Inkwell>(), true);
            }
        }

        // Build / BuildPadding / PlaceFurniture

        public override void Build(BuildContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            TexGen.PaintClearLayer(AsepritePath, ctx.Origin.X, ctx.Origin.Y, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Walls, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Tiles, hp, vp, interiorW, interiorH);
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            int worldX;
            int worldY;
            int srcX;
            int srcY;
            int srcW;
            int srcH;
            switch (ctx.Side)
            {
                case Direction.Left:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp;
                    srcW = hp;
                    srcH = interiorH;
                    break;
                case Direction.Right:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = hp + interiorW;
                    srcY = vp;
                    srcW = hp;
                    srcH = interiorH;
                    break;
                case Direction.Top:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = 0;
                    srcW = 2 * hp + interiorW;
                    srcH = vp;
                    break;
                case Direction.Bottom:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp + interiorH;
                    srcW = 2 * hp + interiorW;
                    srcH = vp;
                    break;
                default:
                    return;
            }

            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, ctx.Palette.Walls, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, ctx.Palette.Tiles, srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());
        }

        public override void PlaceSpawns(FurnitureContext ctx, List<SpawnSlot> slots) => HarvestSpawns(ctx, slots, AsepritePath);
    }
}
