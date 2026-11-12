using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Loot;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
{
    /// <summary>
    /// Terminal cell for dead-end aux branches. Drops a BigChest, optional
    /// decorations. Connects horizontally only.
    /// </summary>
    public class ChestRoom : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.Treasure;

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
        /// Chest rooms open horizontally only. Left/Right faces without a
        /// neighbor get stone-capped by ApplySideCaps. Top/Bottom are walls.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

        // OwnsPadding intentionally not overridden (defaults to false) so
        // ApplySideCaps stone-caps any open side without a neighbor.

        private const string AsepritePath = AssetDirectory.GrandArchives + "ChestRoom.aseprite";

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(55, 148, 110)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                var chestEntity = TileUtils.PlaceTileWithEntity<BigChest, BigChest_TE>(x, y);
                if (chestEntity != null)
                {
                    chestEntity.ContentsModifier = new RarityModifier(rare: 10);
                    chestEntity.ContentsKinds = ItemKind.Weapon | ItemKind.Accessory | ItemKind.Armor;
                    chestEntity.ContentsFavored = ItemKind.Weapon | ItemKind.Armor;
                }
            }),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(BuildContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            TexGen.PaintClearLayer(AsepritePath, ctx.Origin.X, ctx.Origin.Y, hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Walls, hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, ctx.Origin.X, ctx.Origin.Y, ctx.Palette.Tiles, hp, vp, cw, ch);
        }

        public override void BuildPadding(PaddingContext ctx)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

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
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, BuildObjectMap());
        }
    }
}
