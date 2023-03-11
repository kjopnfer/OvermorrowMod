using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace OvermorrowMod.Common.Particles
{
    public class BlackFlame : Particle
    {
        float maxTime = 45;
        public override void OnSpawn()
        {
            Color = Color.Lerp(Color.Purple, Color.Violet, Scale);
            if (Main.rand.NextBool(3))
            {
                CustomData[0] *= 2;
            }
        }

        public override void Update()
        {
            if (ActiveTime > maxTime) Kill();
            if (ActiveTime < 10)
            {
                float progress = (float)ActiveTime / 10f;
                Scale = MathHelper.Lerp(0, CustomData[0], progress);
                Alpha = progress;
            }

            if (ActiveTime > 35)
            {
                float progress = (float)(ActiveTime - 35) / 10f;
                Scale = MathHelper.Lerp(CustomData[0], 0f, progress);
                Alpha = 1f - progress;
            }

            Rotation += 0.1f;
            Velocity *= 0f;
        }
    }
}