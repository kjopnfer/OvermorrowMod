using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class Flash : Particle
{
    public float maxTime = Main.rand.Next(8, 14) * 10;
    public override void OnSpawn()
    {
        CustomData[0] = Scale;
        Scale = 0f;
    }

    public override void Update()
    {
        Velocity *= 0.95f;
        Alpha = MathHelper.SmoothStep(Alpha, 0, ActiveTime / maxTime);
        Scale = MathHelper.SmoothStep(CustomData[0], 0, ActiveTime / maxTime);

        if (ActiveTime > maxTime) Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, 0f, texture.Size() / 2, Scale * 0.2f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.Additive);
        texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.7f, 0f, texture.Size() / 2, Scale * 2.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.4f, 0f, texture.Size() / 2, Scale * 5f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}

public class Bubble : Flash
{
    public new float maxTime = Main.rand.Next(20, 26) * 10;

    public override void Update()
    {
        Velocity *= 0.95f;
        Alpha = MathHelper.SmoothStep(Alpha, 0, ActiveTime / maxTime);
        Scale = MathHelper.SmoothStep(CustomData[0], 0, ActiveTime / maxTime);

        if (ActiveTime > maxTime) Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Stops the additive blendstate from merging them all together into a giant explosion at first
        if (ActiveTime > 180)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, 0f, texture.Size() / 2, Scale * 0.2f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.Additive);
            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.7f, 0f, texture.Size() / 2, Scale * 2.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha * 0.4f, 0f, texture.Size() / 2, Scale * 5f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}