using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class Particle : CustomParticle
    {
        // Behavior configuration
        public ParticleBehavior sizeBehavior = ParticleBehavior.Static;
        public ParticlePhysics physics = ParticlePhysics.None;

        // Size behavior parameters
        public float sizeAmplitude = 1f;
        public float sizeFrequency = 0.1f;
        public float maxSize = 2f;
        public float minSize = 0f;

        // Physics parameters
        public float gravityStrength = 0.1f;
        public float friction = 0.99f;
        public float waveAmplitude = 1f;
        public float waveFrequency = 0.05f;

        // Color transition
        public bool hasColorTransition = false;
        public Color startColor = Color.White;
        public Color endColor = Color.Transparent;
        public int lifetime = 60;

        // Bouncing
        public bool canBounce = false;
        public float bounciness = 0.7f;
        public int maxBounces = 3;
        public int bounceCount = 0;

        // Cached values
        private Vector2 baseVelocity;
        private bool hasInitialized = false;

        public Particle(string texture = null)
        {
            if (!string.IsNullOrEmpty(texture))
                this.Texture = texture;
        }

        public override void OnSpawn()
        {
            if (hasColorTransition) particle.color = startColor;
            baseVelocity = particle.velocity;
            hasInitialized = true;
        }

        public override void Update()
        {
            if (!hasInitialized) return;

            // Apply physics
            switch (physics)
            {
                case ParticlePhysics.Gravity:
                    particle.velocity.Y += gravityStrength;
                    particle.velocity *= friction;
                    HandleBouncing();
                    break;

                case ParticlePhysics.Float:
                    float wave = MathF.Sin(particle.activeTime * waveFrequency) * waveAmplitude;
                    particle.velocity = baseVelocity + new Vector2(wave * 0.1f, wave * 0.05f);
                    break;

                case ParticlePhysics.Orbit:
                    float angle = particle.activeTime * waveFrequency;
                    particle.position += new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * waveAmplitude * 0.1f;
                    break;
            }

            // Apply size behavior
            switch (sizeBehavior)
            {
                case ParticleBehavior.Linear:
                    float linearProgress = (float)particle.activeTime / lifetime;
                    particle.scale = MathHelper.Lerp(minSize, maxSize, linearProgress);
                    break;

                case ParticleBehavior.Sine:
                    float sineValue = MathF.Sin(particle.activeTime * sizeFrequency) * sizeAmplitude;
                    particle.scale = MathHelper.Clamp(1f + sineValue, minSize, maxSize);
                    break;

                case ParticleBehavior.Exponential:
                    float expProgress = (float)particle.activeTime / lifetime;
                    particle.scale = MathHelper.Lerp(minSize, maxSize, expProgress * expProgress);
                    break;

                case ParticleBehavior.Bounce:
                    float bounceValue = MathF.Abs(MathF.Sin(particle.activeTime * sizeFrequency)) * sizeAmplitude;
                    particle.scale = minSize + bounceValue;
                    break;
            }

            // Handle color transitions
            if (hasColorTransition && lifetime > 0)
            {
                float progress = MathHelper.Clamp((float)particle.activeTime / lifetime, 0f, 1f);
                particle.color = Color.Lerp(startColor, endColor, progress);
                if (progress >= 1f) particle.Kill();
            }
        }

        private void HandleBouncing()
        {
            if (!canBounce) return;
            if (particle.position.Y > Main.worldSurface * 16 && particle.velocity.Y > 0)
            {
                if (bounceCount < maxBounces)
                {
                    particle.velocity.Y *= -bounciness;
                    particle.velocity.X *= 0.8f;
                    bounceCount++;
                }
                else particle.Kill();
            }
        }
    }
}