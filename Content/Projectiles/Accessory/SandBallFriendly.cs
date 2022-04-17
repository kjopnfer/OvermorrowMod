using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Accessory
{
    public class SandBallFriendly : ModProjectile
    {
        private bool runOnce = true;
        private float rotateSpeed;
        private int delayAttack = 60;
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (runOnce)
            {
                rotateSpeed = Projectile.ai[1];
                runOnce = false;
            }

            //Making player variable "p" set as the projectile's owner
            Player player = Main.player[Projectile.owner];

            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.timeLeft = 2;
            }

            if (player.GetModPlayer<OvermorrowModPlayer>().sandMode == 1) // Attack Mode
            {
                if (Projectile.localAI[0] == 0f)
                {
                    if (Projectile.velocity.Length() > 6f) Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 6;
                    Projectile.localAI[0] = 1f;
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
                            Vector2 newMove = Main.npc[k].Center - Projectile.Center;
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
                    if (move.Length() > 6f) move = move.SafeNormalize(Vector2.Zero) * 6f;
                    Projectile.velocity = (10 * Projectile.velocity + move) / 11f;
                    if (Projectile.velocity.Length() > 6f) Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 6;
                }
                else
                {
                    Projectile.velocity = Vector2.Zero;
                    //Factors for calculations
                    double deg = (double)Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                    double rad = deg * (Math.PI / 180); //Convert degrees to radians
                    double dist = Projectile.ai[0]; //Distance away from the player

                    /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
                    /distance for the desired distance away from the player minus the projectile's width   /
                    /and height divided by two so the center of the projectile is at the right place.     */
                    Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
                    Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                    //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                    Projectile.ai[1] += (1f * rotateSpeed);
                }
            }
            else
            {
                Projectile.velocity = Vector2.Zero;

                //Factors for calculations
                double deg = (double)Projectile.ai[1]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double dist = Projectile.ai[0]; //Distance away from the player

                /*Position the player projectiled on where the player is, the Sin/Cos of the angle times the /
                /distance for the desired distance away from the player minus the projectile's width   /
                /and height divided by two so the center of the projectile is at the right place.     */
                Projectile.position.X = player.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;

                //Increase the counter/angle in degrees by 1 point, you can change the rate here too, but the orbit may look choppy depending on the value
                Projectile.ai[1] += (1f * rotateSpeed);
            }

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(Projectile.position.X, Projectile.position.Y), 32, Projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Player owner = Main.player[Projectile.owner];
            owner.GetModPlayer<OvermorrowModPlayer>().sandCount--;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 3;
        }
    }
}