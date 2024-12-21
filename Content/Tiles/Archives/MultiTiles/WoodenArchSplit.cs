using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class WoodenArch : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        protected virtual int Width => 14;
        protected virtual int Height => 6;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = false;

            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 0);

            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

        }
    }

    public class WoodenArchSplit : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;
        protected virtual int Width => 8;
        protected virtual int Height => 3;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);

            TileObjectData.newTile.Width = Width;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 0);

            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

        }
    }

    public class WoodenArchL1 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 6;
    }

    public class WoodenArchL2 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 5;
    }

    public class WoodenArchL3 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 4;
    }

    public class WoodenArchR3 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 6;
    }

    public class WoodenArchR2 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 5;
    }

    public class WoodenArchR1 : WoodenArchSplit
    {
        protected override int Width => 1;
        protected override int Height => 4;
    }
}