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
    public class ParticleInstance
    {
        public CustomParticle cParticle;
        public int id;
        public Vector2 position;
        public Vector2 velocity;
        public float alpha;
        public float scale;
        public float rotation;
        public Color color = Color.White;
        public int activeTime;
        public Vector2[] oldPos = new Vector2[10];
        public ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll;

        protected Vector2 DirectionTo(Vector2 pos) => Vector2.Normalize(pos - position);
        public void Kill() => ParticleManager.RemoveAtIndex(id);

        // Gravity-aware position for drawing
        public Vector2 DrawPosition
        {
            get
            {
                Vector2 drawPos = position;

                // If gravity is flipped, adjust the position
                if (Main.LocalPlayer.gravDir == -1f)
                {
                    // Flip Y position relative to screen center
                    Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
                    drawPos.Y = screenCenter.Y - (position.Y - screenCenter.Y);
                }

                return drawPos;
            }
        }
    }

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
                if (particle == null || particle.drawLayer != layer) continue;

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

        public static int CreateParticle(CustomParticle customParticle, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll)
        {
            ParticleInstance particle = CreateParticleDirect(customParticle, position, velocity, color, alpha, scale, rotation, drawLayer);
            return particle?.id ?? -1;
        }

        public static ParticleInstance CreateParticleDirect(CustomParticle customParticle, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll)
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
                drawLayer = drawLayer
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