using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.WaterCave
{
    public class EmptyWall : ModWall
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
        }
    }
}