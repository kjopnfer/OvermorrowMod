using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Loot;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    /// <summary>
    /// Terminal cell for dead-end aux branches. Drops a BigChest, optional
    /// decorations. Connects horizontally only.
    /// </summary>
    public class ChestRoom : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

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

        // Color maps

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildWallMap() => new()
        {
            [(32, 43, 46)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWallBlack>()),
            [(101, 66, 14)] = TexPlaceAction.PlaceWall(ModContent.WallType<ArchiveWoodWall>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildTileMap() => new()
        {
            [(74, 47, 33)] = TexPlaceAction.PlaceTile(ModContent.TileType<ArchiveWood>()),
            [(105, 106, 106)] = TexPlaceAction.PlaceTile(ModContent.TileType<CastleBrick>()),
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(148, 109, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandleholder>()),
            [(135, 28, 66)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArch>()),
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
            [(199, 158, 59)] = TexPlaceAction.PlaceObject(ModContent.TileType<ArchiveBanner>()),
            [(55, 148, 110)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                var chestEntity = TileUtils.PlaceTileWithEntity<BigChest, BigChest_TE>(x, y);
                if (chestEntity != null)
                {
                    chestEntity.ContentsModifier = new RarityModifier(rare: 10);
                    chestEntity.ContentsKinds = ItemKind.Weapon | ItemKind.Accessory | ItemKind.Armor;
                }
            }),
        };

        // Build / BuildPadding / PlaceFurniture

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y, hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, BuildWallMap(), hp, vp, cw, ch);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, BuildTileMap(), hp, vp, cw, ch);
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
