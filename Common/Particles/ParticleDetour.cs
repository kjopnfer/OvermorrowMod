using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using Terraria;

namespace OvermorrowMod.Common.Detours
{
    public class ParticleDetour
    {
        public static void DrawParticles(On_Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            // Custom particle drawing logic
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            Particle.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();

            // Call the original method
            orig(self, time);
        }
    }
}
