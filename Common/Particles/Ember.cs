using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class Ember : CustomParticle
{
    public override string Texture => AssetDirectory.Empty;
    public float maxTime = Main.rand.Next(4, 7) * 10;
    public override void OnSpawn()
    {
        particle.customData[1] = particle.customData[1] != 0 ? particle.customData[1] : Main.rand.NextFloat(0.2f, 0.3f);
        particle.alpha = 0f;
        particle.scale = particle.customData[1];
    }

    public override void Update()
    {
        particle.customData[0]++;
        particle.position += particle.velocity;
        particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(particle.customData[0] * Math.PI / 10);
        particle.alpha = (float)(Math.Sin((1f - particle.customData[0] / maxTime) * Math.PI));
        particle.scale = (1f - particle.customData[0] / maxTime) * particle.customData[1];

        if (particle.customData[0] > maxTime) particle.Kill();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
        spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale * 0.125f, SpriteEffects.None, 0f);
        
        spriteBatch.Reload(BlendState.Additive);

        Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
        spriteBatch.Draw(tex, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.7f, 0f, tex.Size() / 2, particle.scale * 1.5f, SpriteEffects.None, 0f);
        spriteBatch.Draw(tex, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.4f, 0f, tex.Size() / 2, particle.scale * 3f, SpriteEffects.None, 0f);

        spriteBatch.Reload(BlendState.AlphaBlend);
    }
}