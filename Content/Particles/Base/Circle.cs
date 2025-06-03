using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
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
        private bool useSineFade;

        public Circle(float maxTimeOverride = 0f, float scaleOverride = 0f, bool useSineFade = true)
        {
            maxTime = maxTimeOverride > 0 ? maxTimeOverride : Main.rand.Next(3, 4) * 10;
            initialScale = scaleOverride > 0 ? scaleOverride : Main.rand.NextFloat(0.2f, 0.3f);
            flameOffset = Main.rand.NextFloat(0.1f, 0.2f) * (Main.rand.NextBool() ? 1 : -1);
            this.useSineFade = useSineFade;
        }

        public override void OnSpawn()
        {
            particle.alpha = 0f;
            particle.scale = initialScale;
        }

        public override void Update()
        {
            Lighting.AddLight(particle.position, particle.color.ToVector3() / 255f);

            timeAlive++;
            particle.position += particle.velocity;
            particle.position += particle.velocity.RotatedBy(Math.PI / 2) * (float)Math.Sin(timeAlive * Math.PI / 10) * flameOffset;

            float lifeProgress = 1f - timeAlive / maxTime;
            particle.alpha = useSineFade ? (float)(Math.Sin(lifeProgress * Math.PI)) : lifeProgress;
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