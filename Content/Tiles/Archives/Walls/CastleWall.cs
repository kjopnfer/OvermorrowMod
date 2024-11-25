using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class CastleWall : ModWall
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(72, 74, 77));
        }
    }
}