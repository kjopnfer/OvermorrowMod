using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class CrystalMana : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystallized Mana Fragment");
            Tooltip.SetDefault("'Excess mana from cast spells tend to seep into the ground, crystallizing over time'");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 24;
            item.rare = ItemRarityID.Green;
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