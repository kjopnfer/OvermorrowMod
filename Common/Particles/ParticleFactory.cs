using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public static class ParticleFactory
    {
        public static Particle CreateSpark(Color startColor, Color endColor, int lifetime = 120)
        {
            return new Particle("OvermorrowMod/Particles/Spark")
            {
                physics = ParticlePhysics.Gravity,
                gravityStrength = 0.15f,
                friction = 0.98f,
                canBounce = true,
                bounciness = 0.6f,
                maxBounces = 3,
                hasColorTransition = true,
                startColor = startColor,
                endColor = endColor,
                lifetime = lifetime
            };
        }

        public static Particle CreateExpandingCircle(float maxSize = 3f, int lifetime = 45)
        {
            return new Particle("OvermorrowMod/Particles/Circle")
            {
                sizeBehavior = ParticleBehavior.Linear,
                minSize = 0f,
                maxSize = maxSize,
                lifetime = lifetime,
                hasColorTransition = true,
                startColor = Color.White,
                endColor = Color.Transparent
            };
        }

        public static Particle CreateFloatingDust(Color color)
        {
            return new Particle("OvermorrowMod/Particles/Dust")
            {
                physics = ParticlePhysics.Float,
                waveAmplitude = 2f,
                waveFrequency = 0.05f,
                sizeBehavior = ParticleBehavior.Sine,
                sizeAmplitude = 0.3f,
                sizeFrequency = 0.08f,
                hasColorTransition = true,
                startColor = color,
                endColor = Color.Lerp(color, Color.Transparent, 0.7f),
                lifetime = 180
            };
        }

        public static Particle CreateRainDrop()
        {
            return new Particle("OvermorrowMod/Particles/Raindrop")
            {
                physics = ParticlePhysics.Gravity,
                gravityStrength = 0.2f,
                friction = 0.99f
            };
        }
    }
}
