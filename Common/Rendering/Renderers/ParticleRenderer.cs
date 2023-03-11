using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core.Interfaces;
using Terraria;

namespace OvermorrowMod.Common.Rendering.Renderers
{
    public class DefaultParticleRenderer : Renderer
    {
        public override bool UsesTargets => false;
        public override DrawLayer DrawLayer => DrawLayer.PreInterface;

        public override void DrawToScreen()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            for (int index = 0; index < renders.Count; index++)
            {
                IRenderable renderable = renders[index];
                if (renderable is Particle particle)
                    particle.Draw(Main.spriteBatch);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState,
                DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            for (int index = 0; index < renders.Count; index++)
            {
                IRenderable renderable = renders[index];
                if (renderable is Particle particle)
                    particle.DrawAdditive(Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }
    }
}