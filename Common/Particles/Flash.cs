using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class Flash : CustomParticle
{
    public override string Texture => AssetDirectory.Empty;
    public float maxTime = Main.rand.Next(8, 14) * 10;
    public override void OnSpawn()
    {
        particle.customData[0] = particle.scale;
        particle.scale = 0f;
    }

    public override void Update()
    {
        particle.velocity *= 0.95f;
        particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
        particle.scale = MathHelper.SmoothStep(particle.customData[0], 0, particle.activeTime / maxTime);

        if (particle.activeTime > maxTime) particle.Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
        spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale * 0.2f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.Additive);
        texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

        spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.7f, 0f, texture.Size() / 2, particle.scale * 2.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.4f, 0f, texture.Size() / 2, particle.scale * 5f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}

public class Bubble : Flash
{
    public new float maxTime = Main.rand.Next(20, 26) * 10;

    public override void Update()
    {
        particle.velocity *= 0.95f;
        particle.alpha = MathHelper.SmoothStep(particle.alpha, 0, particle.activeTime / maxTime);
        particle.scale = MathHelper.SmoothStep(particle.customData[0], 0, particle.activeTime / maxTime);

        if (particle.activeTime > maxTime) particle.Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Stops the additive blendstate from merging them all together into a giant explosion at first
        if (particle.activeTime > 180)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale * 0.2f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.Additive);
            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.7f, 0f, texture.Size() / 2, particle.scale * 2.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.4f, 0f, texture.Size() / 2, particle.scale * 5f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}