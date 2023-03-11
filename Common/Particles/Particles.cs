using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Rendering;
using OvermorrowMod.Common.Rendering.Renderers;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class ParticleSystem : ModSystem
    {
        public static List<Particle> ActiveParticles;

        public override void Load()
        {
            ActiveParticles = new List<Particle>();
        }

        public override void Unload()
        {
            ActiveParticles = null;
        }

        public override void PostUpdateDusts()
        {
            for (int i = 0; i < ActiveParticles.Count; i++)
            {
                Particle particle = ActiveParticles[i];
                if (--ActiveTime <= 0)
                    if (Kill())
                    {
                        particle.Renderer = null;
                        ActiveParticles.Remove(particle);
                        i--;
                        continue;
                    }

                Position += Velocity;
                particle.Update();
            }
        }

        public static void CreateParticle<T>(Vector2 position, Vector2 velocity = default, Color color = default,
        float scale = 1f, float alpha = 1f, float[] data = default) where T : Particle
        {
            var particle = (Particle)Activator.CreateInstance(typeof(T));
            Position = position;
            Velocity = velocity;
            Color = color == default ? Color.White : color;
            Scale = scale;
            particle.Opacity = alpha;
            particle.Data = data;
            particle.OnSpawn();
            particle.Renderer = RenderingSystem.GetRenderer<DefaultParticleRenderer>();
            ActiveParticles.Add(particle);
        }
    }
}