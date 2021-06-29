using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Ranged
{
    public class ClockworkShotgun : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 10;
            item.ranged = true;
            item.width = 26;
            item.height = 52;
            item.useTime = 10;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2;
            item.UseSound = SoundID.Item31;
            item.autoReuse = false;
            item.shoot = ProjectileID.Bullet;
            item.useAmmo = AmmoID.Bullet;
            item.shootSpeed = 5f;
            item.value = Item.sellPrice(0, 1, 0, 0);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Clockwork Shotgun");
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {

            int numberProjectiles = 10;
            for (int i = 0; i < numberProjectiles; i++)
            {
                int RandomX = Main.rand.Next(-3, 4);
                int RandomY = Main.rand.Next(-2, 3);
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedByRandom(MathHelper.ToRadians(0f));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X + RandomX, perturbedSpeed.Y + RandomY, type, item.damage, 3, player.whoAmI);
            }
            return true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}
