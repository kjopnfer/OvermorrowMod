using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Particle
    {
        public CustomParticle cParticle;
        public int type;
        public int extraUpdates;
        public int id;
        public Vector2 position;
        public Vector2 velocity;
        public float alpha;
        public float scale;
        public float rotation;
        public Color color = Color.White;
        public int activeTime;
        public Vector2[] oldPos = new Vector2[10];
        public float[] customData;
        private static int MaxParticleCount = 5000;
        public static int NextIndex;
        public static int ActiveParticles;
        public static Particle[] particles;
        public static Dictionary<Type, int> ParticleTypes;
        public static Dictionary<int, Texture2D> ParticleTextures;
        public static Dictionary<int, string> ParticleNames;
        public static void Load()
        {
            particles = new Particle[MaxParticleCount];
            ParticleTypes = new Dictionary<Type, int>();
            ParticleTextures = new Dictionary<int, Texture2D>();
            ParticleNames = new Dictionary<int, string>();
            CustomParticle.CustomParticles = new Dictionary<int, CustomParticle>();
            //On.Terraria.Main.DrawInterface += Draw;
        }

        public static void TryRegisteringParticle(Type type)
        {
            Type baseType = typeof(CustomParticle);
            if (type.IsSubclassOf(baseType) && !type.IsAbstract && type != baseType)
            {
                int id = ParticleTypes.Count;
                ParticleTypes.Add(type, id);
                CustomParticle particle = (CustomParticle)Activator.CreateInstance(type);
                particle.mod = OvermorrowModFile.Instance;
                CustomParticle.CustomParticles.Add(id, particle);
                var texture = ModContent.Request<Texture2D>(particle.Texture ?? type.FullName.Replace('.', '/')).Value;
                ParticleTextures.Add(id, texture);
                ParticleNames.Add(id, type.Name);
            }
        }

        public static void Unload()
        {
            NextIndex = -1;
            ActiveParticles = -1;
            //On.Terraria.Main.DrawInterface -= Draw;
            particles = null;
            ParticleTypes = null;
            ParticleTextures = null;
            ParticleNames = null;
            CustomParticle.CustomParticles = null;
        }
        protected Vector2 DirectionTo(Vector2 pos) => Vector2.Normalize(pos - position);
        public void Kill() => Particle.RemoveAtIndex(id);

        public static void UpdateParticles()
        {
            foreach (Particle particle in particles)
            {
                if (particle == null) continue;
                for (int i = 0; i < particle.extraUpdates + 1; i++)
                {
                    particle.activeTime++;
                    if (particle.cParticle.ShouldUpdatePosition())
                        particle.position += particle.velocity;
                    particle.cParticle.particle = particle;
                    particle.cParticle.Update();
                    particle.oldPos[0] = particle.position;
                    for (int j = (particle.oldPos.Length - 1); j > 0; j--)
                        particle.oldPos[j] = particle.oldPos[j - 1];
                }
            }
        }

        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                if (particle == null) continue;
                particle.cParticle.particle = particle;
                try
                {
                    particle.cParticle.Draw(spriteBatch);
                }
                catch (Exception e)
                {
                    OvermorrowModFile.Instance.Logger.Error(e.Message);
                    OvermorrowModFile.Instance.Logger.Error(e.StackTrace);
                    Main.NewText($"Error while drawing particles, error caused by particle of type: {ParticleNames[particle.type]}.", Color.Red);
                    Main.NewText("Read client.log for the full error message.", Color.Red);
                    particle.Kill();
                }
            }
        }

        public static Texture2D GetTexture(int type) => ParticleTextures[type];
        public static int ParticleType<T>() where T : CustomParticle => ParticleTypes[typeof(T)];
        public static void RemoveAtIndex(int index)
        {
            particles[index] = null;
            ActiveParticles--;
            NextIndex = index;
        }

        public static bool ParticleExists(int type)
        {
            foreach (Particle particle in particles)
            {
                if (particle == null) continue;
                if (particle.type == type) return true;
            }
            return false;
        }

        public static void ClearParticles()
        {
            for (int i = 0; i < particles.Length; i++) particles[i] = null;
            NextIndex = 0;
            ActiveParticles = 0;
        }

        /// <summary>
        /// Creates a particle of the specified type and properties, and returns its ID.
        /// If the maximum particle count is reached, the ID of the last particle is returned instead.
        /// </summary>
        /// <param name="type">The type of the particle to create.</param>
        /// <param name="position">The initial position of the particle.</param>
        /// <param name="velocity">The initial velocity of the particle.</param>
        /// <param name="color">The color of the particle.</param>
        /// <param name="alpha">The alpha (transparency) of the particle. Default is 1 (fully opaque).</param>
        /// <param name="scale">The scale (size) of the particle. Default is 1 (original size).</param>
        /// <param name="rotation">The rotation of the particle. Default is 0 (no rotation).</param>
        /// <param name="data1">Custom data value 1 for the particle. Default is 0.</param>
        /// <param name="data2">Custom data value 2 for the particle. Default is 0.</param>
        /// <param name="data3">Custom data value 3 for the particle. Default is 0.</param>
        /// <param name="data4">Custom data value 4 for the particle. Default is 0.</param>
        /// <returns>
        /// The ID of the newly created particle. If the maximum particle count is reached, returns
        /// the ID of the last created particle (MaxParticleCount - 1).
        /// </returns>
        public static int CreateParticle(int type, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, float customData0 = 0, float customData1 = 0, float customData2 = 0, float customData3 = 0)
        {
            Particle particle = CreateParticleDirect(type, position, velocity, color, alpha, scale, rotation, customData0, customData1, customData2, customData3);
            return particle?.id ?? MaxParticleCount - 1;  // Return the ID if particle was created, otherwise return the max index
        }

        /// <summary>
        /// Creates a particle of the specified type and properties, and returns the particle object.
        /// If the maximum particle count is reached, it returns the last particle created (MaxParticleCount - 1).
        /// </summary>
        /// <param name="type">The type of the particle to create.</param>
        /// <param name="position">The initial position of the particle.</param>
        /// <param name="velocity">The initial velocity of the particle.</param>
        /// <param name="color">The color of the particle.</param>
        /// <param name="alpha">The alpha (transparency) of the particle. Default is 1 (fully opaque).</param>
        /// <param name="scale">The scale (size) of the particle. Default is 1 (original size).</param>
        /// <param name="rotation">The rotation of the particle. Default is 0 (no rotation).</param>
        /// <param name="data1">Custom data value 1 for the particle. Default is 0.</param>
        /// <param name="data2">Custom data value 2 for the particle. Default is 0.</param>
        /// <param name="data3">Custom data value 3 for the particle. Default is 0.</param>
        /// <param name="data4">Custom data value 4 for the particle. Default is 0.</param>
        /// <returns>
        /// The newly created particle object, or the last particle if the maximum particle count is reached.
        /// </returns>
        public static Particle CreateParticleDirect(int type, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, float customData0 = 0, float customData1 = 0, float customData2 = 0, float customData3 = 0)
        {
            if (ActiveParticles >= MaxParticleCount)
            {
                return particles[MaxParticleCount - 1];  // Return the last particle (max count - 1)
            }

            // Debugging check: Ensure the type exists in CustomParticles
            if (!CustomParticle.CustomParticles.ContainsKey(type))
            {
                Main.NewText($"No CustomParticle found for type {type}", Color.Red);
                return null; // Return null if the type is invalid
            }

            Particle particle = new Particle
            {
                type = type,
                position = position,
                velocity = velocity,
                color = color,
                alpha = alpha,
                scale = scale,
                rotation = rotation,
                customData = new[] { customData0, customData1, customData2, customData3 },
                id = NextIndex
            };

            // Initialize the custom particle for the type
            CustomParticle particle1 = (CustomParticle)Activator.CreateInstance(CustomParticle.GetCParticle(particle.type).GetType());
            particle.cParticle = particle1;
            particle.cParticle.particle = particle;
            particle.cParticle.OnSpawn();

            // Add to particles array
            particles[NextIndex] = particle;

            // Update NextIndex
            UpdateNextIndex();

            ActiveParticles++;
            return particle;
        }

        // Update the NextIndex after particle addition
        private static void UpdateNextIndex()
        {
            if (NextIndex + 1 < particles.Length && particles[NextIndex + 1] == null)
                NextIndex++;
            else
                for (int i = 0; i < particles.Length; i++)
                    if (particles[i] == null)
                    {
                        NextIndex = i;
                        break;
                    }
        }
    }
}