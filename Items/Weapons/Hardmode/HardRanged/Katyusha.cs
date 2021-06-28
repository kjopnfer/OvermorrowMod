using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardRanged
{


    public class Katyusha : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Fires 5 Rockets Upwards");
        }


        public override void SetDefaults()
        {
            item.damage = 30;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.rare = ItemRarityID.Orange;
            item.UseSound = SoundID.Item62;
            item.autoReuse = true;
            item.ranged = true;
            item.scale = 0.55f;
            item.shoot = ModContent.ProjectileType<katyusharocket>();
            item.shootSpeed = 5f;
            item.crit = 15;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 4;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(6.45f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, mod.ProjectileType("katyusharocket"), item.damage, 3, player.whoAmI);
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
