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
            Item.width = 38;
            Item.height = 22;
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = 99;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.White.ToVector3() * 0.55f * Main.essScale);
        }
    }
}