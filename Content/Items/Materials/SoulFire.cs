using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class SoulFire : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Fire");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 99;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, 0f, 0f, 0.5f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}