using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Particles
{
    public class BlackFlame : CustomParticle
    {
        float maxTime = 45;
        public override void OnSpawn()
        {
            particle.color = Color.Lerp(Color.Purple, Color.Violet, particle.scale);
            if (Main.rand.NextBool(3))
            {
                particle.customData[0] *= 2;
            }
        }
        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();
            if (particle.activeTime < 10)
            {
                float progress = (float)particle.activeTime / 10f;
                particle.scale = MathHelper.Lerp(0, particle.customData[0], progress);
                particle.alpha = progress;
            }
            if (particle.activeTime > 35)
            {
                float progress = (float)(particle.activeTime - 35) / 10f;
                particle.scale = MathHelper.Lerp(particle.customData[0], 0f, progress);
                particle.alpha = 1f - progress;
            }
            particle.rotation += 0.1f;
            particle.velocity *= 0f;
        }
    }
}