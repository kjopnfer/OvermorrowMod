using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class HammerSwing : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveItems + "ArcanistsHammer";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 54;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = ModUtils.SecondsToTicks(5f);
            Projectile.DamageType = DamageClass.Melee;
        }

        public override bool? CanDamage() => canDamage;

        public override void OnSpawn(IEntitySource source)
        {
            Player owner = Main.player[Projectile.owner];
            SpinDirection = owner.direction;
        }

        public ref float AICounter => ref Projectile.ai[0];
        public ref float SpinDirection => ref Projectile.ai[1];

        bool canDamage = true;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active)
                Projectile.Kill();

            Projectile.damage = player.statDefense;
            Projectile.timeLeft = 5;

            const float fadeInTime = 30f;
            const float rotationTime = 60f;
            const float decelerationTime = 60f;


            AICounter++;
            int yOffset = -120;
            canDamage = false;
            if (AICounter <= fadeInTime)
            {
                Projectile.Opacity = AICounter / fadeInTime;
                Vector2 offset = new Vector2(0, yOffset);
                Projectile.Center = player.Center + offset;
                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation();

                UpdateTrailPositions();
                return;
            }

            float adjustedCounter = AICounter - fadeInTime;
            float rotation;

            if (adjustedCounter < rotationTime)
            {
                if (adjustedCounter == 25)
                {
                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with
                    {
                        Pitch = -1f
                    });
                }

                // Normal spin phase
                float baseRotation = MathHelper.Lerp(0, 180, EasingUtils.EaseInBack(adjustedCounter / rotationTime));
                rotation = baseRotation * SpinDirection;
                Projectile.Opacity = 1f;

                canDamage = true;
            }
            else
            {
                // Deceleration phase
                float decelerationProgress = Math.Min((adjustedCounter - rotationTime) / decelerationTime, 1f);

                float extendedProgress = EasingUtils.EaseOutQuart(decelerationProgress);
                float baseRotation = MathHelper.Lerp(180, 540, extendedProgress);
                rotation = baseRotation * SpinDirection;

                Projectile.Opacity = 1f - decelerationProgress;
                if (decelerationProgress > 0.4f)
                    canDamage = false;

                if (decelerationProgress >= 1f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Vector2 positionOffset = new Vector2(0, yOffset).RotatedBy(MathHelper.ToRadians(rotation));
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact with
            {
                MaxInstances = 0,
                Pitch = 1f
            });

            var hitboxHeight = 140;
            Vector2 swordStart = Projectile.Center;
            Vector2 swordEnd = Projectile.Bottom + new Vector2(0, -hitboxHeight).RotatedBy(Projectile.rotation);

            Vector2 hitPoint = Vector2.Lerp(swordStart, swordEnd,
                MathHelper.Clamp(Vector2.Dot(target.Center - swordStart, swordEnd - swordStart) /
                Vector2.DistanceSquared(swordStart, swordEnd), 0f, 1f));

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D circleTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;

            Color color = new Color(60, 71, 255);
            for (int i = 0; i < 12; i++)
            {
                float randomScale = Main.rand.NextFloat(0.15f, 0.3f);
                Vector2 randomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(6, 8);

                var lightSpark = new Spark(sparkTexture, 0f, true, 0f)
                {
                    endColor = new Color(108, 108, 224),
                    rotationOffset = MathHelper.PiOver2
                };
                ParticleManager.CreateParticleDirect(lightSpark, hitPoint, randomVelocity, color, 1f, randomScale, MathHelper.Pi, useAdditiveBlending: true);
            }

            var impact = new Circle(circleTexture, ModUtils.SecondsToTicks(0.5f), false, false)
            {
                endColor = new Color(108, 108, 224),
                intensity = 3
            };

            float randomScale2 = Main.rand.NextFloat(0.45f, 0.65f);
            ParticleManager.CreateParticleDirect(impact, target.Center, Vector2.Zero, color, 0.5f, randomScale2, MathHelper.Pi, useAdditiveBlending: true);

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage += target.defense;
        }

        public override void OnKill(int timeLeft)
        {
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

        private void DrawTrail(Vector2 center, Texture2D texture, Effect effect = null, float segmentSpacing = 5f, float baseWidth = 32f)
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
                Color color1 = Color.Lerp(new Color(31, 44, 255), Color.Black, EasingUtils.EaseOutQuint(prog1)) * alpha;
                Color color2 = Color.Lerp(new Color(31, 44, 255), Color.Black, EasingUtils.EaseOutQuint(prog2)) * alpha;

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
            Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Smoke").Value;

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
                DrawTrail(Projectile.Center, trailTexture, null, segmentSpacing: 1f, baseWidth: 48f);
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 offset = new Vector2(0, -22).RotatedBy(Projectile.rotation);
            Vector2 origin = new(texture.Width / 2, 0);
            Main.spriteBatch.Draw(texture, Projectile.Center + offset - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation - MathHelper.PiOver4, origin, 1f, SpriteEffects.FlipHorizontally, 1);

            return false;
        }
    }
}