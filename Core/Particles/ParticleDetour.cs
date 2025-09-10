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

            Terraria.On_Main.DrawPlayers_BehindNPCs += DrawParticlesBehindNPCs;
            Terraria.On_Main.DrawProjectiles += DrawParticlesProjectile;
            Terraria.On_Main.DrawInterface += DrawParticlesInterface;
            Terraria.On_Main.DrawDust += DrawParticlesDust;
        }

        public void Unload()
        {
            // No special unload logic needed currently.
        }

        private void DrawParticlesBehindNPCs(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.BehindNPCs);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.BehindNPCs);
            Main.spriteBatch.End();

            orig(self);
        }

        private void DrawParticlesInterface(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.Interface);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.Interface);
            Main.spriteBatch.End();

            orig(self, time);
        }

        private void DrawParticlesProjectile(Terraria.On_Main.orig_DrawProjectiles orig, Main self)
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

        private void DrawParticlesDust(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.AboveAll);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(default, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawAdditive(Main.spriteBatch, ParticleDrawLayer.AboveAll);
            Main.spriteBatch.End();

            orig(self);
        }
    }
}
