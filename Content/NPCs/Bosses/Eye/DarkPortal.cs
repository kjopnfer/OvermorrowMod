using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Eye
{
    public class PortalStabber : ModProjectile
    {
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stab Portal");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.timeLeft = 720;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.ai[0]++ == 60)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.Pi), ModContent.ProjectileType<SmolTentacle>(), 10, 0f, Main.myPlayer, 75, 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[0] < 25)
            {
                Projectile.localAI[0]++;
            }
            else
            {
                if (Projectile.timeLeft <= 25) Projectile.localAI[0]--;
            }

            float progress2 = Utils.Clamp(Projectile.localAI[0]++, 0, 25f) / 25f;
            //float progress3 = Utils.Clamp(Projectile.localAI[1]++, 0, 25f) / 25f;

            DrawRing(AssetDirectory.Textures + "Vortex2", Main.spriteBatch, Projectile.Center, 5f, 5f, Main.GameUpdateCount / 20f, progress2, new Color(60, 3, 79));
            DrawRing(AssetDirectory.Textures + "VortexCenter", Main.spriteBatch, Projectile.Center, 3f, 3f, Main.GameUpdateCount / 40f, progress2, Color.Black);

            return false;
        }


        private void DrawRing(string texture, SpriteBatch spriteBatch, Vector2 position, float width, float height, float rotation, float prog, Color color)
        {
            var texRing = ModContent.Request<Texture2D>(texture).Value;
            Effect effect = OvermorrowModFile.Instance.Ring.Value;

            effect.Parameters["uTime"].SetValue(rotation);
            effect.Parameters["cosine"].SetValue((float)Math.Cos(rotation));
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uImageSize1"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uOpacity"].SetValue(prog);
            effect.CurrentTechnique.Passes["BowRingPass"].Apply();

            // The portal doesn't actually move because the shader will break the texture
            if (texture == AssetDirectory.Textures + "VortexCenter")
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, Projectile.rotation + MathHelper.PiOver2, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, Projectile.rotation + MathHelper.PiOver2, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }


            //spriteBatch.End();
            //spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }
    }

    public class EyePortal : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 256;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.timeLeft = (int)Projectile.ai[0];
        }

        public override void OnSpawn(IEntitySource source)
        {
            // I don't know why the SetDefault wasn't working lol
            Projectile.timeLeft = (int)Projectile.ai[0];
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            Projectile.hide = true;
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            /*if (Projectile.ai[1]++ % 15 == 0)
            {
                DelegateMethods.v3_1 = new Vector3(6f, 10f, 10f) * 0.2f;
                Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * 10f, 8f, DelegateMethods.CastLightOpen);
                float num108 = 16f;
                for (int num109 = 0; (float)num109 < num108; num109++)
                {
                    Vector2 spinningpoint5 = Vector2.UnitX * 0f;
                    spinningpoint5 += -Vector2.UnitY.RotatedBy((float)num109 * ((float)Math.PI * 2f / num108)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());
                    int num110 = Dust.NewDust(Projectile.Center, 0, 0, 27);
                    Main.dust[num110].scale = 1.5f;
                    Main.dust[num110].noGravity = true;
                    Main.dust[num110].position = Projectile.Center + spinningpoint5;
                    Main.dust[num110].velocity = Projectile.velocity * 0f + spinningpoint5.SafeNormalize(Vector2.UnitY) * 1f;
                }
            }*/

            if (Projectile.timeLeft < 30)
            {
                Projectile.localAI[0]--;
            }
            else
            {
                // The value that is set for the end-portal, this delays the portal showing up
                if (Projectile.ai[0] == 450)
                {
                    if (Projectile.ai[1] == 180)
                    {
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/NPC/EyePortalOpen")
                        {
                            Volume = 0.9f,
                            PitchVariance = 0.2f,
                            MaxInstances = 3,
                        });
                    }

                    if (Projectile.ai[1]++ >= 180)
                    {
                        if (Projectile.localAI[0] < 25) Projectile.localAI[0]++;
                    }
                }
                else
                {
                    if (Projectile.localAI[0] < 25) Projectile.localAI[0]++;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.rotation);
            float progress2 = Utils.Clamp(Projectile.localAI[0], 0, 25f) / 25f;

            DrawRing(AssetDirectory.Textures + "Vortex2", Main.spriteBatch, Projectile.Center - offset * 40f, 8f, 8f, Main.GameUpdateCount / 20f, progress2, new Color(60, 3, 79));
            DrawRing(AssetDirectory.Textures + "Vortex2", Main.spriteBatch, Projectile.Center - offset * 40f, 8f, 8f, Main.GameUpdateCount / 20f, progress2, new Color(60, 3, 79));
            DrawRing(AssetDirectory.Textures + "VortexCenter", Main.spriteBatch, Projectile.Center - offset * 40f, 6f, 6f, Main.GameUpdateCount / 40f, progress2, Color.Black);

            return false;
        }


        private void DrawRing(string texture, SpriteBatch spriteBatch, Vector2 position, float width, float height, float rotation, float prog, Color color)
        {
            var texRing = ModContent.Request<Texture2D>(texture).Value;
            Effect effect = OvermorrowModFile.Instance.Ring.Value;

            effect.Parameters["uTime"].SetValue(rotation);
            effect.Parameters["cosine"].SetValue((float)Math.Cos(rotation));
            effect.Parameters["uColor"].SetValue(color.ToVector3());
            effect.Parameters["uImageSize1"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            effect.Parameters["uOpacity"].SetValue(prog);
            effect.CurrentTechnique.Passes["BowRingPass"].Apply();

            // The portal center doesn't actually move because the shader will break the texture
            if (texture == AssetDirectory.Textures + "VortexCenter")
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, Projectile.rotation + MathHelper.PiOver2 * 3, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, Projectile.rotation + MathHelper.PiOver2 * 3, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }
}