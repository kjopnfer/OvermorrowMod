using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class WandOrb : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;

            Projectile.DamageType = DamageClass.Summon;
        }

        public override bool? CanDamage() => AICounter > _homingDelay;

        private readonly int _homingDelay = 15;
        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            if (AICounter++ > _homingDelay)
            {
                NPC targetNPC = Projectile.FindNearestNPC();
                if (targetNPC != null)
                {
                    Vector2 toNPC = targetNPC.Center - Projectile.Center;
                    Vector2 directionToNPC = Vector2.Normalize(toNPC);

                    Vector2 currentDirection = Vector2.Normalize(Projectile.velocity);

                    float distanceToNPC = toNPC.Length();
                    float baseTurnRate = 0.1f;
                    float closeDistance = 200f;

                    float turnRate = MathHelper.Lerp(0.3f, baseTurnRate, Math.Min(distanceToNPC / closeDistance, 1f));
                    Vector2 newDirection = Vector2.Lerp(currentDirection, directionToNPC, turnRate);

                    float baseSpeed = 2f;
                    float timeActive = AICounter - _homingDelay;
                    float speedIncrease = timeActive * 0.05f;
                    float speed = baseSpeed + speedIncrease;
                    speed = Math.Min(speed, 12f);

                    Projectile.velocity = newDirection * speed;
                }
            }

            Color color = new(60, 255, 220);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            if (Main.rand.NextBool(2))
                for (int _ = 0; _ < 3; _++)
                {
                    float randomScale = Main.rand.NextFloat(0.015f, 0.025f);
                    var time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                    Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.2f, 0.4f);
                    Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                    var lightSpark = new Circle(texture, time, false, false)
                    {
                        endColor = new Color(0, 255, 80),
                        floatUp = true,
                        doWaveMotion = false,
                        intensity = 3
                    };

                    ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, ParticleDrawLayer.BehindProjectiles, false);
                }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.Heal(1);

            Color color = new(60, 255, 220);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            for (int _ = 0; _ < 12; _++)
            {
                float randomScale = Main.rand.NextFloat(0.015f, 0.025f);
                var time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.5f, 1.4f);
                Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                var lightSpark = new Circle(texture, time, false, false)
                {
                    endColor = new Color(0, 255, 80),
                    floatUp = true,
                    doWaveMotion = false,
                    intensity = 3
                };

                ParticleManager.CreateParticleDirect(lightSpark, target.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, useAdditiveBlending: false);
            }

            var impact = new Circle(texture, ModUtils.SecondsToTicks(0.5f), false, false)
            {
                endColor = new Color(108, 108, 224),
                intensity = 2
            };

            float randomScale2 = Main.rand.NextFloat(0.2f, 0.35f);
            ParticleManager.CreateParticleDirect(impact, target.Center, Vector2.Zero, color, 0.5f, randomScale2, MathHelper.Pi, useAdditiveBlending: true);
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D circle = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;

            Vector2 center = Projectile.Center - Main.screenPosition;
            Vector2 origin = circle.Size() / 2f;

            (float scale, float alpha, Color color)[] glowLayers = new (float, float, Color)[]
            {
               (0.65f, 0.15f, new Color(0, 255, 80)),
               (0.45f, 0.25f, new Color(20, 255, 120)),
               (0.3f,  0.35f, new Color(40, 255, 170)),
               (0.15f, 0.45f, new Color(60, 255, 220)),
               (0.05f, 0.6f,  new Color(120, 255, 255)),
               (0.08f, 0.9f,  new Color(200, 255, 255))
            };

            foreach (var (scale, alpha, color) in glowLayers)
            {
                spriteBatch.Draw(circle, center, null, color * alpha, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}