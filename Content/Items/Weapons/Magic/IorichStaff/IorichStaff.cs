using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.IorichStaff
{
    public class IorichStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iorich's Sorrow");
            Tooltip.SetDefault("Right click on enemies on lock onto them\n" +
                "Holding down shoot while locked onto enemies will create Spirit Daggers around them\n" +
                "Converges towards the center after creating 8 Spirit Daggers\n" +
                "'The veil of darkness did little to hide the inferno that devoured his people'");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.autoReuse = true;
            Item.rare = ItemRarityID.Orange;
            Item.mana = 12;
            Item.UseSound = SoundID.Item8;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 30;
            Item.useTurn = false;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.width = 56;
            Item.height = 60;
            Item.shoot = ModContent.ProjectileType<NatureBolt>();
            Item.shootSpeed = 9f;
            Item.knockBack = 3f;
            Item.DamageType = DamageClass.Magic;
            Item.channel = true;
            Item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NatureSpike>()] < 8)
            {
                return true;
            }

            return false;
        }

        public override bool CanShoot(Player player)
        {
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NatureBolt>()] <= 0)
            {
                Projectile.NewProjectile(player.GetProjectileSource_Item(Item), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<NatureBolt>(), Item.damage, 0f, player.whoAmI);
            }
        }
    }
}