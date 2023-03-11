using System;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Common.Rendering
{
    public class ExternalRenderable : IRenderable
    {
        private readonly Action draw;

        public ExternalRenderable(Action drawMethod)
        {
            draw = drawMethod;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            draw();
        }
    }
}