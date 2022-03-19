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
            item.width = 18;
            item.height = 22;
            item.rare = ItemRarityID.Blue;
            item.maxStack = 99;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, 0f, 0f, 0.5f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}