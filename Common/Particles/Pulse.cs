using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Pulse : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "PulseCircle";
        public float maxSize { get { return particle.customData[0]; } set { particle.customData[0] = value; } }
        public float maxTime { get { return particle.customData[1]; } set { particle.customData[1] = value; } }
        public override void OnSpawn()
        {
            if (particle.customData[1] == 0) particle.customData[1] = 60;
            if (particle.customData[2] == 0) particle.customData[2] = 1;
            if (particle.customData[3] == 0) particle.customData[3] = 1;

            maxTime = particle.customData[1] == 0 ? 60 : particle.customData[1];
            maxSize = particle.scale;
            particle.scale = 0f;
        }

        public override void Update()
        {
            particle.velocity = Vector2.Zero;
            //float progress = particle.activeTime / maxTime;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            particle.scale = MathHelper.SmoothStep(particle.scale, maxSize, progress);
            particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}