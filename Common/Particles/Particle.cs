using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Rendering;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Common.Particles
{
    public abstract class Particle : IRenderable
    {
        public Color Color;
        public float[] CustomData;
        public int ActiveTime;
        public float Alpha;
        public Vector2 Position;
        private Renderer renderer;
        public float Rotation;
        public float Scale;
        public Vector2 Velocity;
        public virtual DrawLayer DrawLayer => DrawLayer.PreInterface;

        public Renderer Renderer
        {
            get => renderer;
            set
            {
                renderer?.Remove(this);
                renderer = value;
                renderer?.Add(this);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch) { }

        public virtual void OnSpawn() { }

        public virtual void Update() { }

        public virtual void DrawAdditive(SpriteBatch spriteBatch) { }

        public virtual bool Kill()
        {
            return true;
        }
    }
}
