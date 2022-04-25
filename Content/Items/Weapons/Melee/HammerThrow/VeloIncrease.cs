using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.HammerThrow
{
    public class VeloIncrease : ModProjectile
    {
        private int timer = 0;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone");
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.timeLeft = 50;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {

            var player = Main.player[Projectile.owner];


            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
            if (timer == 1)
            {
                Vector2 position = Main.player[Projectile.owner].Center;
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                player.velocity = direction * 17;
            }
            player.fullRotation += 1f;
            player.direction = 1;

            player.itemRotation = 360;


            Projectile.position.X = Main.player[Projectile.owner].Center.X - Projectile.width / 2;
            Projectile.position.Y = Main.player[Projectile.owner].Center.Y - Projectile.height / 2;

        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[Projectile.owner].velocity = -Main.player[Projectile.owner].velocity;
        }


        public override void Kill(int timeLeft)
        {
            Main.player[Projectile.owner].fullRotation = 0f;


            if (Main.MouseWorld.X < Main.player[Projectile.owner].Center.X)
            {
                Main.player[Projectile.owner].direction = -1;
            }
        }
    }
}
