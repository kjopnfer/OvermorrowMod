using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class CorridorCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.HorizontalConnector;

        /// <summary>
        /// Corridor padding paints before any neighbor's (bookshelf, stair,
        /// combat) so the neighbor's strip content overwrites the corridor's
        /// on any shared edge.
        /// </summary>
        public override int PaddingPriority => -1;

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new WritingRoom(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        /// <summary>
        /// Corridors are open on the horizontal sides only. Top and bottom
        /// are walls (the corridor's ceiling and floor).
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

        public override bool AllowsEmptyNeighbors => false;

        /// <summary>
        /// A corridor cannot sit directly above or below a shaft, since
        /// shafts only accept bookshelves on their vertical ends.
        /// </summary>
        public override bool IsValidPlacement(DungeonGrid grid, Point anchor, Func<int, int, GridRoom> pendingLookup = null)
        {
            var above = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y - 1);
            var below = GetEffectiveRoomAt(grid, pendingLookup, anchor.X, anchor.Y + 1);
            if (above is ShaftCell) return false;
            if (below is ShaftCell) return false;
            return true;
        }

        private const string AsepritePath = AssetDirectory.GrandArchives + "CorridorCell.aseprite";

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildInteriorObjectMap() => new()
        {
            [(171, 73, 94)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArchSmallHallway>()),
        };

        // Build / BuildPadding / PlaceFurniture

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

        public override void BuildPadding(PaddingContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            // Skip the padding generation for non-Corridor connetions
            if (ctx.Side == Direction.Left || ctx.Side == Direction.Right)
            {
                int neighborCol = ctx.Side == Direction.Left ? ctx.Col - 1 : ctx.Col + 1;
                var neighbor = ctx.Grid?.GetSlot(neighborCol, ctx.Row);
                if (neighbor == null || neighbor.IsEmpty || neighbor.Room is not CorridorCell)
                    return;
            }

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
                    srcH = ch;
                    break;
                case Direction.Right:
                    worldX = ctx.X;
                    worldY = ctx.Y;
                    srcX = hp + cw;
                    srcY = vp;
                    srcW = hp;
                    srcH = ch;
                    break;
                case Direction.Top:
                    // Extend horizontally to claim the two corner squares.
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = 0;
                    srcW = 2 * hp + cw;
                    srcH = vp;
                    break;
                case Direction.Bottom:
                    worldX = ctx.X - hp;
                    worldY = ctx.Y;
                    srcX = 0;
                    srcY = vp + ch;
                    srcW = 2 * hp + cw;
                    srcH = vp;
                    break;
                default:
                    return;
            }

            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, ctx.Palette.Walls, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, ctx.Palette.Tiles, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, worldX, worldY, ctx.Palette.Objects, srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Objects, hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, ctx.Origin.X, ctx.Origin.Y, BuildInteriorObjectMap(), hp, vp, cw, ch);
        }
    }
}
