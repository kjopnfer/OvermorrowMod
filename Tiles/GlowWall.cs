using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Tiles
{
    public class GlowWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(23, 71, 71));
        }
    }
}