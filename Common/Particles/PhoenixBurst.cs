using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class PhoenixBurst : CustomParticle
    {
        public float maxTime;

        public override string Texture => AssetDirectory.Empty;
        public override void OnSpawn()
        {
            maxTime = 120;
            base.OnSpawn();
        }

        public int AICounter = 0;
        public override void Update()
        {
            AICounter++;

            particle.scale = MathHelper.Lerp(0.5f, 0, (1 - particle.activeTime) / maxTime);
            particle.rotation += 0.08f;

            if (particle.activeTime > maxTime) particle.Kill();

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            float alpha = MathHelper.Lerp(0.5f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            float scale = MathHelper.Lerp(0f, 6f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.Red * alpha, particle.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            spriteBatch.Reload(BlendState.Additive);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_03").Value;
            scale = MathHelper.Lerp(0f, 2f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.OrangeRed * alpha, particle.rotation, texture.Size() / 2f, scale + 0.5f, SpriteEffects.None, 1);


            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            alpha = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            scale = MathHelper.Lerp(0f, 0.75f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * alpha, (particle.rotation * 2 + MathHelper.ToRadians(10 * i)) * -1, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            scale = MathHelper.Lerp(0f, 1.25f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            float rotationSpeed = MathHelper.Lerp(3, 2, Utils.Clamp(AICounter - 10, 0f, 60f) / 60f);
            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * alpha, particle.rotation * rotationSpeed + MathHelper.ToRadians(25 * i) + MathHelper.ToRadians(240), texture.Size() / 2f, scale, SpriteEffects.FlipHorizontally, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            scale = MathHelper.Lerp(0f, 1.75f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            rotationSpeed = MathHelper.Lerp(4, 2, Utils.Clamp(AICounter - 20, 0f, 60f) / 60f);
            for (int i = 0; i < 8; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / 8f);
                spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, color * alpha, (particle.rotation * rotationSpeed + MathHelper.ToRadians(5 * i) + MathHelper.ToRadians(240)) * -1, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.Orange * alpha, particle.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}