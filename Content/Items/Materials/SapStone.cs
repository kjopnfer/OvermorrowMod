using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class SapStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sorceror Sap Stone");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.rare = ItemRarityID.Blue;
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