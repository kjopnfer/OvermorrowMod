using Microsoft.Xna.Framework;
using OvermorrowMod.Projectiles.Magic;
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
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.autoReuse = true;
            item.rare = ItemRarityID.Orange;
            item.mana = 12;
            item.UseSound = SoundID.Item8;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.damage = 30;
            item.useTurn = false;
            item.useAnimation = 32;
            item.useTime = 32;
            item.width = 56;
            item.height = 60;
            item.shoot = ModContent.ProjectileType<NatureBolt>();
            item.shootSpeed = 9f;
            item.knockBack = 3f;
            item.magic = true;
            item.channel = true;
            item.value = Item.sellPrice(gold: 1, silver: 75);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NatureSpike>()] < 8)
            {
                return true;
            }

            return false;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<NatureBolt>()] <= 0)
            {
                Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<NatureBolt>(), item.damage, 0f, player.whoAmI);
            }
        }
    }
}