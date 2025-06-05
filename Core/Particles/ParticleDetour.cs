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

            Terraria.On_Main.DrawInterface += DrawParticles;
            Terraria.On_Main.DrawProjectiles += DrawParticles;
        }

        public void Unload()
        {
            // No special unload logic needed currently.
        }

        private void DrawParticles(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.AboveAll);
            Main.spriteBatch.End();

            orig(self, time);
        }

        private void DrawParticles(Terraria.On_Main.orig_DrawProjectiles orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch, ParticleDrawLayer.BehindProjectiles);
            Main.spriteBatch.End();

            orig(self);
        }
    }
}
