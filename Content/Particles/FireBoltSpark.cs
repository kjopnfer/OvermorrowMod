using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Particles
{
    /// <summary>
    /// This is literally LightOrb but I change the time to be shorter.
    /// Because inheritance doesn't work in these for some fucking reason.
    /// </summary>
    public class FireBoltSpark : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;
        public float maxTime = Main.rand.Next(2, 3) * 10;
        public float flameOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
        public override void OnSpawn()
        {
            particle.customData[1] = particle.customData[1] != 0 ? particle.customData[1] : Main.rand.NextFloat(0.2f, 0.3f);
            particle.alpha = 0f;
            particle.scale = particle.customData[1];
        }

        public override void Update()
        {
            Lighting.AddLight(particle.position, particle.color.ToVector3() * 0.2f * (1f - particle.customData[0] / maxTime));
            particle.customData[0]++;
            particle.position += particle.velocity;
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(particle.customData[0] * Math.PI / 10) * flameOffset;
            particle.alpha = (1f - particle.customData[0] / maxTime);
            particle.scale = (1f - particle.customData[0] / maxTime) * particle.customData[1];

            if (particle.customData[0] > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale * 0.125f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.Additive);

            //Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.7f, 0f, texture.Size() / 2, particle.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.4f, 0f, texture.Size() / 2, particle.scale * 3f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}
