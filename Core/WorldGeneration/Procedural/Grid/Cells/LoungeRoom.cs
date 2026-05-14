using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class LoungeRoom : GridRoom
    {
        public override int CellWidth => 2;
        public override int CellHeight => 1;

        // Caps and spacing rules used by IsValidPlacement.
        private const int MaxInstancesPerDungeon = 2;
        private const int MinSpacingBetweenLounges = 5;
        private const int MinDistanceFromDoor = 2;

        // Lounges must always sit between bookshelves, so they accept
        // bookshelves only on both horizontal sides.
        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
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
            if (subCol == 1 && side == Direction.Right) return true;
            return false;
        }

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            if (subCol == 0 && side == Direction.Right) return true;
            if (subCol == 1 && side == Direction.Left) return true;
            return false;
        }

        /// <summary>
        /// Cap total lounges, enforce minimum spacing between them, and keep
        /// them away from doors. Counts unique room instances across the
        /// committed grid AND in-progress placements via pendingLookup.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, Func<int, int, GridRoom> pendingLookup = null)
        {
            // Bounded proximity scan covers door distance and lounge spacing.
            int radius = Math.Max(MinSpacingBetweenLounges, MinDistanceFromDoor);
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
                    if (room is LoungeRoom && dist < MinSpacingBetweenLounges) return false;
                    if (room is DoorRoom && dist < MinDistanceFromDoor) return false;
                }
            }

            // Lounge count cap: cheap grid-only pass (no pendingLookup walk).
            var seen = new HashSet<GridRoom>();
            int loungeCount = 0;
            for (int c = 0; c < grid.Cols; c++)
            {
                for (int r = 0; r < grid.Rows; r++)
                {
                    var slot = grid.GetSlot(c, r);
                    if (slot == null || slot.IsEmpty) continue;
                    if (slot.Room is LoungeRoom && seen.Add(slot.Room)) loungeCount++;
                }
            }
            return loungeCount < MaxInstancesPerDungeon;
        }

        private const string AsepritePath = AssetDirectory.GrandArchives + "LoungeRoom.aseprite";

        // Color maps

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(66, 64, 61)] = TexPlaceAction.PlaceWall(ModContent.WallType<CastleWall>()),
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildTileMap() => new()
        {
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
            [(134, 42, 104)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>()),
            [(131, 42, 134)] = TexPlaceAction.PlaceObject(ModContent.TileType<SmallChair>(), direction: 1), // Faces right
            [(69, 40, 60)] = TexPlaceAction.PlaceObject(ModContent.TileType<BanquetTable>()),
            [(159, 131, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandelabra>()),
            [(75, 105, 47)] = TexPlaceAction.PlaceObject(ModContent.TileType<BookPileTable>()),
            [(159, 183, 204)] = TexPlaceAction.PlaceObject(ModContent.TileType<Bismarck>()),
            [(99, 49, 110)] = TexPlaceAction.PlaceObject(ModContent.TileType<FireplacePillar>()),
            [(180, 58, 0)] = TexPlaceAction.PlaceObject(ModContent.TileType<Fireplace>()),
            [(208, 61, 125)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>()),
            [(237, 86, 227)] = TexPlaceAction.PlaceObject(ModContent.TileType<CozyChair>(), direction: 1), // Faces right
            [(171, 73, 94)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArchSmall>()),
            [(199, 158, 59)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchiveBanner>()),
            [(153, 229, 80)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxChandelier>()),

            // Random painting from the width-4 / width-8 pools.
            [(19, 215, 73)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width4;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),

            [(101, 224, 135)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                int[] pool = PaintingPool.Width8;
                WorldGen.PlaceObject(x, y, pool[Main.rand.Next(pool.Length)]);
            }),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int interiorW = CellWidth * DungeonGrid.CellTileWidth + (CellWidth - 1) * hp;
            int interiorH = CellHeight * DungeonGrid.CellTileHeight + (CellHeight - 1) * vp;

            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y, hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, BuildWallMap(), hp, vp, interiorW, interiorH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, BuildTileMap(), hp, vp, interiorW, interiorH);
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
                    // Extend horizontally to claim the two corner squares.
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
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, BuildWallMap(), srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, BuildTileMap(), srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());
        }
    }
}
