using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Pets;
using OvermorrowMod.Content.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Pets
{
    public class BeanSummon : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName and Tooltip are automatically set from the .lang files, but below is how it is done normally.
            // DisplayName.SetDefault("Keepsake Chain");
            // Tooltip.SetDefault("Summons a miniature Rune Merchant to travel with you");
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.shoot = ModContent.ProjectileType<Smolboi>();
            Item.buffType = ModContent.BuffType<SmolboiBuff>();
        }

        public override void UseStyle(Player player, Rectangle rect)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600, true);
            }
        }
    }
}