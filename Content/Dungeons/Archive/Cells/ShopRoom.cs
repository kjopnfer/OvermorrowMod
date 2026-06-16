using System;
using System.Collections.Generic;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.TextureMapping;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives.Shop;
using OvermorrowMod.Core.NPCs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

using OvermorrowMod.Core.WorldGeneration.Procedural.Grid;

namespace OvermorrowMod.Content.Dungeons.Archive.Cells
{
    public class ShopRoom : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        public override RoomType Type => RoomType.Filler;

        public static readonly (byte R, byte G, byte B) ShopkeeperMarker = (220, 255, 4);

        private static readonly GridRoom[] HorizontalNeighbors =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
        };

        private static readonly GridRoom[] VerticalNeighbors = { new ShaftCell() };

        protected override GridRoom[] AllowedNeighbors(Direction side) => side switch
        {
            Direction.Left or Direction.Right => HorizontalNeighbors,
            Direction.Top or Direction.Bottom => VerticalNeighbors,
            _ => Array.Empty<GridRoom>(),
        };

        public override bool IsOpenSide(int subCol, int subRow, Direction side) => true;

        public override bool OwnsPadding => true;

        private const string AsepritePath = AssetDirectory.GrandArchives + "ShopRoom.aseprite";

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
            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
            {
                int neighborRow = ctx.Side == Direction.Top ? ctx.Row - 1 : ctx.Row + 1;
                var neighbor = ctx.Grid.GetSlot(ctx.Col, neighborRow);
                if (neighbor != null && !neighbor.IsEmpty && neighbor.Room is ShaftCell)
                    return;
            }

            // Seal a horizontal side with stone when nothing connects there, instead of painting the open doorway.
            if (ctx.Side == Direction.Left || ctx.Side == Direction.Right)
            {
                int neighborCol = ctx.Side == Direction.Left ? ctx.Col - 1 : ctx.Col + 1;
                var sideNeighbor = ctx.Grid.GetSlot(neighborCol, ctx.Row);
                if (sideNeighbor == null || sideNeighbor.IsEmpty)
                {
                    PaddingBuilder.FillSolid(ctx.X, ctx.Y, ctx.Width, ctx.Height, ctx.FillTileType);
                    return;
                }
            }

            int hp = DungeonGrid.HorizontalPadding;
            int vp = DungeonGrid.VerticalPadding;
            int cw = DungeonGrid.CellTileWidth;
            int ch = DungeonGrid.CellTileHeight;

            int worldX, worldY, srcX, srcY, srcW, srcH;
            switch (ctx.Side)
            {
                case Direction.Left:
                    worldX = ctx.X; worldY = ctx.Y; srcX = 0; srcY = vp; srcW = hp; srcH = ch; break;
                case Direction.Right:
                    worldX = ctx.X; worldY = ctx.Y; srcX = hp + cw; srcY = vp; srcW = hp; srcH = ch; break;
                case Direction.Top:
                    worldX = ctx.X - hp; worldY = ctx.Y; srcX = 0; srcY = 0; srcW = 2 * hp + cw; srcH = vp; break;
                case Direction.Bottom:
                    worldX = ctx.X - hp; worldY = ctx.Y; srcX = 0; srcY = vp + ch; srcW = 2 * hp + cw; srcH = vp; break;
                default:
                    return;
            }

            TexGen.PaintClearLayer(AsepritePath, worldX, worldY, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, worldX, worldY, ctx.Palette.Walls, srcX, srcY, srcW, srcH);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, worldX, worldY, ctx.Palette.Tiles, srcX, srcY, srcW, srcH);

            if (ctx.Side == Direction.Top || ctx.Side == Direction.Bottom)
                PaddingBuilder.FillWoodFloor(ctx.X, ctx.Y, ctx.Width, ctx.Height);
        }

        public override void PlaceFurniture(FurnitureContext ctx)
        {
            int paintX = ctx.Origin.X - DungeonGrid.HorizontalPadding;
            int paintY = ctx.Origin.Y - DungeonGrid.VerticalPadding;
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, paintX, paintY, ctx.Palette.Objects);
        }

        public override void PlaceSpawns(FurnitureContext ctx, List<SpawnSlot> slots)
        {
            var harvested = new List<SpawnSlot>();
            HarvestSpawns(ctx, harvested, AsepritePath);

            foreach (var slot in harvested)
            {
                if (slot.Color != ShopkeeperMarker) continue;

                int teId = ModContent.GetInstance<ShopkeeperSpawnPoint>().Place(slot.WorldPos.X, slot.WorldPos.Y);
                if (teId != -1 && TileEntity.ByID.TryGetValue(teId, out var te) && te is ShopkeeperSpawnPoint sp)
                {
                    sp.NPCType = ModContent.NPCType<CartShopkeeper>();
                    sp.Facing = FacingFromOpenSide(ctx);
                }
            }
        }

        /// <summary>
        /// Faces the shopkeeper toward the room's connected side: -1 if the left neighbor is a room, otherwise 1.
        /// </summary>
        private static int FacingFromOpenSide(FurnitureContext ctx)
        {
            var left = ctx.Grid.GetSlot(ctx.Col - 1, ctx.Row);
            if (left != null && !left.IsEmpty) return -1;
            return 1;
        }
    }
}
