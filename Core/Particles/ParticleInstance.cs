using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using Terraria;

namespace OvermorrowMod.Core.Particles
{
    public class ParticleInstance
    {
        public CustomParticle cParticle;
        public int id;
        public Vector2 position;
        public Vector2 velocity;
        public float alpha;
        public float scale;
        public float rotation;
        public Color color = Color.White;
        public int activeTime;
        public Vector2[] oldPos = new Vector2[10];
        public ParticleDrawLayer drawLayer = ParticleDrawLayer.AboveAll;

        public bool UseAdditiveBlending { get; set; } = false;

        protected Vector2 DirectionTo(Vector2 pos) => Vector2.Normalize(pos - position);
        public void Kill() => ParticleManager.RemoveAtIndex(id);

        // Gravity-aware position for drawing
        public Vector2 DrawPosition
        {
            get
            {
                Vector2 drawPos = position;

                // If gravity is flipped, adjust the position
                if (Main.LocalPlayer.gravDir == -1f)
                {
                    // Flip Y position relative to screen center
                    Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
                    drawPos.Y = screenCenter.Y - (position.Y - screenCenter.Y);
                }

                return drawPos;
            }
        }
    }
}