using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Particles
{
    /// <summary>
    /// Handles custom particle rendering outside of the base Terraria draw flow.
    /// </summary>
    public class ParticleDetour : ILoadable
    {
        public void Load(Mod mod)
        {
            if (Main.dedServ)
                return;

            Terraria.On_Main.DrawPlayers_BehindNPCs += DrawParticles;
            Terraria.On_Main.DrawProjectiles += DrawParticles;
            Terraria.On_Main.DrawInterface += DrawParticles;
        }

        public void Unload()
        {
            // No special unload logic needed currently.
        }

        private void DrawParticles(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.BehindNPCs);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.BehindNPCs);
            Main.spriteBatch.End();

            orig(self);
        }

        private void DrawParticles(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.AboveAll);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.AboveAll);
            Main.spriteBatch.End();

            orig(self, time);
        }

        private void DrawParticles(Terraria.On_Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.BehindProjectiles);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.BehindProjectiles);
            Main.spriteBatch.End();

            orig(self);
        }
    }
}
