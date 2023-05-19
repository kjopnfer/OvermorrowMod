using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Particles
{
    public class Orb : CustomParticle
    {
        // customData[0] = maxTime

        public override string Texture => AssetDirectory.Empty;

        float maxTime = 120;
        public override void OnSpawn()
        {
            particle.rotation = MathHelper.ToRadians(Main.rand.Next(0, 90));
            maxTime = particle.customData[0] != 0 ? particle.customData[0] : 60;
        }

        public override void Update()
        {
            if (particle.activeTime > maxTime) particle.Kill();

            float progress = (float)(particle.activeTime) / maxTime;
            particle.alpha = MathHelper.SmoothStep(1f, 0f, Utils.Clamp(particle.activeTime - (maxTime - 45), 0, 45) / 45f);
            particle.rotation += 0.06f;
            particle.velocity *= 0.98f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(203, 243, 119) * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.05f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.025f, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(80, 130, 55) * particle.alpha * 0.85f, particle.rotation, texture.Size() / 2, particle.scale * 0.65f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class PoisonOrb : Orb
    {  
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(203, 243, 119) * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.05f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.025f, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(80, 130, 55) * particle.alpha * 0.85f, particle.rotation, texture.Size() / 2, particle.scale * 0.65f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }

    public class VenomOrb : Orb
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(151, 79, 162) * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.05f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, Color.White * particle.alpha, particle.rotation, texture.Size() / 2, particle.scale * 0.025f, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, new Color(185, 128, 193) * particle.alpha * 0.85f, particle.rotation, texture.Size() / 2, particle.scale * 0.65f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}