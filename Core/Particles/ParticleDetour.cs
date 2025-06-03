using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
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
        }

        public void Unload()
        {
            // No special unload logic needed currently.
        }

        private void DrawParticles(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            // Custom particle drawing logic
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            ParticleManager.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();

            // Call the original method
            orig(self, time);
        }
    }
}
