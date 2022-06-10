using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using System;
using Terraria;
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

    public class DarkPortal : ModNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal Test");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 256;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float progress2 = Utils.Clamp(NPC.localAI[1]++, 0, 25f) / 25f;
            float progress3 = Utils.Clamp(NPC.localAI[2]++, 0, 25f) / 25f;

            DrawRing(AssetDirectory.Textures + "Vortex2", spriteBatch, NPC.Center, 8f, 8f, Main.GameUpdateCount / 20f, progress3, new Color(60, 3, 79));
            DrawRing(AssetDirectory.Textures + "VortexCenter", spriteBatch, NPC.Center, 6f, 6f, Main.GameUpdateCount / 40f, progress2, Color.Black);

            return false;
        }


        private void DrawRing(string texture, SpriteBatch spriteBatch, Vector2 position, float width, float height, float rotation, float prog, Color color)
        {
            var texRing = ModContent.Request<Texture2D>(texture).Value;
            Effect effect = OvermorrowModFile.Instance.Ring.Value;

            //effect.Parameters["uProgress"].SetValue(rotation);
            //effect.Parameters["uColor"].SetValue(color.ToVector3());
            //effect.Parameters["uImageSize1"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
            //effect.Parameters["uOpacity"].SetValue(prog);
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
                spriteBatch.Draw(texRing, target, null, color * prog, NPC.rotation + MathHelper.PiOver2, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, NPC.rotation + MathHelper.PiOver2, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }


            //spriteBatch.End();
            //spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }
    }
    public class DarkPortal2 : ModNPC
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Portal Test 2");
        }

        public override void SetDefaults()
        {
            NPC.width = NPC.height = 256;
            NPC.lifeMax = 640;
            NPC.aiStyle = -1;
            NPC.defense = 16;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.friendly = false;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 offset = Vector2.UnitY.RotatedBy(NPC.rotation);
            float progress2 = Utils.Clamp(NPC.localAI[1]++, 0, 25f) / 25f;
            float progress3 = Utils.Clamp(NPC.localAI[2]++, 0, 25f) / 25f;

            DrawRing(AssetDirectory.Textures + "Vortex2", spriteBatch, NPC.Center, 5f, 5f, Main.GameUpdateCount / 20f, progress3, new Color(60, 3, 79));
            DrawRing(AssetDirectory.Textures + "VortexCenter", spriteBatch, NPC.Center + offset * 6f, 3f, 3f, Main.GameUpdateCount / 40f, progress2, Color.Black);

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
                spriteBatch.Draw(texRing, target, null, color * prog, NPC.rotation + MathHelper.PiOver2 * 3, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
            else
            {
                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.Additive, default, default, default, effect, Main.GameViewMatrix.ZoomMatrix);

                var target = ModUtils.toRect(position, (int)(16 * (width + prog)), (int)(60 * (height + prog)));
                spriteBatch.Draw(texRing, target, null, color * prog, NPC.rotation + MathHelper.PiOver2 * 3, texRing.Size() / 2, 0, 0);

                spriteBatch.End();
                spriteBatch.Begin(default, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
            }
        }
    }

}