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
            Item.width = 18;
            Item.height = 24;
            Item.rare = ItemRarityID.Green;
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