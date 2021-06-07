using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Accessory
{
    public class SandBallFriendly : ModProjectile
    {
        private bool runOnce = true;
        private float rotateSpeed;
        private int delayAttack = 60;
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 900;
            projectile.alpha = 255;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (runOnce)
            {
                rotateSpeed = projectile.ai[1];
                runOnce = false;
            }
            
            //Making player variable "p" set as the projectile's owner
            Player player = Main.player[projectile.owner];

            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.timeLeft = 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().sandMode == 1) // Attack Mode
            {
                if(projectile.localAI[0] == 0f)
                {
                    AdjustMagnitude(ref projectile.velocity);
                    projectile.localAI[0] = 1f;
                }

                Vector2 move = Vector2.Zero;
                float distance = 400f;
                bool target = false;
                if (delayAttack > 0)
                {
                    delayAttack--;
                }
                else
                {
                    for (int k = 0; k < 200; k++)
                    {
                        if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5 && Main.npc[k].CanBeChasedBy())
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
                }

                if (target)
                {
                    AdjustMagnitude(ref move);
                    projectile.velocity = (10 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
                else
                {
                    projectile.velocity = Vector2.Zero;
                    //Factors for calculations
                    double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                    double rad = deg * (Math.PI / 180); //Convert degrees to radians
                    double dist = projectile.ai[0]; //Distance away from the player

                    /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
                    /distance for the desired distance away from the player minus the projectile's width   /
                    /and height divided by two so the center of the projectile is at the right place.     */
                    projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
                    projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

                    //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                    projectile.ai[1] += (1f * rotateSpeed);
                }
            }
            else
            {
                projectile.velocity = Vector2.Zero;

                //Factors for calculations
                double deg = (double)projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = projectile.ai[0]; //Distance away from the player

                /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
                /distance for the desired distance away from the player minus the projectile's width   /
                /and height divided by two so the center of the projectile is at the right place.     */
                projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
                projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

                //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                projectile.ai[1] += (1f * rotateSpeed);
            }

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, projectile.velocity.X, projectile.velocity.Y, 50, default(Color), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(projectile.position.X, projectile.position.Y), 32, projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + projectile.Center) / 2f;
                dustTrail.noGravity = true;
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

        public override void Kill(int timeLeft)
        {
            Player owner = Main.player[projectile.owner];
            owner.GetModPlayer<OvermorrowModPlayer>().sandCount--;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 3;
        }
    }
}