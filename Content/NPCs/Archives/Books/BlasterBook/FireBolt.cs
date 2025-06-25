using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common;
using System.Collections.Generic;
using Terraria.ModLoader;
using OvermorrowMod.Common.Utilities;
using Terraria;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Particles;
using System;
using OvermorrowMod.Core.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using System.Diagnostics.Metrics;
using ReLogic.Content;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class FireBolt : ModProjectile, ITrailEntity, IDrawAdditive
    {
        public IEnumerable<TrailConfig> TrailConfigurations()
        {
            return new List<TrailConfig>
            {
                new TrailConfig(
                    typeof(LaserTrail),
                    progress => Color.Lerp(Color.White, Color.White, progress) * MathHelper.SmoothStep(0, 1, progress),
                    progress => 30
                ),
                new TrailConfig(
                    typeof(FireTrail),
                    progress => DrawUtils.ColorLerp3(Color.Purple, Color.MediumOrchid, Color.Cyan, progress) *  MathHelper.SmoothStep(0, 1, progress),
                    progress => 40
                )
            };
        }

        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            float frequency = 0.1f; // Controls how quickly the sine wave oscillates
            float amplitude = 0.5f;   // Controls the amplitude of the sinusoidal rotation

            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0.25f);

            // Compute the sine-based offset
            float sineOffset = (float)Math.Sin(Projectile.ai[0]++ * frequency) * amplitude;

            // Rotate velocity by the sine offset
            Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(sineOffset));

            int randomIterations = Main.rand.Next(2, 5);
            Vector2 drawOffset = new Vector2(-4, -4).RotatedBy(Projectile.rotation);
            float particleScale = 0.1f;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < randomIterations; i++)
            {
                var lightSpark = new Spark(texture, Main.rand.Next(2, 3) * 10, false);

                var emberParticle = new Circle(texture, ModUtils.SecondsToTicks(0.7f), useSineFade: true);
                emberParticle.endColor = Color.Purple;
                ParticleManager.CreateParticleDirect(emberParticle, Projectile.Center, -Projectile.velocity * 0.1f, Color.Cyan * 0.1f, 0.5f, particleScale, 0f, useAdditiveBlending: true);
            }

            base.AI();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05", AssetRequestMode.ImmediateLoad).Value;

            Color color = Color.Cyan;
            var lightOrb = new Circle(texture, ModUtils.SecondsToTicks(0.4f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;

            float orbScale = 0.5f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, Color.White, 1f, orbScale, 0.2f, useAdditiveBlending: true);

            lightOrb = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_01", AssetRequestMode.ImmediateLoad).Value, ModUtils.SecondsToTicks(0.3f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, scale: 0.3f, 0.2f, useAdditiveBlending: true);


            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            return base.PreDraw(ref lightColor);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Color color = Color.Cyan;
            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_01").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;

            spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.5f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(0.7f, 0.25f), SpriteEffects.None, 1);
            spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, color * 0.3f, Projectile.velocity.ToRotation(), tex2.Size() / 2f, new Vector2(1f, 0.25f), SpriteEffects.None, 1);

            spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, MathHelper.PiOver2, circle.Size() / 2f, 0.2f, SpriteEffects.None, 1);
            spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, color * 0.4f, MathHelper.PiOver2, circle.Size() / 2f, 0.5f, SpriteEffects.None, 1);
        }
    }
}