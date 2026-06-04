using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class ShaftCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.VerticalConnector;

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
                        TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, ctx.Palette.Walls, srcX, srcY, srcW, srcH);
                        TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, ctx.Palette.Tiles, srcX, srcY, srcW, srcH);
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
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
        }

        public override void PlaceSpawns(FurnitureContext ctx, List<SpawnSlot> slots) => HarvestSpawns(ctx, slots, AsepritePath);

        public override void Build(BuildContext ctx)
        {
            int sx = DungeonGrid.HorizontalPadding;
            int sy = DungeonGrid.VerticalPadding;
            int sw = DungeonGrid.CellTileWidth;
            int sh = DungeonGrid.CellTileHeight;

            TexGen.PaintClearLayer(AsepritePath, ctx.Origin.X, ctx.Origin.Y, sx, sy, sw, sh);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Walls, sx, sy, sw, sh);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Tiles, sx, sy, sw, sh);
        }
    }
}
