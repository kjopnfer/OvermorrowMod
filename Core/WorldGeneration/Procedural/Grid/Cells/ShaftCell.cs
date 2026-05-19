using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class ShaftCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly GridRoom[] VerticalNeighbors =
        {
            new ShaftCell(),
            new BookshelfCell(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Top or Direction.Bottom => VerticalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Top || side == Direction.Bottom;

        public override bool AllowsEmptyNeighbors => false;

        private const string AsepritePath = AssetDirectory.GrandArchives + "ShaftCell.aseprite";

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildTileMap() => new()
        {
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(237, 152, 93)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxSconce>()),
            [(74, 15, 56)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar>()),
        };

        public override void BuildPadding(PaddingContext ctx)
        {
            switch (ctx.Side)
            {
                case Direction.Left:
                case Direction.Right:
                    {
                        int worldX = ctx.X;
                        int worldY = ctx.Y - DungeonGrid.VerticalPadding;
                        int srcX = ctx.Side == Direction.Left ? 0 : DungeonGrid.HorizontalPadding + DungeonGrid.CellTileWidth;
                        int srcY = 0;
                        int srcW = DungeonGrid.HorizontalPadding;
                        int srcH = 2 * DungeonGrid.VerticalPadding + DungeonGrid.CellTileHeight;

                        TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
                        TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, BuildWallMap(), srcX, srcY, srcW, srcH);
                        TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, BuildTileMap(), srcX, srcY, srcW, srcH);
                        break;
                    }
                case Direction.Top:
                case Direction.Bottom:
                    // PlaceShaftFloorPadding leaves the diagonal-stair gap; FillWoodFloor would seal it.
                    PaddingBuilder.PlaceShaftFloorPadding(ctx.X, ctx.Y, ctx.Width, ctx.Height, ctx.FillTileType);
                    break;
            }
        }

        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, System.Func<int, int, GridRoom> pendingLookup = null)
        {
            var above = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y - 1);
            var below = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y + 1);
            if (above != null && above is not (BookshelfCell or ShaftCell))
                return false;
            if (below != null && below is not (BookshelfCell or ShaftCell))
                return false;
            return true;
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int sx = DungeonGrid.HorizontalPadding;
            int sy = DungeonGrid.VerticalPadding;
            int sw = DungeonGrid.CellTileWidth;
            int sh = DungeonGrid.CellTileHeight;

            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y, sx, sy, sw, sh);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, BuildWallMap(), sx, sy, sw, sh);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, BuildTileMap(), sx, sy, sw, sh);
        }
    }
}
