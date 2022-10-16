using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

public class LightBurst : CustomParticle
{
    public override string Texture => AssetDirectory.Textures + "PulseCircle";
    public float maxSize;
    public float maxTime; 
    public override void OnSpawn()
    {
        maxTime = particle.customData[0] == 0 ? 60 : particle.customData[0];
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
        particle.rotation += 0.009f;

        if (particle.activeTime > maxTime) particle.Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Reload(BlendState.Additive);

        Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "LightBurst").Value;
        Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
        spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, particle.rotation, origin, particle.scale, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}