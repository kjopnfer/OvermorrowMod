using OvermorrowMod.Content.Buffs.Pets;
using OvermorrowMod.Projectiles.Pets;
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
            DisplayName.SetDefault("Keepsake Chain");
            Tooltip.SetDefault("Summons a miniature Rune Merchant to travel with you");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.ZephyrFish);
            item.shoot = ModContent.ProjectileType<Smolboi>();
            item.buffType = ModContent.BuffType<SmolboiBuff>();
        }

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 3600, true);
            }
        }
    }
}