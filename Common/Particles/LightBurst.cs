using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

public class LightBurst : Particle
{
    public float maxSize;
    public float maxTime; 
    public override void OnSpawn()
    {
        maxTime = CustomData[0] == 0 ? 60 : CustomData[0];
        maxSize = Scale;
        Scale = 0f;
    }

    public override void Update()
    {
        Velocity = Vector2.Zero;
        //float progress = ActiveTime / maxTime;

        float progress = ModUtils.EaseOutQuad(ActiveTime / maxTime);
        Scale = MathHelper.SmoothStep(Scale, maxSize, progress);
        Alpha = MathHelper.SmoothStep(Alpha, 0, ActiveTime / maxTime);
        Rotation += 0.009f;

        if (ActiveTime > maxTime) Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Reload(BlendState.Additive);

        Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "LightBurst").Value;
        Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, Rotation, origin, Scale, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}