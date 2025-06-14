using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public abstract class ArchiveTable : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public abstract int Height { get; }
        public virtual bool SolidTop => true;
        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = true;
            Main.tileSolidTop[Type] = SolidTop;

            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = Height;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, Height - 1);

            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
            AddMapEntry(new Color(78, 67, 62));
        }
    }

    public class ArchiveTable1 : ArchiveTable
    {
        public override int Height => 2;
    }

    public class ArchiveTable2 : ArchiveTable
    {
        public override int Height => 3;
        public override bool SolidTop => false;
    }

    public class ArchiveTable3 : ArchiveTable
    {
        public override int Height => 4;
        public override bool SolidTop => false;
    }

    public class ArchiveTable4 : ArchiveTable
    {
        public override int Height => 3;
        public override bool SolidTop => false;
    }

    public class ArchiveTable5 : ArchiveTable
    {
        public override int Height => 4;
        public override bool SolidTop => false;
    }

    public class ArchiveTable6 : ArchiveTable
    {
        public override int Height => 3;
        public override bool SolidTop => false;
    }

    public class ArchiveTableBlue : ArchiveTable
    {
        public override int Height => 2;
    }

    public class ArchiveTableGreen : ArchiveTable
    {
        public override int Height => 2;
    }

    public class ArchiveTablePink : ArchiveTable
    {
        public override int Height => 2;
    }

    public class ArchiveTableRed : ArchiveTable
    {
        public override int Height => 2;
    }
}