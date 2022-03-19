using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Buffs.Debuffs;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.SoulSaber
{
    public class SoulFlameRing : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Textures/FlameRing";

        public override bool CanDamage() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("SoulFlame Ring");
        }

        public override void SetDefaults()
        {
            projectile.height = 285;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 540;
            projectile.alpha = 200;
        }

        public override void AI()
        {
            projectile.rotation += 0.09f;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && projectile.Distance(npc.Center) < 150)
                {
                    npc.AddBuff(ModContent.BuffType<SoulFlame>(), 60 * 10);
                }
            }

            projectile.velocity.X *= 0.995f;            
            projectile.velocity.Y *= 0.995f ;
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/FlameRing");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = MathHelper.Lerp(0.95f, 1.05f, (float)Math.Sin(projectile.ai[0]++ / 20f));
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, Color.Lerp(Color.Transparent, Color.Blue, Utils.Clamp(projectile.timeLeft, 0, 60) / 60f), projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            scale = MathHelper.Lerp(0.95f, 1.05f, (float)Math.Sin(projectile.ai[0] / 20f));
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, Color.Lerp(Color.Transparent, new Color(35, 200, 254), Utils.Clamp(projectile.timeLeft, 0, 60) / 60f), projectile.rotation * 1.1f, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}