using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class TaintedAthame : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5f);
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player owner = Main.player[Projectile.owner];
            SpinDirection = owner.direction;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float SpinDirection => ref Projectile.ai[1];
        public override void AI()
        {
            Projectile.damage = 50;
            Projectile.CritChance = 100;

            Player player = Main.player[Projectile.owner];
            if (!player.active)
                Projectile.Kill();

            Projectile.timeLeft = 5;

            const float fadeInTime = 30f;
            const float rotationTime = 60f;
            const float decelerationTime = 60f;

            AICounter++;

            // Fade in phase (0-30 ticks)
            if (AICounter <= fadeInTime)
            {
                Projectile.Opacity = AICounter / fadeInTime;
                // Stay at starting position during fade in
                Vector2 offset = new Vector2(0, -80);
                Projectile.Center = player.Center + offset;
                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();

                UpdateTrailPositions();
                return; // Exit early, don't do rotation logic yet
            }

            // Adjust counter for the main logic (subtract fade in time)
            float adjustedCounter = AICounter - fadeInTime;

            float rotation;

            if (adjustedCounter < rotationTime)
            {
                // Normal spin phase
                float baseRotation = MathHelper.Lerp(0, 360, EasingUtils.EaseInExpo(adjustedCounter / rotationTime));
                rotation = baseRotation * SpinDirection;
                Projectile.Opacity = 1f;
            }
            else
            {
                // Deceleration phase
                float decelerationProgress = Math.Min((adjustedCounter - rotationTime) / decelerationTime, 1f);

                float extendedProgress = EasingUtils.EaseOutQuart(decelerationProgress);
                float baseRotation = MathHelper.Lerp(360, 720, extendedProgress);
                rotation = baseRotation * SpinDirection;

                Projectile.Opacity = 1f - decelerationProgress;

                if (decelerationProgress >= 1f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Vector2 positionOffset = new Vector2(0, -100).RotatedBy(MathHelper.ToRadians(rotation));
            Projectile.Center = player.Center + positionOffset;
            Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();

            // Only spawn initial particles after fade in is complete
            if (adjustedCounter == 0)
            {
                Color color = new Color(137, 15, 78);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
                for (int _ = 0; _ < 16; _++)
                {
                    float randomScale = Main.rand.NextFloat(0.025f, 0.045f);
                    var time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                    Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 0.9f);
                    Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                    var lightSpark = new Circle(texture, time, true, false)
                    {
                        endColor = Color.Black,
                        floatUp = true,
                        doWaveMotion = false,
                        intensity = 3
                    };

                    ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, useAdditiveBlending: false);
                }
            }

            UpdateTrailPositions();
        }

        private HashSet<int> UniqueHits = new HashSet<int>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            UniqueHits.Add(target.whoAmI);

            Color color = new Color(137, 15, 78);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            for (int _ = 0; _ < 16; _++)
            {
                float randomScale = Main.rand.NextFloat(0.025f, 0.045f);
                var time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 0.9f);
                Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                var lightSpark = new Circle(texture, time, true, false)
                {
                    endColor = Color.Black,
                    floatUp = true,
                    doWaveMotion = false,
                    intensity = 3
                };

                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, useAdditiveBlending: false);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (UniqueHits.Count < 2)
            {
                Player player = Main.player[Projectile.owner];
                Color color = new(137, 15, 78);

                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
                for (int _ = 0; _ < 16; _++)
                {
                    float randomScale = Main.rand.NextFloat(0.025f, 0.045f);
                    var time = ModUtils.SecondsToTicks(Main.rand.NextFloat(0.5f, 1.2f));
                    Vector2 randomVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0.7f, 0.9f);
                    Vector2 randomOffset = Main.rand.NextVector2Circular(16f, 16f);
                    var lightSpark = new Circle(texture, time, true, false)
                    {
                        endColor = Color.Black,
                        floatUp = true,
                        doWaveMotion = false,
                        intensity = 3
                    };

                    ParticleManager.CreateParticleDirect(lightSpark, player.Center + randomOffset, randomVelocity, color, 1f, randomScale, 0f, useAdditiveBlending: false);
                }

                player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, Projectile.whoAmI), 30, Projectile.direction);
            }
        }

        private List<Vector2> Positions = new List<Vector2>();
        private readonly int MAX_TRAIL_POSITIONS = 25;

        private void UpdateTrailPositions()
        {
            // Add current position if far enough from last position or if list is empty
            Positions.Insert(0, Projectile.Center);

            if (Positions.Count > MAX_TRAIL_POSITIONS)
            {
                Positions.RemoveAt(Positions.Count - 1);
            }
        }

        private void DrawEyeTrail(Vector2 center, Texture2D texture, Effect effect = null, float segmentSpacing = 5f, float baseWidth = 32f)
        {
            if (Positions.Count < 2) return;

            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 0; i < Positions.Count - 1; i++)
            {
                float prog1 = (float)i / (float)Positions.Count;
                float prog2 = (float)(i + 1) / (float)Positions.Count;

                Vector2 pos1 = Positions[i];
                Vector2 pos2 = Positions[i + 1];

                if (pos1 == pos2) continue;

                Vector2 off1 = PrimitiveHelper.GetRotation(Positions, i) * baseWidth * MathHelper.Lerp(1f, 0f, EasingUtils.EaseInOutBounce(prog1));
                Vector2 off2 = PrimitiveHelper.GetRotation(Positions, i + 1) * baseWidth * MathHelper.Lerp(1f, 0f, EasingUtils.EaseInOutBounce(prog2));

                Vector2 pos1Top = pos1 + off1;
                Vector2 pos1Bottom = pos1 - off1;
                Vector2 pos2Top = pos2 + off2;
                Vector2 pos2Bottom = pos2 - off2;

                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                float alpha = MathHelper.Lerp(2f, 0f, MathHelper.Clamp(EasingUtils.EaseInQuad(prog1), 0, 1f)) * Projectile.Opacity;
                Color color1 = Color.Lerp(new Color(137, 15, 78), Color.Black, EasingUtils.EaseOutQuint(prog1)) * alpha;
                Color color2 = Color.Lerp(new Color(137, 15, 78), Color.Black, EasingUtils.EaseOutQuint(prog2)) * alpha;

                float segmentSize = segmentSpacing / (float)(Positions.Count - 1);
                float startOffset = segmentSize * i;
                float endOffset = segmentSize * (i + 1);

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color1, new Vector2(startOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(endOffset, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(startOffset, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color2, new Vector2(endOffset, 1f)));
            }

            if (vertices.Count > 2)
            {
                DrawPrimitives(vertices, texture, effect ?? OvermorrowModFile.Instance.TrailShader.Value);
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
            Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Jagged").Value;

            const float fadeInTime = 30f;
            const float rotationTime = 60f;
            const float decelerationTime = 60f;

            bool shouldDrawTrail = false;

            // Only draw trail after fade in is complete
            if (AICounter > fadeInTime)
            {
                float adjustedCounter = AICounter - fadeInTime;

                if (adjustedCounter < rotationTime)
                {
                    // Only draw after initial acceleration
                    float spinProgress = adjustedCounter / rotationTime;
                    shouldDrawTrail = spinProgress > 0.2f;
                }
                else
                {
                    // Only draw before final slowdow
                    float decelerationProgress = (adjustedCounter - rotationTime) / decelerationTime;
                    shouldDrawTrail = decelerationProgress < 0.8f;
                }
            }

            if (shouldDrawTrail)
            {
                DrawEyeTrail(Projectile.Center, trailTexture, null, segmentSpacing: 1f, baseWidth: 48f);
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 1);

            return false;
        }
    }
}