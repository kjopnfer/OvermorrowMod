using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.NPCs.Bosses.TreeBoss;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class CurvedTelegraph : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Curved Telegraph");
        }

        public override void SetDefaults()
        {
            projectile.width = 1004;
            projectile.height = 1004;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.alpha = 200;

            //drawOriginOffsetY = 480;
        }

        public override void AI()
        {
            //projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(1f));
            projectile.rotation = MathHelper.ToRadians(projectile.ai[0]);
            /*if (projectile.ai[0]++ % 2 == 0)
            {
                Dust dust = Terraria.Dust.NewDustPerfect(projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }*/
        }

        //public override Color? GetAlpha(Color lightColor) => Main.DiscoColor * ((255 - projectile.alpha) / 255f);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/CurvedTelegraph");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, Main.DiscoColor, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class CurvedTelegraph2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Curved Telegraph");
        }

        public override void SetDefaults()
        {
            // 120 or 122 blocks diameter
            projectile.width = 2038;
            projectile.height = 2038;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 900;
            projectile.alpha = 200;

            //drawOriginOffsetY = 480;
        }

        public override void AI()
        {
            //projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(1f));
            projectile.rotation = MathHelper.ToRadians(projectile.ai[0]);
            /*if (projectile.ai[0]++ % 2 == 0)
            {
                Dust dust = Terraria.Dust.NewDustPerfect(projectile.Center, 107, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }*/
        }

        //public override Color? GetAlpha(Color lightColor) => Main.DiscoColor * ((255 - projectile.alpha) / 255f);

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/CurvedTelegraph2");
            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, drawRectangle, Main.DiscoColor, projectile.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

}