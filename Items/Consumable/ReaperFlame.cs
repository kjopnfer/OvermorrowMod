using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Consumable
{
    public class ReaperFlame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaped Soul");
            ItemID.Sets.ItemNoGravity[item.type] = true;
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 9));
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 30;
            item.maxStack = 999;
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override bool ItemSpace(Player player) => true;

 
        public override void PostUpdate()
        {
            Lighting.AddLight(item.Center, new Color(187, 127, 128).ToVector3() * 0.55f * Main.essScale);
        }
    }
}