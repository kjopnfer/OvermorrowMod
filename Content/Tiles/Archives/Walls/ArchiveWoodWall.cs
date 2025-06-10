using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveWoodWall : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(83, 59, 44));
        }
    }

    public class ArchiveWoodWallBlack : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(32, 43, 46));
        }
    }

    public class ArchiveWoodWallRed : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(107, 50, 45));
        }
    }

    public class ArchiveWoodWallGreen : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(67, 84, 50));
        }
    }

    public class ArchiveWoodWallBlue : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(70, 67, 117));
        }
    }

    public class ArchiveWoodWallYellow : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(121, 80, 22));
        }
    }
}