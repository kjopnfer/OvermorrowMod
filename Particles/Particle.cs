using System;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Particles
{
    public class Particle
    {
        public CustomParticle cParticle;
        public int type;
        public int id;
        public int frame;
        public Vector2 position;
        public Vector2 velocity;
        public float alpha;
        public float scale;
        public float rotation;
        public float frameCounter;
        public Color color = Color.White;
        public int activeTime;
        public float[] customData;
        public void Kill() => Particle.RemoveAtIndex(id);
        protected Vector2 DirectionTo(Vector2 pos) => Vector2.Normalize(pos - position);
        private static readonly int MaxParticleCount = 500;
        public static int NextIndex;
        public static int ActiveParticles;
        public static Particle[] particles;
        public static Dictionary<Type, int> ParticleTypes;
        public static Dictionary<int, Texture2D> ParticleTextures;
        public static int[] ParticleFrameCount;
        public static void Load()
        {
            particles = new Particle[MaxParticleCount];
            ParticleTypes = new Dictionary<Type, int>();
            ParticleTextures = new Dictionary<int, Texture2D>();
			CustomParticle.CustomParticles = new Dictionary<int, CustomParticle>();
            OvermorrowModFile mod = OvermorrowModFile.Mod;
            foreach (Type type in mod.Code.GetTypes())
            {
                if (type.IsSubclassOf(typeof(CustomParticle)) && !type.IsAbstract && type != typeof(CustomParticle))
                {
                    int id = ParticleTypes.Count;
                    ParticleTypes.Add(type, id);
                    CustomParticle particle = (CustomParticle)Activator.CreateInstance(type);
                    particle.mod = mod;
                    CustomParticle.CustomParticles.Add(id, particle);
                    Texture2D texture = particle.Texture == null ? mod.GetTexture("Particles/" + type.Name) : mod.GetTexture(particle.Texture);
                    ParticleTextures.Add(id, texture);
                }
            }
            ParticleFrameCount = new int[ParticleTypes.Count];
            for (int i = 0; i < ParticleFrameCount.Length; i++)
            {
                ParticleFrameCount[i] = 1;
            }
        }
        public static void Unload()
        {
            particles = null;
            ParticleTypes = null;
            ParticleTextures = null;
            CustomParticle.CustomParticles = null;
            ParticleFrameCount = null;
        }
        public static void UpdateParticles()
        {
            foreach(Particle particle in particles)
            {
                if (particle == null) continue;
                CustomParticle particle1 = CustomParticle.GetCParticle(particle.type);
                particle1.particle = particle;
                particle1.Update();
                particle.activeTime++;
				if (particle.cParticle.ShouldUpdatePosition())
				    particle.position += particle.velocity;
            }
        }
        public static void DrawParticles(SpriteBatch spriteBatch)
        {
            foreach(Particle particle in particles)
            {
                if (particle == null) continue;
                CustomParticle particle1 = CustomParticle.GetCParticle(particle.type);
                particle1.particle = particle;
                particle1.Draw(spriteBatch);
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
            foreach(Particle particle in particles)
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
        public static int CreateParticle(int type, Vector2 position, Vector2 velocity, Color color, float alpha = 1f, float scale = 1f, float rotation = 0f, float data1 = 0, float data2 = 0, float data3 = 0, float data4 = 0)
        {
            if (ActiveParticles >= MaxParticleCount) return MaxParticleCount - 1;
            Particle particle = new Particle();
            particle.type = type;
            particle.position = position;
            particle.velocity = velocity;
            particle.color = color;
            particle.alpha = alpha;
            particle.scale = scale;
            particle.rotation = rotation;
            particle.customData = new float[4] {data1, data2, data3, data4};
            particle.id = NextIndex;
            
            CustomParticle particle1 = CustomParticle.GetCParticle(particle.type);
            particle1.particle = particle;
            particle.cParticle = particle1;
            particle1.OnSpawn();

            particles[NextIndex] = particle;
            if (NextIndex + 1 < particles.Length && particles[NextIndex + 1] == null)
				NextIndex++;
			else
				for (int i = 0; i < particles.Length; i++)
					if (particles[i] == null)
						NextIndex = i;

            ActiveParticles++;
            return particle.id;
        }
    }
}