using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Consumable
{
    public class ReaperFlame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaped Soul");
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 9));
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 30;
            Item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool ItemSpace(Player player) => true;


        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, new Color(187, 127, 128).ToVector3() * 0.55f * Main.essScale);
        }
    }
}