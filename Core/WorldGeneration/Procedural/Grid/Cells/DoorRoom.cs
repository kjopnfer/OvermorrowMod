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
    /// <summary>
    /// The entrance/exit cell that holds a door linking this dungeon to
    /// another area (a second procedural section, a hand-crafted room, etc.).
    /// Both sides are horizontal connections: one side sits against the
    /// grid boundary (the portal side), the other faces the dungeon.
    /// </summary>
    public class DoorRoom : GridRoom
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

        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

        public override bool OwnsPadding => true;

        private const string AsepritePath = AssetDirectory.GrandArchives + "DoorRoom.aseprite";

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
            [(0, 0, 0)] = TexPlaceAction.Clear,
        };

        private static Dictionary<(byte, byte, byte), TexPlaceFunction> BuildObjectMap() => new()
        {
            [(148, 109, 65)] = TexPlaceAction.PlaceObject(ModContent.TileType<WaxCandleholder>()),
            [(135, 28, 66)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenArch>()),
            [(179, 36, 136)] = TexPlaceAction.PlaceObject(ModContent.TileType<WoodenPillar2>()),
            [(32, 30, 27)] = TexPlaceAction.CustomPlaceObject((x, y) =>
            {
                TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(x, y);
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
