using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class VeloIncrease : ModProjectile
    {
        private int length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bone");
        }

        public override void SetDefaults()
        {
            projectile.width = 90;
            projectile.height = 90;
            projectile.timeLeft = 50;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {

            var player = Main.player[projectile.owner];


            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
            if(timer == 1)
            {
                Vector2 position = Main.player[projectile.owner].Center;
                Vector2 targetPosition = Main.MouseWorld;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                player.velocity = direction * 17;
            }
            player.fullRotation += 1f;
            player.direction = 1;

            player.itemRotation = 360;


            projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - projectile.height / 2;

        }


        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].velocity = -Main.player[projectile.owner].velocity;
        }


        public override void Kill(int timeLeft)
        {
            Main.player[projectile.owner].fullRotation = 0f;


            if(Main.MouseWorld.X < Main.player[projectile.owner].Center.X)
            {
                Main.player[projectile.owner].direction = -1;
            }
        }
    }
}
