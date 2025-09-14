using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace OvermorrowMod.Content.Items.Archives.Weapons
{
    public class Patronus : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.width = 58;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.timeLeft = ModUtils.SecondsToTicks(4);
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(12);

            Projectile.Opacity = 0;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.AbigailUpgrade with
            {
                Volume = 1f,
                Pitch = 0.8f,
                PitchVariance = 0.1f,
            });

            Vector2 currentCenter = Projectile.Center;
            Vector2 backwardDirection = -Vector2.Normalize(Projectile.velocity);

            for (int i = 0; i < INITIAL_TRAIL_LENGTH; i++)
            {
                // Each position is 2 pixels further back
                Vector2 trailPosition = currentCenter + (backwardDirection * i * 6f);
                trailPositions.Add(trailPosition);
                trailRotations.Add(Projectile.rotation);
            }

            TransformEffect();
        }

        private float fadeInTime = 20f; // Frames to fade in
        private float fadeOutTime = 30f; // Frames to fade out before despawn
        public ref float AICounter => ref Projectile.ai[0];

        private List<Vector2> trailPositions = new List<Vector2>();
        private List<float> trailRotations = new List<float>();
        private int MAX_TRAIL_LENGTH = 100;
        private int INITIAL_TRAIL_LENGTH = 0;
        private int DELAY = 10;
        public override void AI()
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;
            var glowParticle = new Circle(texture, ModUtils.SecondsToTicks(10), useSineFade: false)
            {
                endColor = new Color(202, 188, 255),
                doWaveMotion = false,
                fadeIn = true
            };


            Texture2D texture2 = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_08", AssetRequestMode.ImmediateLoad).Value;
            var starParticle = new Circle(texture2, ModUtils.SecondsToTicks(10), useSineFade: false)
            {
                endColor = new Color(202, 188, 255),
                doWaveMotion = false,
                fadeIn = true
            };

            if (AICounter == DELAY)
            {
                Vector2 currentCenter = Projectile.Center;
                Vector2 backwardDirection = -Vector2.Normalize(Projectile.velocity);

                for (int i = 0; i < 25; i++)
                {
                    // Each position is 2 pixels further back
                    Vector2 trailPosition = currentCenter + (backwardDirection * i * 6f);
                    trailPositions.Add(trailPosition);
                    trailRotations.Add(Projectile.rotation);
                }

                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 5;
            }

            AICounter++;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (AICounter > DELAY)
            {
                UpdateTrail();

                float fadeOpacity = GetFadeOpacity();
                Projectile.Opacity = fadeOpacity;
            }



            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                // Reset to frame 0 after reaching frame 8 (9 total frames: 0 to 8)
                if (Projectile.frame >= 9)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.timeLeft % 3 == 0 && AICounter > DELAY)
            {
                Vector2 randomOffset = Main.rand.NextVector2Circular(64f, 64f);
                float randomSpeed = Main.rand.NextFloat(0.5f, 1f);
                for (int _ = 0; _ < 3; _++)
                {
                    float scale = Main.rand.NextFloat(0.01f, 0.025f);

                    ParticleManager.CreateParticleDirect(
                        glowParticle,
                        Projectile.Center + randomOffset,
                        -Vector2.Normalize(Projectile.velocity) * randomSpeed,
                        new Color(108, 108, 224),
                        1f,
                        scale,
                        Main.rand.NextFloat(0f, MathHelper.TwoPi),
                        ParticleDrawLayer.BehindNPCs,
                        useAdditiveBlending: true
                    );
                }

                if (Main.rand.NextBool(3))
                {
                    randomOffset = Main.rand.NextVector2Circular(64f, 64f);
                    float scale2 = Main.rand.NextFloat(0.02f, 0.05f);
                    ParticleManager.CreateParticleDirect(
                        starParticle,
                        Projectile.Center + randomOffset,
                        -Vector2.Normalize(Projectile.velocity) * randomSpeed,
                        new Color(108, 108, 224),
                        1f,
                        scale2,
                        Main.rand.NextFloat(0f, MathHelper.TwoPi),
                        ParticleDrawLayer.BehindNPCs,
                        useAdditiveBlending: true
                    );
                }
            }

            SpawnStars();

            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.4f, 0.7f));
        }

        private void SpawnStars()
        {
            if (Projectile.timeLeft % 5 == 0 && AICounter > DELAY && Main.rand.NextBool(2))
            {
                Vector2 starSpawnOffset = Main.rand.NextVector2Circular(80f, 80f);
                Vector2 starVelocity = Main.rand.NextVector2Circular(2f, 2f);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center + starSpawnOffset,
                    starVelocity,
                    ModContent.ProjectileType<PatronusStar>(),
                    Projectile.damage / 2,
                    Projectile.knockBack * 0.5f,
                    Projectile.owner
                );
            }
        }

        private void TransformEffect()
        {
            Color color = new Color(202, 188, 255);
            float randomScale = Main.rand.NextFloat(5f, 10f);
            float time = ModUtils.SecondsToTicks(0.4f);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_06", AssetRequestMode.ImmediateLoad).Value;
            var numSparks = 16;
            for (int i = 0; i < numSparks; i++)
            {
                randomScale = Main.rand.NextFloat(0.1f, 0.2f);

                Vector2 direction = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);
                float randomVelocity = Main.rand.NextFloat(1, 3);

                var lightSpark = new Spark(sparkTexture, 0f, true, 0f)
                {
                    endColor = new Color(108, 108, 224)
                };
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, direction * randomVelocity * 2, color, 1f, randomScale, 0f, useAdditiveBlending: true);
            }


            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            for (int _ = 0; _ < 16; _++)
            {
                randomScale = Main.rand.NextFloat(0.04f, 0.08f);
                time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 0.9f);
                Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                var lightSpark = new Circle(texture, time, true, false)
                {
                    endColor = new Color(108, 108, 224),
                    floatUp = true,
                    doWaveMotion = false,
                    intensity = 3
                };

                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, useAdditiveBlending: true);
            }

            Texture2D circleTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 3; i++)
            {
                Vector2 RandomVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(9, 11);

                var impact = new Circle(circleTexture, ModUtils.SecondsToTicks(0.5f), canGrow: true, false)
                {
                    endColor = new Color(108, 108, 224),
                    intensity = 4
                };

                randomScale = Main.rand.NextFloat(0.25f, 0.45f);
                ParticleManager.CreateParticleDirect(impact, Projectile.Center, Vector2.Zero, new Color(108, 108, 224), 2f, 0.5f, MathHelper.Pi, useAdditiveBlending: true);
            }
        }

        private float GetFadeOpacity()
        {
            if (AICounter < DELAY) return 0f;

            float fadeOpacity = 1f;
            // Fade in at the beginning
            if (AICounter < fadeInTime + DELAY)
            {
                fadeOpacity = (AICounter - DELAY) / fadeInTime;
            }
            // Fade out at the end
            else if (Projectile.timeLeft < fadeOutTime)
            {
                fadeOpacity = Projectile.timeLeft / fadeOutTime;
            }

            return MathHelper.Clamp(fadeOpacity, 0f, 1f);
        }

        private void UpdateTrail()
        {
            // Add current position to the front
            trailPositions.Insert(0, Projectile.Center);
            trailRotations.Insert(0, Projectile.rotation);

            if (trailPositions.Count < MAX_TRAIL_LENGTH) return;

            // Once at max length, remove the oldest position
            if (trailPositions.Count > MAX_TRAIL_LENGTH)
            {
                trailPositions.RemoveAt(trailPositions.Count - 1);
                trailRotations.RemoveAt(trailRotations.Count - 1);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        private void DrawTrail(Vector2 center, Texture2D texture, Color startColor, Color endColor, int trailLength = -1, float segmentSpacing = 5f, float baseWidth = 32f)
        {
            if (trailPositions.Count < 2) return;

            int maxLength = trailLength > 0 ? Math.Min(trailLength, trailPositions.Count) : trailPositions.Count;
            var vertices = new List<VertexPositionColorTexture>();
            float globalFade = GetFadeOpacity();

            for (int i = 0; i < maxLength - 1; i++)
            {
                // Skip if position is zero (shouldn't happen with our system, but safety check)
                if (trailPositions[i] == Vector2.Zero || trailPositions[i + 1] == Vector2.Zero) continue;

                float prog1 = (float)i / (float)(maxLength - 1);
                float prog2 = (float)(i + 1) / (float)(maxLength - 1);

                // Apply offset to trail positions
                Vector2 centerOffset = center - Projectile.Center;
                Vector2 v0 = trailPositions[i] + centerOffset;
                Vector2 v1 = trailPositions[i + 1] + centerOffset;

                Vector2 normaldir = v1 - v0;
                if (normaldir.LengthSquared() > 0)
                {
                    normaldir = new Vector2(normaldir.Y, -normaldir.X);
                    normaldir.Normalize();
                }
                else
                {
                    // Fallback to using rotation from our custom trail
                    float rotation = i < trailRotations.Count ? trailRotations[i] : Projectile.rotation;
                    normaldir = new Vector2((float)Math.Cos(rotation + MathHelper.PiOver2), (float)Math.Sin(rotation + MathHelper.PiOver2));
                }

                float width1 = baseWidth;
                float width2 = baseWidth;

                Vector2 pos1Top = v0 + width1 * normaldir;
                Vector2 pos1Bottom = v0 - width1 * normaldir;
                Vector2 pos2Top = v1 + width2 * normaldir;
                Vector2 pos2Bottom = v1 - width2 * normaldir;

                // Convert to screen space
                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                float alpha = MathHelper.Lerp(1f, 0f, prog1) * globalFade;
                Color color1 = Color.Lerp(startColor, endColor, prog1) * alpha;
                Color color2 = Color.Lerp(startColor, endColor, prog2) * alpha;

                // UV mapping with easing
                float easedProg1 = EasingUtils.EaseOutQuint(prog1);
                float easedProg2 = EasingUtils.EaseOutQuint(prog2);

                float startOffset = easedProg1 * 0.5f;
                float endOffset = easedProg2 * 0.5f;

                // Define triangles to map textures to
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color1, new Vector2(startOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color2, new Vector2(endOffset, 1f)));
            }

            if (vertices.Count > 2)
            {
                DrawPrimitives(vertices, texture, OvermorrowModFile.Instance.TrailShader.Value);
            }
        }

        private void DrawPrimitives(List<VertexPositionColorTexture> vertices, Texture2D texture, Effect effect)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            if (effect.Parameters["WorldViewProjection"] != null)
                effect.Parameters["WorldViewProjection"].SetValue(model * projection);
            else if (effect.Parameters["WVP"] != null)
                effect.Parameters["WVP"].SetValue(model * projection);
            else if (effect.Parameters.Count > 0)
                effect.Parameters[0].SetValue(model * projection);

            effect.SafeSetParameter("uImage0", texture);
            effect.CurrentTechnique.Passes["Texturized"].Apply();

            Main.instance.GraphicsDevice.Textures[0] = texture;
            Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                vertices.ToArray(), 0, vertices.Count / 3);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (AICounter > DELAY)
            {
                Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_02").Value;
                Vector2 offset = new Vector2(200, 0).RotatedBy(Projectile.velocity.ToRotation());
                var spriteEffects = Projectile.direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
                //for (int _ = 0; _ < 2; _++)

                DrawTrail(Projectile.Center + offset, ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value, new Color(108, 108, 224),
                    new Color(202, 188, 255), trailLength: 80, segmentSpacing: 16f, baseWidth: 120f);

                offset = new Vector2(160, 0).RotatedBy(Projectile.velocity.ToRotation());
                for (int _ = 0; _ < 2; _++)
                    DrawTrail(Projectile.Center + offset, trailTexture, Color.White, Color.MediumPurple, trailLength: 100, segmentSpacing: 16f, baseWidth: 70f);

                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

                var textureHeight = texture.Height / 9;
                Rectangle drawRectangle = new Rectangle(0, textureHeight * Projectile.frame, texture.Width, textureHeight);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * Projectile.Opacity, Projectile.rotation, drawRectangle.Size() / 2f, Projectile.scale, spriteEffects, 0);
            }

            return false;
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
        }
    }
}