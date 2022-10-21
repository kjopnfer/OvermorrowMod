using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public void Kill() => Particle.RemoveAtIndex(id);
        protected Vector2 DirectionTo(Vector2 pos) => Vector2.Normalize(pos - position);
        private static int MaxParticleCount = 1000;
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
            particle.customData = new float[4] { data1, data2, data3, data4 };
            particle.id = NextIndex;

            CustomParticle particle1 = (CustomParticle)Activator.CreateInstance(CustomParticle.GetCParticle(particle.type).GetType());
            particle.cParticle = particle1;
            particle.cParticle.particle = particle;
            particle.cParticle.OnSpawn();

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
    public class CustomParticle
    {
        public static Dictionary<int, CustomParticle> CustomParticles;
        public OvermorrowModFile mod;
        public static CustomParticle GetCParticle(int type) => CustomParticles[type];
        public Particle particle;
        public virtual void OnSpawn() { }
        public virtual void Update() { }
        public virtual string Texture { get { return null; } private set { } }
        public virtual bool ShouldUpdatePosition() => true;
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Particle.GetTexture(particle.type);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, new Vector2(texture.Width / 2, texture.Height / 2), particle.scale, SpriteEffects.None, 0f);
        }
    }
}
