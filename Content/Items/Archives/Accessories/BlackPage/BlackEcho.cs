using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class BlackEcho : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            Projectile.width = Projectile.height = 114;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = ModUtils.SecondsToTicks(1);
            Projectile.timeLeft = ModUtils.SecondsToTicks(10);
            Projectile.DamageType = DamageClass.Summon;
        }

        private int length = 0;
        private List<Vector2> segmentPositions = new List<Vector2>();
        private Vector2 previousCenter;

        public override void OnSpawn(IEntitySource source)
        {
            previousCenter = Projectile.Center;
        }

        private float wavePhase = 0f;
        private float baseDirection = MathHelper.ToRadians(-90);

        public override void AI()
        {
            Projectile.timeLeft = 5;

            // TESTING ONLY
            Vector2 targetPosition = Main.MouseWorld + new Vector2(0, -30);
            Vector2 directionToTarget = targetPosition - Projectile.Center;

            if (directionToTarget.Length() > 5f)
            {
                directionToTarget.Normalize();
                float homingSpeed = 8f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToTarget * homingSpeed, 0.1f);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }

            Projectile.Center += Projectile.velocity;

            wavePhase += 0.08f;
            baseDirection = MathHelper.ToRadians(0);

            if (length < 20)
            {
                float newRotation = Projectile.rotation - (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f;
                rotations.Add(newRotation);
                segmentPositions.Add(Projectile.Center); // Initialize segment position
                length++;
            }

            //Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.rotation = 0;

            // Calculate movement velocity for trailing effect
            Vector2 velocity = Projectile.Center - previousCenter;
            previousCenter = Projectile.Center;

            // Update segment positions with lag based on projectile movement
            for (int i = 0; i < segmentPositions.Count; i++)
            {
                float segmentFactor = i / (float)Math.Max(1, segmentPositions.Count - 1);

                // Calculate where this segment should be based on base direction
                Vector2 baseVector = new Vector2((float)Math.Cos(baseDirection), (float)Math.Sin(baseDirection));
                Vector2 idealPosition = Projectile.Center - (baseVector * 10f * i).RotatedBy(rotations[i] - baseDirection);

                // Apply velocity influence to segment positions where segments get "pushed" by projectile movement
                Vector2 velocityInfluence = velocity * (1f - segmentFactor) * 0.8f; // Stronger influence on segments closer to base
                idealPosition -= velocityInfluence * i * 0.95f; // Segments trail behind based on velocity

                // Apply lag - segments further from center lag more
                float lagStrength = MathHelper.Lerp(1f, 0.1f, segmentFactor); // First segments follow quickly, later ones lag more
                segmentPositions[i] = Vector2.Lerp(segmentPositions[i], idealPosition, lagStrength);
            }

            // Wispy smoke-like movement for rotations
            for (int i = 0; i < rotations.Count; i++)
            {
                float segmentFactor = i / (float)Math.Max(1, rotations.Count - 1);

                // Gentle drift that gets stronger towards the tip
                float driftStrength = MathHelper.Lerp(0.05f, 0.9f, segmentFactor);

                // Multiple noise sources for wavey movement
                float timeOffset = wavePhase + i * 0.3f;
                float drift1 = (float)Math.Sin(timeOffset * 0.7f) * driftStrength;
                float drift2 = (float)Math.Cos(timeOffset * 1.2f + i * 0.1f) * driftStrength * 0.6f;
                float drift3 = (float)Math.Sin(timeOffset * 2.1f + i * 0.05f) * driftStrength * 0.3f;

                // Gentle upward drift like smoke rising
                float upwardDrift = (float)Math.Sin(timeOffset * 0.5f + segmentFactor) * 0.15f * segmentFactor;

                // Add influence from projectile movement - stronger velocity creates more drift
                float movementInfluence = velocity.Length() * 0.05f * segmentFactor;

                float totalDrift = drift1 + drift2 + drift3 + movementInfluence;

                if (i == 0)
                {
                    // First segment follows projectile position with some drift
                    //rotations[i] = Projectile.rotation + totalDrift + upwardDrift;
                    rotations[i] = Projectile.rotation;
                }
                else
                {
                    // Each segment loosely follows the previous one with gentle independence
                    float followStrength = MathHelper.Lerp(0.8f, 0.1f, segmentFactor);
                    //float targetRotation = MathHelper.Lerp(rotations[i - 1] + totalDrift + upwardDrift, rotations[i - 1], followStrength);
                    float targetRotation = MathHelper.Lerp(rotations[i - 1], rotations[i - 1], followStrength);

                    rotations[i] = MathHelper.Lerp(rotations[i], targetRotation, 1f); // Gentle interpolation
                }
            }

            base.AI();
        }

        private void DrawTentacle(Vector2 center, List<float> rotations, Texture2D texture, Effect effect = null, float segmentSpacing = 5f, float baseWidth = 32f)
        {
            if (rotations.Count < 2 || segmentPositions.Count < 2) return;

            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 0; i < rotations.Count - 1; i++)
            {
                float prog1 = (float)i / (float)(rotations.Count - 1);
                float prog2 = (float)(i + 1) / (float)(rotations.Count - 1);

                // Use the actual lagged segment positions instead of calculating from center
                Vector2 v0 = segmentPositions[i];
                Vector2 v1 = segmentPositions[i + 1];

                Vector2 normaldir = v1 - v0;
                if (normaldir.LengthSquared() > 0)
                {
                    normaldir = new Vector2(normaldir.Y, -normaldir.X);
                    normaldir.Normalize();
                }
                else
                {
                    normaldir = Vector2.UnitY; // Fallback if positions are identical
                }

                // tapering - starts thicker, gets very thin at tip
                float width1 = baseWidth * MathHelper.Lerp(1f, 0.05f, EasingUtils.EaseInOutBounce(prog1));
                float width2 = baseWidth * MathHelper.Lerp(1f, 0.05f, EasingUtils.EaseInOutBounce(prog2));

                Vector2 pos1Top = v0 + width1 * normaldir;
                Vector2 pos1Bottom = v0 - width1 * normaldir;
                Vector2 pos2Top = v1 + width2 * normaldir;
                Vector2 pos2Bottom = v1 - width2 * normaldir;

                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                float alpha = MathHelper.Lerp(2f, 0f, MathHelper.Clamp(EasingUtils.EaseInQuad(prog1) + 0.4f, 0, 1f));
                Color color1 = Color.Lerp(Color.Red, Color.Black, EasingUtils.EaseOutQuint(prog1)) * alpha;
                Color color2 = Color.Lerp(Color.Red, Color.Black, EasingUtils.EaseOutQuint(prog2)) * alpha;

                float segmentSize = 1.0f / (float)(rotations.Count - 1);
                float startOffset = segmentSize * i;
                float endOffset = segmentSize * (i + 1);

                // Defne triangles to map textures to
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

        private List<float> rotations = new List<float>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            float alpha = 1f;
            Rectangle drawRectangle = new Rectangle(0, 0, texture.Width, texture.Height / 3);

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * alpha, Projectile.rotation, drawRectangle.Size() / 2f, 1f, spriteEffects, 1);

            Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Jagged").Value;
            DrawTentacle(Projectile.Center + new Vector2(6, 0), rotations, trailTexture, null, segmentSpacing: 10f, baseWidth: 24f);
            return false;
        }
    }
}