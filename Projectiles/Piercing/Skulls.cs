using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class Skulls : ModProjectile, ITrailEntity
    {
        private float storeDirection;
        private float trigCounter = 0;
        private float period = 30;
        private float amplitude = 10;
        private float previousR = 0;

        public Type TrailType()
        {
            return typeof(SkullTrail);
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull");
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                storeDirection = projectile.velocity.ToRotation();

                Vector2 origin = projectile.Center;
                float radius = 15;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 20f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, 160, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                }
            }

            trigCounter += 2 * (float)Math.PI / period;
            float r = amplitude * (float)Math.Sin(trigCounter) * (projectile.ai[1] == 0 ? 1 : -1);
            Vector2 instaVel = PolarVector(r - previousR, storeDirection + (float)Math.PI / 2);
            projectile.position += instaVel;
            previousR = r;
            //projectile.rotation = (projectile.velocity + instaVel).ToRotation() + (float)Math.PI / 27;

            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            projectile.spriteDirection = projectile.direction;

            if (projectile.ai[0] < 60)
            {
                projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(1f));
            }
            else
            {
                Vector2 move = Vector2.Zero;
                float distance = 1000f;
                bool target = false;
                for (int k = 0; k < 200; k++)
                {
                    if (Main.npc[k].CanBeChasedBy(this))
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
                    projectile.velocity = (20 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }

            /*if (projectile.ai[0] >= 61)
            {
                projectile.alpha = 0;
                Vector2 position = projectile.Center;
                Dust dust = Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }*/

            //projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Lighting.AddLight(projectile.Center, 0, 0.5f, 0.5f);

            projectile.ai[0]++;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }

        public Vector2 PolarVector(float radius, float theta)
        {
            return new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * radius;
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
                int dust = Dust.NewDust(position, 2, 2, 160, dustvelocity.X, dustvelocity.Y, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}