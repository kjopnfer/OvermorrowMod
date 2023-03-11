using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Common.Rendering
{
    public abstract class Renderer
    {
        protected List<IRenderable> renders = new();
        public virtual bool UsesTargets => true;
        public virtual DrawLayer DrawLayer => DrawLayer.PreProjectiles;

        public void Add(IRenderable renderable)
        {
            renders.Add(renderable);
        }

        public void Remove(IRenderable renderable)
        {
            renders.Remove(renderable);
        }

        public virtual void PrepareTargets(GraphicsDevice device) { }

        public virtual void DrawToTarget(GraphicsDevice device, SpriteBatch spriteBatch) { }

        public abstract void DrawToScreen();
    }
}