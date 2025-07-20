using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.AdventurersGuild.Accessories
{
    public class ScabbardStrike : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
        }

        private struct SlashData
        {
            public Vector2 StartPoint;
            public Vector2 EndPoint;
            public float CurrentLength;
            public float MaxLength;
            public float Alpha;
            public float Width;
        }

        private SlashData slash1;
        private SlashData slash2;

        public override void OnSpawn(IEntitySource source)
        {
            InitializeSlashes();

            if (Main.myPlayer == Projectile.owner)
            {
                Main.player[Projectile.owner].velocity *= 0.1f;
            }
        }

        private void InitializeSlashes()
        {
            float slashLength = 180f;
            Vector2 center = Projectile.Center;

            Vector2 diagonal1 = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(45));
            slash1 = new SlashData
            {
                StartPoint = center - diagonal1 * slashLength * 0.5f,
                EndPoint = center + diagonal1 * slashLength * 0.5f,
                CurrentLength = 0f,
                MaxLength = slashLength,
                Alpha = 1f,
                Width = 1f
            };

            Vector2 diagonal2 = Vector2.UnitX.RotatedBy(MathHelper.ToRadians(135));
            slash2 = new SlashData
            {
                StartPoint = center - diagonal2 * slashLength * 0.5f,
                EndPoint = center + diagonal2 * slashLength * 0.5f,
                CurrentLength = 0f,
                MaxLength = slashLength,
                Alpha = 1f,
                Width = 1f
            };
        }

        public override void AI()
        {
            UpdateSlashes();
        }

        private void UpdateSlashes()
        {
            float progress = 1f - (float)Projectile.timeLeft / 40f;

            float rotationAmount = MathHelper.ToRadians(5f);
            float currentRotation = rotationAmount * progress;

            if (progress < 0.3f)
            {
                float growthProgress = progress / 0.3f;
                float easedProgress = 1f - (1f - growthProgress) * (1f - growthProgress);

                var tempSlash1 = slash1;
                var tempSlash2 = slash2;

                tempSlash1.CurrentLength = MathHelper.Lerp(0f, tempSlash1.MaxLength * 1.2f, easedProgress);
                tempSlash1.Alpha = 1f;
                tempSlash1.Width = MathHelper.Lerp(0.8f, 1.3f, easedProgress);

                tempSlash2.CurrentLength = MathHelper.Lerp(0f, tempSlash2.MaxLength * 1.2f, easedProgress);
                tempSlash2.Alpha = 1f;
                tempSlash2.Width = MathHelper.Lerp(0.8f, 1.3f, easedProgress);

                ApplyRotationToSlash(ref tempSlash1, currentRotation);
                ApplyRotationToSlash(ref tempSlash2, currentRotation);

                slash1 = tempSlash1;
                slash2 = tempSlash2;
            }
            else
            {
                float fadeProgress = (progress - 0.3f) / 0.7f;

                var tempSlash1 = slash1;
                var tempSlash2 = slash2;

                tempSlash1.CurrentLength = MathHelper.Lerp(tempSlash1.MaxLength * 1.2f, tempSlash1.MaxLength * 0.9f, fadeProgress);
                tempSlash1.Alpha = (1f - fadeProgress) * (1f - fadeProgress);
                tempSlash1.Width = MathHelper.Lerp(1.3f, 0.6f, fadeProgress);

                tempSlash2.CurrentLength = MathHelper.Lerp(tempSlash2.MaxLength * 1.2f, tempSlash2.MaxLength * 0.9f, fadeProgress);
                tempSlash2.Alpha = (1f - fadeProgress) * (1f - fadeProgress);
                tempSlash2.Width = MathHelper.Lerp(1.3f, 0.6f, fadeProgress);

                ApplyRotationToSlash(ref tempSlash1, currentRotation);
                ApplyRotationToSlash(ref tempSlash2, currentRotation);

                slash1 = tempSlash1;
                slash2 = tempSlash2;
            }
        }

        private void ApplyRotationToSlash(ref SlashData slash, float rotation)
        {
            Vector2 center = Projectile.Center;

            Vector2 originalStart = slash.StartPoint - center;
            Vector2 originalEnd = slash.EndPoint - center;

            Vector2 rotatedStart = originalStart.RotatedBy(rotation);
            Vector2 rotatedEnd = originalEnd.RotatedBy(rotation);

            slash.StartPoint = center + rotatedStart;
            slash.EndPoint = center + rotatedEnd;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var allVertices = new List<VertexPositionColorTexture>();

            // Collect all vertices from all layers of both slashes
            CollectSlashVertices(slash1, Color.White * 0.6f, Color.Cyan * 0.6f, 1.5f, allVertices);
            CollectSlashVertices(slash1, Color.White, Color.LightCyan, 1.0f, allVertices);
            CollectSlashVertices(slash1, Color.White * 1.5f, Color.White * 1.2f, 0.4f, allVertices);

            CollectSlashVertices(slash2, Color.White * 0.6f, Color.Cyan * 0.6f, 1.5f, allVertices);
            CollectSlashVertices(slash2, Color.White, Color.LightCyan, 1.0f, allVertices);
            CollectSlashVertices(slash2, Color.White * 1.5f, Color.White * 1.2f, 0.4f, allVertices);

            // Draw all vertices at once
            if (allVertices.Count > 0)
            {
                DrawPrimitives(allVertices, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Laser").Value);
            }

            return false;
        }

        private void CollectSlashVertices(SlashData slash, Color startColor, Color endColor, float widthMultiplier, List<VertexPositionColorTexture> vertices)
        {
            if (slash.CurrentLength <= 0) return;

            Vector2 direction = Vector2.Normalize(slash.EndPoint - slash.StartPoint);
            Vector2 currentEnd = slash.StartPoint + direction * slash.CurrentLength;

            GenerateSlashVertices(slash.StartPoint, currentEnd, startColor * slash.Alpha, endColor * slash.Alpha, slash.Width * widthMultiplier, vertices);
        }

        private void GenerateSlashVertices(Vector2 start, Vector2 end, Color startColor, Color endColor, float widthScale, List<VertexPositionColorTexture> vertices)
        {
            float maxWidth = 32f * widthScale;
            int segments = 80;

            Vector2 direction = end - start;
            Vector2 normal = Vector2.Normalize(new Vector2(-direction.Y, direction.X));

            for (int i = 0; i < segments; i++)
            {
                float t1 = (float)i / segments;
                float t2 = (float)(i + 1) / segments;

                float taper1 = CalculateTaperFactor(t1);
                float taper2 = CalculateTaperFactor(t2);

                float width1 = maxWidth * taper1;
                float width2 = maxWidth * taper2;

                Vector2 pos1 = Vector2.Lerp(start, end, t1);
                Vector2 pos2 = Vector2.Lerp(start, end, t2);

                Vector2 pos1Top = pos1 + normal * width1;
                Vector2 pos1Bottom = pos1 - normal * width1;
                Vector2 pos2Top = pos2 + normal * width2;
                Vector2 pos2Bottom = pos2 - normal * width2;

                pos1Top -= Main.screenPosition;
                pos1Bottom -= Main.screenPosition;
                pos2Top -= Main.screenPosition;
                pos2Bottom -= Main.screenPosition;

                Color color1 = Color.Lerp(startColor, endColor, t1);
                Color color2 = Color.Lerp(startColor, endColor, t2);

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Top.X, pos1Top.Y, 0f), color1, new Vector2(t1, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(t1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(t2, 0f)));

                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Top.X, pos2Top.Y, 0f), color2, new Vector2(t2, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1Bottom.X, pos1Bottom.Y, 0f), color1, new Vector2(t1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2Bottom.X, pos2Bottom.Y, 0f), color2, new Vector2(t2, 1f)));
            }
        }

        private float CalculateTaperFactor(float t)
        {
            float sinTaper = (float)Math.Sin(t * Math.PI);
            return sinTaper * sinTaper;
        }

        private void DrawPrimitives(List<VertexPositionColorTexture> vertices, Texture2D texture)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            Effect effect = OvermorrowModFile.Instance.TrailShader.Value;

            if (effect.Parameters["WorldViewProjection"] != null)
                effect.Parameters["WorldViewProjection"].SetValue(model * projection);
            else if (effect.Parameters["WVP"] != null)
                effect.Parameters["WVP"].SetValue(model * projection);

            effect.SafeSetParameter("uImage0", texture);
            effect.CurrentTechnique.Passes["Texturized"].Apply();

            Main.instance.GraphicsDevice.Textures[0] = texture;
            Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList,
                vertices.ToArray(), 0, vertices.Count / 3);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}