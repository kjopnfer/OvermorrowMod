using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class Spores : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spore");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.timeLeft = 540;
            projectile.penetrate = 3;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }



        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(20, 60);
        }

        public override void AI()
        {
            projectile.rotation += 0.3f;

            projectile.ai[0]++;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 39, projectile.velocity.X, projectile.velocity.Y, 75, new Color(), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }

            if (projectile.velocity.X != 0)
            {
                if (projectile.velocity.X > 0)
                {
                    projectile.velocity.X -= 0.01f;
                }
                else
                {
                    projectile.velocity.X += 0.01f;
                }
            }

            if (projectile.velocity.Y != 0)
            {
                if (projectile.velocity.Y > 0)
                {
                    projectile.velocity.Y -= 0.01f;
                }
                else
                {
                    projectile.velocity.Y += 0.01f;
                }
            }
        }

        public override bool CanDamage()
        {
            if (projectile.ai[0] < 120)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Item10, projectile.position);
                projectile.velocity.X = -projectile.velocity.X;
                projectile.velocity.Y = -projectile.velocity.Y;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, 39, dustvelocity.X, dustvelocity.Y, 0, default, 1f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}