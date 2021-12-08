using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using OvermorrowMod.WardenClass;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace WardenClass
{
    public class DevouredSoul : OrbitingProjectileNPC, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Red;
        public float TrailSize(float progress) => 3;
        public Type TrailType()
        {
            return typeof(SoulTrail);
        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        private bool reductionCheck = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devoured Soul");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 60 * 20;

            ProjectileSlot = 1;
            Period = 300;
            PeriodFast = 100;
            ProjectileSpeed = 8;
            OrbitingRadius = 100;
            CurrentOrbitingRadius = 300;
        }

        public override void AI()
        {
            if (Proj_State != 0)
            {
                if (!npc.active)
                {
                    projectile.Kill();
                }

                if (!reductionCheck)
                {
                    npc.damage -= 4;
                    reductionCheck = true;
                }

                RelativeVelocity = npc.velocity;
                OrbitCenter = npc.Center;
            }

            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);

            projectile.rotation += 0.04f;
            projectile.ai[1]++;

            base.AI();
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.OrangeRed, projectile.rotation, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 0.5f, (float)Math.Sin(projectile.ai[1] / 10f)), 1f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.OrangeRed, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(MathHelper.Lerp(0.3f, 0.5f, (float)Math.Sin(projectile.ai[1] / 10f)), 1f), SpriteEffects.None, 0);

        }

        public override void Kill(int timeLeft)
        {
            if (Proj_State == 1 || Proj_State == 2)
            {
                GeneratePositionsAfterKill();
            }

            if (reductionCheck && npc != null)
            {
                npc.damage += 4;
            }

            Vector2 origin = projectile.Center;
            float radius = 5;
            int numLocations = 3;

            for (int i = 0; i < 3; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 0.5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * 2;

                Particle.CreateParticle(Particle.ParticleType<Shockwave>(), position, dustvelocity, Color.Black, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
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

        // The attack state homes in on nearby enemies
        public override void Attack()
        {
            projectile.spriteDirection = projectile.direction;

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

                if (target)
                {
                    AdjustMagnitude(ref move);
                    projectile.velocity = (20 * projectile.velocity + move) / 11f;
                    AdjustMagnitude(ref projectile.velocity);
                }
            }
        }

        // Set the orbiting position to the NPC that was just hit
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            npc = target;

            // Set the state to initialization
            Proj_State = 4;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Proj_State == 1 || Proj_State == 2)
            {
                return false;
            }

            return base.CanHitNPC(target);
        }
    }
}