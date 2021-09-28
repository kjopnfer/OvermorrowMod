using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Particles
{
    public static class TestDetours
    {
        public static void Load()
        {
            On.Terraria.Main.DrawInterface += DrawParticles;
        }
        public static void Unload()
        {
            On.Terraria.Main.DrawInterface -= DrawParticles;
        }
        public static void DrawParticles(On.Terraria.Main.orig_DrawInterface orig, Main self, GameTime time)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            Particle.DrawParticles(Main.spriteBatch);
            Main.spriteBatch.End();
            orig(self, time);
        }
    }
}