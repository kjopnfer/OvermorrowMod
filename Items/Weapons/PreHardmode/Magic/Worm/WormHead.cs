using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Items.Weapons.PreHardmode.Magic.Worm
{
    public class WormHead : ModProjectile
    {
        private bool didHit = false;
        private int timer = 0;
        private int SaveVeloX = 0;
        private int SaveVeloY = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corruptor");
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ranged = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 400;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;

        }
        public override void AI()
        {
            if(!didHit)
            {
            Player player = Main.player[projectile.owner];
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 400f;
            bool target = false;
            for (int k = 0; k < 200; k++)
            {
                if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5)
                {
                    Vector2 newMove = Main.npc[k].Center - projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = newMove;
                        distance = distanceTo;
                        target = true;
                    }
                }
            }
            if (target)
            {
                AdjustMagnitude(ref move);
                projectile.velocity += (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }
            }
            if(projectile.velocity.X > 11)
            {
                projectile.velocity.X = 11;
            }
            if(projectile.velocity.X < -11)
            {
                projectile.velocity.X = -11;
            }

            if(projectile.velocity.Y > 11)
            {
                projectile.velocity.Y = 11;
            }
            if(projectile.velocity.Y < -11)
            {
                projectile.velocity.Y = -11;
            }

            projectile.velocity.Y = projectile.velocity.Y + 0.06f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            timer++;
            if(timer == 3)
            {
            Vector2 value1 = new Vector2(0f, 0f);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X / 7, projectile.velocity.Y / 7, mod.ProjectileType("WormBody"), projectile.damage / 2, 1f, projectile.owner, 0f);
            timer = 0;
            }
        }


        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            didHit = true;
            projectile.tileCollide = false;
        }
    }
}