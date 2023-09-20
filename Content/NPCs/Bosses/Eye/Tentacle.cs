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
using Terraria.DataStructures;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    internal class EyeTentacle : ModProjectile
    {
        private bool RunOnce = true;
        private List<float> rots;

        public int length;
        public int parentID = -1;

        public bool lockGrow = false;
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => AssetDirectory.Trails + "Trail1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.hide = true;

            rots = new List<float>();
            length = 0;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public float value;
        //proj ai 0 is length and 1 is how fast it wiggles, dont make it too fast
        public override void AI()
        {
            if (parentID != -1)
            {
                if (RunOnce)
                {
                    Projectile.timeLeft = 420;
                    RunOnce = false;
                }

                NPC npc = Main.npc[parentID];
                if (!npc.active) Projectile.Kill();

                Projectile.Center = npc.Center;

                if (Projectile.timeLeft < 300)
                {
                    Projectile.timeLeft = 420;
                }

                Projectile.velocity = -npc.DirectionTo(npc.Center + Vector2.UnitY.RotatedBy(npc.rotation - MathHelper.PiOver4));

                //if (npc.velocity != Vector2.Zero)
                //    Projectile.velocity = -npc.velocity;

                for (int i = 0; i < 3; i++)
                {
                    value += Projectile.ai[1];
                    if (Projectile.timeLeft % 1 == 0)
                    {
                        float factor = 1f;
                        Vector2 velocity = Projectile.velocity * factor * 4f;
                        Projectile.rotation = 0.3f * (float)Math.Sin((double)(value / 100f)) + velocity.ToRotation();
                        rots.Insert(0, Projectile.rotation);
                        while (rots.Count > Projectile.ai[0])
                        {
                            rots.RemoveAt(rots.Count - 1);
                        }
                    }

                    // AI 0 indicates the maximum length, and should not be adjusted
                    // Adjust the length instead if you want to control the tentacle size
                    // The lockGrow boolean is for when the boss wants to manually control the size without the projectile interfering
                    if (length < Projectile.ai[0] && Projectile.timeLeft > 50 && !lockGrow)
                    {
                        length++;
                    }

                    if (length >= 0 && Projectile.timeLeft <= 50)
                    {
                        length -= 5;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            List<VertexInfo> bars = new List<VertexInfo>();
            for (int i = 1; i < length; i++)
            {
                float factor = i / (float)length;
                Vector2 v0 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * (i - 1)), 0f), rots[i - 1]);
                Vector2 v1 = Projectile.Center + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]);
                Vector2 normaldir = v1 - v0;
                normaldir = new Vector2(normaldir.Y, 0f - normaldir.X);
                normaldir.Normalize();
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                bars.Add(new VertexInfo(v1 + w * normaldir, Color.White, new Vector3(factor, 0f, 0f)));
                bars.Add(new VertexInfo(v1 - w * normaldir, Color.White, new Vector3(factor, 1f, 0f)));
            }

            if (bars.Count > 2)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                Effect effect = OvermorrowModFile.Instance.TentacleBlack.Value;

                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, Main.screenWidth, Main.screenHeight, 0f, 0f, 1f);
                Matrix model = Matrix.CreateTranslation(new Vector3(0f - Main.screenPosition.X, 0f - Main.screenPosition.Y, 0f)) * Main.GameViewMatrix.ZoomMatrix;

                effect.Parameters[0].SetValue(model * projection);
                effect.CurrentTechnique.Passes[0].Apply();

                Main.instance.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(Texture, AssetRequestMode.AsyncLoad).Value;
                Main.instance.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 1; i < length; i++)
            {
                float factor = i / (float)length;
                float w = (Projectile.ai[0] == 400 ? 28 : 32f) * MathHelper.SmoothStep(0.8f, 0.1f, factor);
                if (Collision.CheckAABBvAABBCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - new Vector2(w, w) + Utils.RotatedBy(new Vector2((float)(5 * i), 0f), rots[i]), new Vector2(w, w) * 2f))
                {
                    return true;
                }
            }

            return false;
        }
    }
    internal class Tentacle : ModProjectile
    {
        public override string Texture => AssetDirectory.Trails + "Trail1";
        private List<float> rots;

        public int len;
        float value;

        public override bool ShouldUpdatePosition() => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Color Tentacle");
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
                bars.Add(new VertexInfo(v1 + w * normaldir, Color.Red, new Vector3(factor, 0f, 0f)));
                bars.Add(new VertexInfo(v1 - w * normaldir, Color.Red, new Vector3(factor, 1f, 0f)));
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
                bars.Add(new VertexInfo(v1 + w * normaldir, Color.Red, new Vector3(factor, 0f, 0f)));
                bars.Add(new VertexInfo(v1 - w * normaldir, Color.Red, new Vector3(factor, 1f, 0f)));
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
