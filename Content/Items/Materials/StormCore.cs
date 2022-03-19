using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class StormCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Core");
            Tooltip.SetDefault("'Resonates with the unbridled strength of the storm'");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 22;
            item.rare = ItemRarityID.Orange;
            item.maxStack = 99;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
        }
    }
}