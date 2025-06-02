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
    public class Circle : CustomParticle
    {
        public override string Texture => AssetDirectory.Empty;

        // Convert customData to proper fields
        private float timeAlive = 0f;
        private float maxTime;
        private float initialScale;
        private float flameOffset;

        public Circle(float maxTimeOverride = 0f, float scaleOverride = 0f)
        {
            maxTime = maxTimeOverride > 0 ? maxTimeOverride : Main.rand.Next(4, 5) * 10;
            initialScale = scaleOverride > 0 ? scaleOverride : Main.rand.NextFloat(0.2f, 0.3f);
            flameOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
        }

        public override void OnSpawn()
        {
            particle.alpha = 0f;
            particle.scale = initialScale;
        }

        public override void Update()
        {
            Lighting.AddLight(particle.position, particle.color.ToVector3() * 0.2f * (1f - timeAlive / maxTime));

            timeAlive++;
            particle.position += particle.velocity;
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * flameOffset;

            float lifeProgress = 1f - timeAlive / maxTime;
            particle.alpha = lifeProgress;
            particle.scale = lifeProgress * initialScale;

            if (timeAlive > maxTime) particle.Kill();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_" + ProjectileID.StardustTowerMark).Value;
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha, 0f, texture.Size() / 2, particle.scale * 0.125f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.Additive);

            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.7f, 0f, texture.Size() / 2, particle.scale * 1.5f, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, particle.position - Main.screenPosition, null, particle.color * particle.alpha * 0.4f, 0f, texture.Size() / 2, particle.scale * 3f, SpriteEffects.None, 0f);

            spriteBatch.Reload(BlendState.AlphaBlend);
        }
    }
}

// Usage example - how to spawn the particle:
// if (Main.rand.NextBool(3))
// {
//     Vector2 offset = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
//     var lightOrb = new LightOrb(0f, scale * 0.5f); // maxTime = random, initialScale = scale * 0.5f
//     OvermorrowMod.Core.ParticleManager.CreateParticleDirect(lightOrb, npc.Bottom + offset, -Vector2.UnitY, Color.White, 1f, scale, 0f);
// }