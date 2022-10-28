using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class Inkwell : ModTile
    {
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 16 };

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 1);

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newAlternate.Width = 1;
            TileObjectData.newAlternate.Height = 2;
            TileObjectData.newAlternate.CoordinateHeights = new int[2] { 16, 16 };

            TileObjectData.newAlternate.UsesCustomCanPlace = true;
            TileObjectData.newAlternate.Origin = new Point16(0, 1);

            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(new Color(78, 67, 62));
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 34, 14, ModContent.ItemType<Content.Items.Placeable.Furniture.Inkwell>());
        }
    }
}