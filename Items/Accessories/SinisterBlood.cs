using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Projectiles.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Accessories
{
    public class SinisterBlood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sinister Blood Sample");
            Tooltip.SetDefault("5% increased minion damage\nSummons a Looming Drippler to protect you");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 1, 50, 0);
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.minionDamage += .05f;
            player.AddBuff(ModContent.BuffType<DripplerBuff>(), 2);
            if ((int)player.ownedProjectileCounts[ModContent.ProjectileType<DripplerFriendly>()] >= 1)
            {
                return;
            }
            Projectile.NewProjectile(player.position.X, player.position.Y, 0f, 0f, ModContent.ProjectileType<DripplerFriendly>(), 20, 1.25f, player.whoAmI, 0f, 0f);
        }
    }
}