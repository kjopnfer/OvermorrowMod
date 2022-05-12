using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Primitives;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    internal class Tentacle : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "Tentacle";
        private List<float> rots;

        public int len;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 600;
            Projectile.damage = 1000;
            Projectile.penetrate = -1;
            rots = new List<float>();
            len = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo> bars = new List<VertexInfo>();
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                Vector2 v0 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * (i - 1)), 0f), rots[i - 1]);
                Vector2 v1 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]);
                Vector2 normaldir = v1 - v0;
                normaldir = new Vector2(normaldir.Y, 0f - normaldir.X);
                ((Vector2)(normaldir)).Normalize();
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                bars.Add(new VertexInfo(v1 + w * normaldir, Color.White, new Vector3(factor, 0f, 0f)));
                bars.Add(new VertexInfo(v1 - w * normaldir, Color.White, new Vector3(factor, 1f, 0f)));
            }
            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                Effect effect = OvermorrowModFile.Instance.Tentacle.Value;

                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, (float)Main.screenWidth, (float)Main.screenHeight, 0f, 0f, 1f);
                Matrix model = Matrix.CreateTranslation(new Vector3(0f - Main.screenPosition.X, 0f - Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;
                
                effect.Parameters[0].SetValue(model * projection);
                effect.CurrentTechnique.Passes[0].Apply();
                
                ((Game)Main.instance).GraphicsDevice.Textures[0] = (Texture)(object)ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
                ((Game)Main.instance).GraphicsDevice.DrawUserPrimitives<VertexInfo>((PrimitiveType)1, bars.ToArray(), 0, bars.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        float value;
        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                value += Projectile.ai[1];
                if (base.Projectile.timeLeft % 1 == 0)
                {
                    float factor = 1f;
                    Vector2 velocity = base.Projectile.velocity * factor * 4f;
                    Projectile.rotation = 0.3f * (float)Math.Sin((double)(value / 100f)) + velocity.ToRotation();
                    rots.Insert(0, Projectile.rotation);
                    while (rots.Count > Projectile.ai[0])
                    {
                        rots.RemoveAt(rots.Count - 1);
                    }
                }
                if (len < Projectile.ai[0] && Projectile.timeLeft > 50)
                {
                    len++;
                }
                if (len >= 0 && Projectile.timeLeft <= 50)
                {
                    len -= 5;
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                if (Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - new Vector2(w, w) + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]), new Vector2(w, w) * 2f))
                {
                    return true;
                }
            }
            return false;
        }
    }
    internal class SmolTentacle : ModProjectile
    {
        public override string Texture => AssetDirectory.Textures + "Laser";
        private List<float> rots;

        public int len;

        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 600;
            Projectile.damage = 1000;
            Projectile.penetrate = -1;
            rots = new List<float>();
            len = 0;
        }

        float value;
        //proj ai 0 is length and 1 is how fast it wiggles, dont make it too fast
        public override void AI()
        {
            for (int i = 0; i < 3; i++)
            {
                value += Projectile.ai[1];
                if (base.Projectile.timeLeft % 1 == 0)
                {
                    float factor = 1f;
                    Vector2 velocity = base.Projectile.velocity * factor * 4f;
                    Projectile.rotation = 0.3f * (float)Math.Sin((double)(value / 100f)) + velocity.ToRotation();
                    rots.Insert(0, Projectile.rotation);
                    while (rots.Count > Projectile.ai[0])
                    {
                        rots.RemoveAt(rots.Count - 1);
                    }
                }
                if (len < Projectile.ai[0] && Projectile.timeLeft > 50)
                {
                    len++;
                }
                if (len >= 0 && Projectile.timeLeft <= 50)
                {
                    len -= 5;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo> bars = new List<VertexInfo>();
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                Vector2 v0 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * (i - 1)), 0f), rots[i - 1]);
                Vector2 v1 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]);
                Vector2 normaldir = v1 - v0;
                normaldir = new Vector2(normaldir.Y, 0f - normaldir.X);
                ((Vector2)(normaldir)).Normalize();
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                bars.Add(new VertexInfo(v1 + w * normaldir, Color.White, new Vector3(factor, 0f, 0f)));
                bars.Add(new VertexInfo(v1 - w * normaldir, Color.White, new Vector3(factor, 1f, 0f)));
            }
            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)1, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                Effect effect = OvermorrowModFile.Instance.TentacleBlack.Value;

                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, (float)Main.screenWidth, (float)Main.screenHeight, 0f, 0f, 1f);
                Matrix model = Matrix.CreateTranslation(new Vector3(0f - Main.screenPosition.X, 0f - Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;
                
                effect.Parameters[0].SetValue(model * projection);
                effect.CurrentTechnique.Passes[0].Apply();
                
                ((Game)Main.instance).GraphicsDevice.Textures[0] = (Texture)(object)ModContent.Request<Texture2D>(Texture, (AssetRequestMode)2).Value;
                ((Game)Main.instance).GraphicsDevice.DrawUserPrimitives<VertexInfo>((PrimitiveType)1, bars.ToArray(), 0, bars.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin((SpriteSortMode)0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, (Effect)null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < len; i++)
            {
                float factor = (float)i / (float)len;
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                if (Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - new Vector2(w, w) + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]), new Vector2(w, w) * 2f))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
