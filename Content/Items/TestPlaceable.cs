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

namespace OvermorrowMod.Content.Projectiles
{
    public class PrimitiveRing : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        private struct RingData
        {
            public Vector2 Center;
            public float InnerRadius;
            public float OuterRadius;
            public float MaxRadius;
            public float Alpha;
            public float Thickness;
            public float Rotation;
        }

        private RingData ringData;

        public override void OnSpawn(IEntitySource source)
        {
            InitializeRing();
        }

        private void InitializeRing()
        {
            ringData = new RingData
            {
                Center = Projectile.Center,
                InnerRadius = 10f,
                OuterRadius = 15f,
                MaxRadius = 150f,
                Alpha = 1f,
                Thickness = 5f,
                Rotation = 0f
            };
        }

        public override void AI()
        {
            Projectile.Center = Main.LocalPlayer.Center;
            UpdateRing();
        }

        private void UpdateRing()
        {
            float normalizedTime = 1f - (float)Projectile.timeLeft / 60f;

            //if (normalizedTime < 0.7f)
            {
                float expansionProgress = normalizedTime / 0.7f;
                float easedProgress = EasingUtils.EaseOutQuart(expansionProgress);

                float currentRadius = MathHelper.Lerp(15f, ringData.MaxRadius, 1f);

                var tempRing = ringData;
                tempRing.InnerRadius = currentRadius - ringData.Thickness;
                tempRing.OuterRadius = currentRadius + ringData.Thickness;
                tempRing.Alpha = 1f;
                tempRing.Thickness = MathHelper.Lerp(5f, 60f, 1f);
                tempRing.Rotation += 0.02f;
                tempRing.Center = Projectile.Center;

                ringData = tempRing;
            }
            //else
            //{
            //    float fadeProgress = (normalizedTime - 0.7f) / 0.3f;

            //    var tempRing = ringData;
            //    tempRing.Alpha = (1f - fadeProgress) * (1f - fadeProgress);
            //    tempRing.Rotation += 0.02f;

            //    ringData = tempRing;
            //}
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var vertices = new List<VertexPositionColorTexture>();

            GenerateRingVertices(ringData, Color.White, Color.Cyan, vertices);

            if (vertices.Count > 0)
            {
                DrawRingPrimitives(vertices, ModContent.Request<Texture2D>(AssetDirectory.Trails + "Jagged").Value);
            }

            return false;
        }

        private void GenerateRingVertices(RingData ring, Color innerColor, Color outerColor, List<VertexPositionColorTexture> vertices)
        {
            if (ring.Alpha <= 0) return;

            int segments = 64;
            float angleStep = MathHelper.TwoPi / segments;

            Vector2 screenCenter = ring.Center - Main.screenPosition;

            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep + ring.Rotation;
                float angle2 = (i + 1) * angleStep + ring.Rotation;

                Vector2 direction1 = new Vector2((float)Math.Cos(angle1), (float)Math.Sin(angle1));
                Vector2 direction2 = new Vector2((float)Math.Cos(angle2), (float)Math.Sin(angle2));

                Vector2 innerPos1 = screenCenter + direction1 * ring.InnerRadius;
                Vector2 outerPos1 = screenCenter + direction1 * ring.OuterRadius;
                Vector2 innerPos2 = screenCenter + direction2 * ring.InnerRadius;
                Vector2 outerPos2 = screenCenter + direction2 * ring.OuterRadius;

                Color finalInnerColor = innerColor * ring.Alpha;
                Color finalOuterColor = outerColor * ring.Alpha;

                float u1 = (float)i / segments;
                float u2 = (float)(i + 1) / segments;

                vertices.Add(new VertexPositionColorTexture(new Vector3(innerPos1, 0f), finalInnerColor, new Vector2(u1, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(outerPos1, 0f), finalOuterColor, new Vector2(u1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(innerPos2, 0f), finalInnerColor, new Vector2(u2, 0f)));

                vertices.Add(new VertexPositionColorTexture(new Vector3(innerPos2, 0f), finalInnerColor, new Vector2(u2, 0f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(outerPos1, 0f), finalOuterColor, new Vector2(u1, 1f)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(outerPos2, 0f), finalOuterColor, new Vector2(u2, 1f)));
            }
        }

        private void DrawRingPrimitives(List<VertexPositionColorTexture> vertices, Texture2D texture)
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