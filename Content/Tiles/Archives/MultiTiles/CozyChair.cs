using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class CozyChair : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        protected virtual Color MapColor => new Color(208, 61, 125);

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 2);

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);

            TileObjectData.addTile(Type);

            AddMapEntry(MapColor);
        }
    }

    public class CozyChairRed : CozyChair
    {
        protected override Color MapColor => new Color(166, 46, 56);
    }

    public class CozyChairGreen : CozyChair
    {
        protected override Color MapColor => new Color(41, 156, 153);
    }

    public class CozyChairBlue : CozyChair
    {
        protected override Color MapColor => new Color(84, 98, 157);
    }
}