using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.Upgrades
{
    public class HomingStinger : ModProjectile
    {


        public override string Texture => "Terraria/Projectile_" + ProjectileID.Stinger;
        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 240;
            projectile.ignoreWater = true;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(6) == 3)
            {
                target.AddBuff(20, 600);
            }
        }

        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                if(Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    projectile.velocity.X = 10;
                    projectile.velocity.Y = 0;
                }
                else
                {
                    projectile.velocity.X = -10;
                    projectile.velocity.Y = 0;
                }
            }
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.CorruptionThorns, projectile.velocity.X, projectile.velocity.Y, 1, new Color(), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.25f;
            }



            Player player = Main.player[projectile.owner];
            projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
            if (projectile.localAI[0] == 0f)
            {
                AdjustMagnitude(ref projectile.velocity);
                projectile.localAI[0] = 1f;
            }
            Vector2 move = Vector2.Zero;
            float distance = 180f;
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
                projectile.velocity = (10 * projectile.velocity + move) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }
        }
        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 10f)
            {
                vector *= 10f / magnitude;
            }
        }
    }
}