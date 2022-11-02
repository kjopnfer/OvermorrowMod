using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.TilePiles
{
    internal class Blurb : ModItem
    {
        public override void SetDefaults()
        {
            Item.height = 16; Item.width = 16;
            Item.useStyle = ItemUseStyleID.Swing; Item.useTime = 15; Item.useAnimation = 15;
            Item.createTile = Mod.Find<ModTile>("TilePiles").Type;
        }
    }
}
