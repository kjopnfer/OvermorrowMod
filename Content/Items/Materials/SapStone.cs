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
            item.width = 18;
            item.height = 32;
            item.rare = ItemRarityID.Blue;
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