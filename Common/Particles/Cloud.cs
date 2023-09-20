using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Cloud : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;
        public float maxTime = Main.rand.Next(9, 15) * 10;

        protected int variant = Main.rand.Next(3, 7);

        public override void OnSpawn()
        {
            particle.customData[0] = particle.scale;
            //maxTime = particle.customData[1];
            particle.rotation += MathHelper.Pi / 2;
            //particle.scale = 0f;

            if (particle.customData[2] != 0) maxTime = particle.customData[2];
        }

        public override void Update()
        {
            // 0.05 == 20
            particle.velocity *= 0.95f;
            particle.rotation = particle.velocity.ToRotation() + MathHelper.PiOver2;

            float progress = ModUtils.EaseOutQuad(particle.activeTime / maxTime);
            particle.scale = MathHelper.Lerp(particle.customData[0], particle.customData[0] + 0.25f, progress);
            //particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
            particle.alpha = MathHelper.SmoothStep(1f, 0f, progress);
            //particle.scale = Utils.GetLerpValue(0f, 20f, particle.activeTime, clamped: true) * Utils.GetLerpValue(45f, 30f, particle.activeTime, clamped: true);

            if (particle.activeTime > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            // smoke_03 or smoke_06
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "smoke_0" + variant).Value;

            //float heightLerp = MathHelper.Lerp(particle.customData[0], 0, ModUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            //float widthLerp = MathHelper.Lerp(0.25f, 0, ModUtils.EaseOutQuad(Utils.Clamp(particle.activeTime, 0, maxTime) / maxTime));
            Color color = Color.Lerp(particle.color, Color.Green, particle.activeTime / maxTime);

            float rotationOffset = particle.customData[1] == 1 ? MathHelper.PiOver2 : 0;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha, particle.rotation + rotationOffset, texture.Size() / 2f, particle.scale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class VenomCloud : Cloud
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            // smoke_03 or smoke_06
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "smoke_0" + variant).Value;
            Color color = Color.Lerp(new Color(151, 79, 162), new Color(185, 128, 193), particle.activeTime / maxTime);

            float rotationOffset = particle.customData[1] == 1 ? MathHelper.PiOver2 : 0;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * particle.alpha, particle.rotation + rotationOffset, texture.Size() / 2f, particle.scale, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}