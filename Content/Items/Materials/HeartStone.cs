using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Materials
{
    public class HeartStone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of Stone");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 16));
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 80;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 550;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 999;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
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