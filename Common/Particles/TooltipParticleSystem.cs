using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public delegate void UpdateFunction(TParticle particle);
    public delegate void DrawFunctionLine(TParticle particle, DrawableTooltipLine line, SpriteBatch spriteBatch);
    public delegate void DrawFunction(TParticle particle, SpriteBatch spriteBatch);
    public delegate void InitializeFunction(TParticle particle);
    public class TParticle
    {
        public TParticleSystem parent;
        public Vector2 position;
        public Vector2 velocity;
        public Color color;
        public float alpha;
        public float scale;
        public Texture2D texture;
        public float[] ai;
        public bool dead;
        public UpdateFunction Update;
        public DrawFunctionLine DrawLine;
        public DrawFunction Draw;
    }
    public class TParticleSystem
    {
        public List<TParticle> particles;
        public TParticleSystem()
        {
            particles = new();
        }
        public void UpdateParticles()
        {
            for (int i = 0; i < particles.Count; i++)
            {
                TParticle part = particles[i];
                if (part.dead)
                {
                    particles.RemoveAt(i);
                    i--;
                }
                else
                {
                    part.Update(part);
                }
            }
        }
        public void DrawParticles(DrawableTooltipLine line)
        {
            foreach (TParticle particle in particles)
            {
                particle.DrawLine(particle, line, Main.spriteBatch);
            }
        }
        public void DrawParticles()
        {
            foreach (TParticle particle in particles)
            {
                particle.Draw(particle, Main.spriteBatch);
            }
        }

        public void CreateParticle(Vector2 pos, Vector2 vel, Color col, UpdateFunction update, Texture2D tex, DrawFunctionLine draw, InitializeFunction init = null)
        {
            TParticle particle = new TParticle
            {
                position = pos,
                velocity = vel,
                color = col,
                Update = update,
                texture = tex,
                DrawLine = draw,
                ai = new float[4],
                parent = this
            };
            init?.Invoke(particle);
            particles.Add(particle);
        }

        public void CreateParticle(Vector2 pos, Vector2 vel, Color col, UpdateFunction update, Texture2D tex, DrawFunction draw, InitializeFunction init = null)
        {
            TParticle particle = new TParticle
            {
                position = pos,
                velocity = vel,
                color = col,
                Update = update,
                texture = tex,
                Draw = draw,
                ai = new float[4],
                parent = this
            };
            init?.Invoke(particle);
            particles.Add(particle);
        }
    }
}
