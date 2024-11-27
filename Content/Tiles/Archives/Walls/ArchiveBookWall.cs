using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveBookWall : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(97, 66, 17));
        }
    }
}