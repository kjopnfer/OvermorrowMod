using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
{
    /// <summary>
    /// One void strip column of a bridge crossing. The sheet spans two columns, so the left cell of each
    /// pair (even offset in its strip) paints the whole sheet and the right cell paints nothing.
    /// </summary>
    public class BridgeCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.Bridge;

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new BridgeCell(),
        };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side) => side == Direction.Left || side == Direction.Right;

        public override bool OwnsPadding => true;

        private const string AsepritePath = AssetDirectory.GrandArchives + "BridgeCell.aseprite";

        private const int SheetWidth = 2 * DungeonGrid.CellTileWidth + 3 * DungeonGrid.HorizontalPadding;

        private static readonly (byte R, byte G, byte B) BridgeMarker = (246, 178, 185);

        /// <summary>
        /// <see cref="ArchiveBridge"/> footprint: 17 wide, 10 tall, origin on the deck row 4 rows below its top.
        /// </summary>
        private const int BridgeWidth = 17;
        private const int BridgeRowsAboveDeck = 4;
        private const int BridgeRowsBelowDeck = 5;

        private static bool IsPairAnchor(DungeonGrid grid, int col) => grid.VoidStripOffset(col) % 2 == 0;

        public override void Build(BuildContext ctx)
        {
            if (!IsPairAnchor(ctx.Grid, ctx.Col)) return;

            int sx = DungeonGrid.HorizontalPadding;
            int sy = DungeonGrid.VerticalPadding;
            int sw = SheetWidth - 2 * DungeonGrid.HorizontalPadding;
            int sh = DungeonGrid.CellTileHeight;

            PaintRegion(ctx.Palette, ctx.Origin.X, ctx.Origin.Y, sx, sy, sw, sh);
        }

        /// <summary>
        /// The top strip is skipped under another bridge: that bridge's deck strip is the same rows, and its cleared gap holds the arch.
        /// </summary>
        public override void BuildPadding(PaddingContext ctx)
        {
            if (!IsPairAnchor(ctx.Grid, ctx.Col)) return;
            if (ctx.Side == Direction.Top && ctx.Grid.GetSlot(ctx.Col, ctx.Row - 1)?.Room is BridgeCell) return;

            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int ch = DungeonGrid.CellTileHeight;
            Point origin = ctx.Grid.GridToWorld(ctx.Col, ctx.Row);

            switch (ctx.Side)
            {
                case Direction.Top:
                    PaintRegion(ctx.Palette, origin.X - hp, origin.Y - vp, 0, 0, SheetWidth, vp);
                    break;
                case Direction.Bottom:
                    PaintRegion(ctx.Palette, origin.X - hp, origin.Y + ch, 0, vp + ch, SheetWidth, vp);
                    break;
                case Direction.Left:
                    PaintRegion(ctx.Palette, origin.X - hp, origin.Y, 0, vp, hp, ch);
                    break;
                case Direction.Right:
                    PaintRegion(ctx.Palette, origin.X + SheetWidth - 2 * hp, origin.Y, SheetWidth - hp, vp, hp, ch);
                    break;
            }
        }

        private static void PaintRegion(DungeonPalette palette, int worldX, int worldY, int srcX, int srcY, int srcW, int srcH)
        {
            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, palette.Walls, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, palette.Tiles, srcX, srcY, srcW, srcH);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            if (!IsPairAnchor(ctx.Grid, ctx.Col)) return;

            var objects = new Dictionary<(byte R, byte G, byte B), TexPlaceFunction>(ctx.Palette.Objects)
            {
                [BridgeMarker] = PlaceBridge,
            };
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, ctx.Origin.X - DungeonGrid.HorizontalPadding, ctx.Origin.Y - DungeonGrid.VerticalPadding, objects);
        }

        /// <summary>
        /// The arch hangs into the cell below, which is stone at the bottom of the dungeon, so the footprint is cleared first.
        /// </summary>
        private static void PlaceBridge(int x, int y)
        {
            for (int lx = 0; lx < BridgeWidth; lx++)
                for (int ly = -BridgeRowsAboveDeck; ly <= BridgeRowsBelowDeck; ly++)
                    WorldGenUtils.ClearTile(x + lx, y + ly);

            WorldGen.PlaceObject(x, y, ModContent.TileType<ArchiveBridge>());
        }
    }
}
