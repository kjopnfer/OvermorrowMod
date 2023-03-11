using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class Ember : Particle
{
    public float maxTime = Main.rand.Next(4, 7) * 10;
    public override void OnSpawn()
    {
        CustomData[1] = Main.rand.NextFloat(0.2f, 0.3f);
        Alpha = 0f;
        Scale = CustomData[1];
    }

    public override void Update()
    {
        CustomData[0]++;
        Position += Velocity;
        Position += Velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(CustomData[0] * Math.PI / 10);
        Alpha = (float)(Math.Sin((1f - CustomData[0] / maxTime) * Math.PI));
        Scale = (1f - CustomData[0] / maxTime) * CustomData[1];

        if (CustomData[0] > maxTime) Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
        spriteBatch.Draw(texture, Position - Main.screenPosition, null, Color * Alpha, 0f, texture.Size() / 2, Scale * 0.125f, SpriteEffects.None, 0f);
        
        spriteBatch.Reload(BlendState.Additive);

        Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
        spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * Alpha * 0.7f, 0f, tex.Size() / 2, Scale * 1.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(tex, Position - Main.screenPosition, null, Color * Alpha * 0.4f, 0f, tex.Size() / 2, Scale * 3f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}