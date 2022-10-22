using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class RingSolid : CustomParticle
    {
        public override string Texture => AssetDirectory.Textures + "RingSolid";
        public float maxSize;
        public float maxTime;
        public override void OnSpawn()
        {
            if (particle.customData[0] == 0) particle.customData[0] = 60;

            maxTime = particle.customData[1] == 0 ? 60 : particle.customData[1];
            maxSize = particle.scale;
            particle.scale = 0f;
        }

        public override void Update()
        {
            particle.velocity = Vector2.Zero;

            float fadeTime = maxTime - 10;
;
            float progress = ModUtils.EaseOutCirc(particle.activeTime / maxTime);
            particle.scale = MathHelper.Lerp(particle.scale, maxSize, progress);
            //particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, Utils.Clamp(particle.activeTime - 10, 0, fadeTime) / fadeTime);
            particle.alpha = MathHelper.Lerp(particle.alpha, 0, particle.activeTime / maxTime);

            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "RingSolid").Value;
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}