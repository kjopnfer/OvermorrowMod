
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
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class BlackEcho : ModProjectile
    {
        public override string Texture => AssetDirectory.ArchiveProjectiles + Name;
        public override void SetDefaults()
        {
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
        public override void OnSpawn(IEntitySource source)
        {
            /*if (length < 50) // Max segments
            {
                float newRotation = 0f;
                rotations.Add(newRotation);
                length++;
            }*/
        }


        public override void AI()
        {
            Projectile.timeLeft = 5;

            if (length < 50) // Max segments
            {
                // Calculate rotation
                float newRotation = Projectile.rotation - (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.2f;

                rotations.Add(newRotation);
                length++;
            }

            base.AI();
        }

        private void DrawTentacle(Vector2 center, List<float> rotations, Texture2D texture, Effect effect = null, float segmentSpacing = 5f, float baseWidth = 32f)
        {
            if (rotations.Count < 2) return;

            var vertices = new List<VertexPositionColorTexture>();

            for (int i = 1; i < rotations.Count; i++)
            {
                float factor = i / (float)rotations.Count;

                Vector2 v0 = center + new Vector2(segmentSpacing * (i - 1), 0f).RotatedBy(rotations[i - 1]);
                Vector2 v1 = center + new Vector2(segmentSpacing * i, 0f).RotatedBy(rotations[i]);

                Vector2 normaldir = v1 - v0;
                normaldir = new Vector2(normaldir.Y, -normaldir.X);
                normaldir.Normalize();

                float width = baseWidth * MathHelper.SmoothStep(0.8f, 0.1f, factor);

                Vector2 pos1 = v1 + width * normaldir;
                Vector2 pos2 = v1 - width * normaldir;

                // Convert to screen space
                pos1 -= Main.screenPosition;
                pos2 -= Main.screenPosition;

                vertices.Add(new VertexPositionColorTexture(
                    new Vector3(pos1.X, pos1.Y, 0f),
                    Color.Black,
                    new Vector2(factor, 0f)
                ));
                vertices.Add(new VertexPositionColorTexture(
                    new Vector3(pos2.X, pos2.Y, 0f),
                    Color.Black,
                    new Vector2(factor, 1f)
                ));
            }

            if (vertices.Count > 2)
            {
                DrawPrimitives(vertices, texture, effect ?? OvermorrowModFile.Instance.TrailShader.Value);
            }
        }

        private void DrawPrimitives(List<VertexPositionColorTexture> vertices, Texture2D texture, Effect effect)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap,
                DepthStencilState.Default, RasterizerState.CullNone);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
            Matrix model = Matrix.CreateTranslation(new Vector3(0f, 0f, 0f)) * Main.GameViewMatrix.ZoomMatrix;

            // Use the proper parameter name that matches your shader
            if (effect.Parameters["WorldViewProjection"] != null)
                effect.Parameters["WorldViewProjection"].SetValue(model * projection);
            else if (effect.Parameters["WVP"] != null)
                effect.Parameters["WVP"].SetValue(model * projection);
            else if (effect.Parameters.Count > 0)
                effect.Parameters[0].SetValue(model * projection);

            effect.CurrentTechnique.Passes[0].Apply();

            Main.instance.GraphicsDevice.Textures[0] = texture;
            Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip,
                vertices.ToArray(), 0, vertices.Count - 2);

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

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, drawRectangle, Color.White * alpha, Projectile.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 1);

            Texture2D trailTexture = ModContent.Request<Texture2D>(AssetDirectory.Trails + "Laser").Value;
            DrawTentacle(Projectile.Center, rotations, texture);
            return false;
        }
    }
}