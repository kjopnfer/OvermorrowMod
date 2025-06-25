using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles
{
    public static class ParticleManager
    {
        private static int MaxParticleCount = 5000;
        public static int NextIndex;
        public static int ActiveParticles;
        public static ParticleInstance[] particles;

        public static void Load()
        {
            particles = new ParticleInstance[MaxParticleCount];
            NextIndex = 0;
            ActiveParticles = 0;
        }

        public static void DrawAdditive(SpriteBatch spriteBatch)
        {
            // Draw all particles that are marked for additive blending
            foreach (ParticleInstance particle in particles)
            {
                if (particle == null || !particle.UseAdditiveBlending) continue;

                // Temporarily set the particle's position to the gravity-aware position for drawing
                Vector2 originalPosition = particle.position;
                particle.position = particle.DrawPosition;

                particle.cParticle.particle = particle;
                try
                {
                    particle.cParticle.Draw(spriteBatch);
                }
                catch (Exception e)
                {
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    Main.NewText($"Error drawing additive particle: {particle.cParticle.GetType().Name}", Color.Red);
                    particle.Kill();
                }

                // Restore original position
                particle.position = originalPosition;
            }
        }

        public static void Unload()
        {
            NextIndex = -1;
            ActiveParticles = -1;
            particles = null;
        }

        public static void UpdateParticles()
        {
            foreach (ParticleInstance particle in particles)
            {
                if (particle == null) continue;
                particle.activeTime++;

                if (particle.cParticle.ShouldUpdatePosition())
                {
                    // Apply gravity-aware velocity
                    Vector2 adjustedVelocity = particle.velocity;

                    // If gravity is flipped, flip the Y component of velocity
                    if (Main.LocalPlayer.gravDir == -1f)
                    {
                        adjustedVelocity.Y = -adjustedVelocity.Y;
                    }

                    particle.position += adjustedVelocity;
                }

                particle.cParticle.particle = particle;
                particle.cParticle.Update();

                // Update old positions for trails (using gravity-aware positions)
                for (int j = (particle.oldPos.Length - 1); j > 0; j--)
                    particle.oldPos[j] = particle.oldPos[j - 1];
                particle.oldPos[0] = particle.DrawPosition; // Use gravity-aware position for trails
            }
        }

        public static void DrawParticles(SpriteBatch spriteBatch, ParticleDrawLayer layer)
        {
            foreach (ParticleInstance particle in particles)
            {
                // Skip particles that use additive blending
                if (particle == null || particle.drawLayer != layer || particle.UseAdditiveBlending) continue;

                // Temporarily set the particle's position to the gravity-aware position for drawing
                Vector2 originalPosition = particle.position;
                particle.position = particle.DrawPosition;

                particle.cParticle.particle = particle;
                try
                {
                    particle.cParticle.Draw(spriteBatch);
                }
                catch (Exception e)
                {
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    Main.NewText($"Error drawing particle: {particle.cParticle.GetType().Name}", Color.Red);
                    particle.Kill();
                }

                // Restore original position
                particle.position = originalPosition;
            }
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            DrawParticles(spriteBatch, ParticleDrawLayer.AboveAll);
        }

        public static void RemoveAtIndex(int index)
        {
            particles[index] = null;
            ActiveParticles--;
            if (index < NextIndex) NextIndex = index;
        }

        public static void ClearParticles()
        {
            for (int i = 0; i < particles.Length; i++) particles[i] = null;
            NextIndex = 0;
            ActiveParticles = 0;
        }

        // Add overloads that support additive blending
        public static int CreateParticle(CustomParticle customParticle, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll, bool useAdditiveBlending = false)
        {
            ParticleInstance particle = CreateParticleDirect(customParticle, position, velocity, color, alpha, scale, rotation, drawLayer, useAdditiveBlending);
            return particle?.id ?? -1;
        }

        public static ParticleInstance CreateParticleDirect(CustomParticle customParticle, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll, bool useAdditiveBlending = false)
        {
            if (ActiveParticles >= MaxParticleCount || customParticle == null) return null;

            while (NextIndex < particles.Length && particles[NextIndex] != null) NextIndex++;
            if (NextIndex >= particles.Length) return null;

            // Adjust initial velocity for gravity if needed
            Vector2 adjustedVelocity = velocity;
            //if (Main.LocalPlayer.gravDir == -1f)
            //{
            //    adjustedVelocity.Y = -adjustedVelocity.Y;
            //}

            ParticleInstance particle = new ParticleInstance
            {
                position = position,
                velocity = adjustedVelocity, // Use gravity-adjusted velocity
                color = color,
                alpha = alpha,
                scale = scale,
                rotation = rotation,
                id = NextIndex,
                cParticle = customParticle,
                drawLayer = drawLayer,
                UseAdditiveBlending = useAdditiveBlending // Set the additive blending flag
            };

            particle.cParticle.particle = particle;
            particle.cParticle.OnSpawn();
            particles[NextIndex] = particle;
            ActiveParticles++;
            UpdateNextIndex();
            return particle;
        }

        private static void UpdateNextIndex()
        {
            for (int i = NextIndex + 1; i < particles.Length; i++)
            {
                if (particles[i] == null)
                {
                    NextIndex = i;
                    return;
                }
            }
            for (int i = 0; i < NextIndex; i++)
            {
                if (particles[i] == null)
                {
                    NextIndex = i;
                    return;
                }
            }
            NextIndex = particles.Length;
        }
    }
}