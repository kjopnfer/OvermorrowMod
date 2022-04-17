using OvermorrowMod.Content.Buffs.Summon;
using OvermorrowMod.Content.Projectiles.Accessory;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Accessories
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
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Summon) *= 0.05f;
            player.AddBuff(ModContent.BuffType<DripplerBuff>(), 2);
            if ((int)player.ownedProjectileCounts[ModContent.ProjectileType<DripplerFriendly>()] >= 1)
            {
                return;
            }
            Projectile.NewProjectile(player.GetProjectileSource_Accessory(Item), player.position.X, player.position.Y, 0f, 0f, ModContent.ProjectileType<DripplerFriendly>(), 20, 1.25f, player.whoAmI, 0f, 0f);
        }
    }
}