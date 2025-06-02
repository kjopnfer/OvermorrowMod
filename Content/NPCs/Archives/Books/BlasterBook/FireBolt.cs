using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria.ModLoader;
using OvermorrowMod.Common.Utilities;
using Terraria;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using System;
using OvermorrowMod.Core.Particles;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class FireBolt : ModProjectile, ITrailEntity
    {
        public IEnumerable<TrailConfig> TrailConfigurations()
        {
            return new List<TrailConfig>
            {
                new TrailConfig(
                    typeof(FireTrail),
                    progress => Color.Lerp(Color.Blue, Color.Cyan, progress) * MathHelper.SmoothStep(0, 1, progress),
                    progress => 30
                ),
                new TrailConfig(
                    typeof(FireTrail),
                    progress => DrawUtils.ColorLerp3(Color.Purple, Color.MediumOrchid, Color.BlueViolet, progress) * 0.5f *  MathHelper.SmoothStep(0, 1, progress),
                    progress => 40
                )
            };
        }

        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            float frequency = 0.1f; // Controls how quickly the sine wave oscillates
            float amplitude = 0.5f;   // Controls the amplitude of the sinusoidal rotation

            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0.5f);

            // Compute the sine-based offset
            float sineOffset = (float)Math.Sin(Projectile.ai[0]++ * frequency) * amplitude;

            // Rotate velocity by the sine offset
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(sineOffset));


            base.AI();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            float baseSpeed = Main.rand.NextFloat(1f); // Base speed of the particles
            for (int repeat = 0; repeat < Main.rand.Next(3, 5); repeat++)
            {
                int numParticles = Main.rand.Next(8, 16); // Number of particles to spawn
                for (int i = 0; i < numParticles; i++)
                {
                    Color color = Color.Cyan;

                    float angle = MathHelper.TwoPi / numParticles * i;
                    float scale = Main.rand.NextFloat(0.1f, 0.5f);

                    // Adjust the velocity to create a horizontal oval shape
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * baseSpeed;

                    // Add a small random offset to the center
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));

                    var lightOrb = new Circle(0f, scale * 0.5f);
                    ParticleManager.CreateParticleDirect(lightOrb, Projectile.Bottom + offset, velocity, color, 1f, scale, 0f);
                }
            }

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float particleScale = 0.1f;
            if (!Main.gamePaused)
            {
                int randomIterations = Main.rand.Next(2, 5);
                Vector2 drawOffset = new Vector2(-4, -4).RotatedBy(Projectile.rotation);
                Color color = Color.Lerp(Color.Cyan, Color.Cyan, Main.rand.NextFloat(0, 1f));
                for (int i = 0; i < randomIterations; i++)
                {                    
                    var emberParticle = new Circle(0f, particleScale, useSineFade: true); // Default max time, custom scale, sine fade
                    ParticleManager.CreateParticleDirect(emberParticle, Projectile.Center, -Projectile.velocity * 0.1f, Color.Purple, 1f, particleScale, 0f);
                }
            }

            return base.PreDraw(ref lightColor);
        }

    }
}