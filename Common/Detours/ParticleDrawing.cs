using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.NPCs;
using OvermorrowMod.Common.Particles;
using Terraria;

namespace OvermorrowMod.Common.Detours
{
    public class ParticleDrawing
    {
        public static void DrawParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            Particle.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self, time);
        }
    }
}