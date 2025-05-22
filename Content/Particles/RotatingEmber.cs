using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Particles
{
    public class RotatingEmber : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;
        public float maxTime = Main.rand.Next(4, 7) * 10;
        public override void OnSpawn()
        {
            //particle.customData[0] = particle.scale;
            particle.scale = 0f;
            particle.alpha = 0;
            maxTime = Main.rand.Next(8, 10) * 10;
        }

        public override void Update()
        {
            particle.customData[0]++;
            //particle.position += particle.velocity;
            particle.velocity = particle.velocity.RotatedBy(MathHelper.ToRadians(3 * particle.customData[1]));

            particle.position += particle.velocity;
            particle.alpha = (float)(Math.Sin((1f - particle.customData[0] / maxTime) * Math.PI));

            //particle.scale = (1f - particle.customData[0] / maxTime) * particle.customData[2];
            particle.rotation = particle.velocity.ToRotation();

            if (particle.customData[0] > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
            float heightLerp = MathHelper.Lerp(1f, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            float widthLerp = MathHelper.Lerp(0.25f, 0, EasingUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            Color color = Color.Lerp(particle.color, Color.Red, particle.activeTime / maxTime);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha, particle.rotation + MathHelper.PiOver2, texture.Size() / 2, new Vector2(heightLerp, widthLerp), SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}