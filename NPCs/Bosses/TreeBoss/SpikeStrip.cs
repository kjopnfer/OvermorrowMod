using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class SpikeStrip : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spike Strip");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.damage = 1;
            projectile.friendly = false;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 200; // Takes 180 ticks for the strip to fully draw
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            //Main.NewText(projectile.timeLeft + " alpha: " + projectile.alpha);

            if (projectile.timeLeft > 20)
            {
                float value = 10;
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                projectile.ai[1]++;

                if (projectile.ai[1] > 90f && projectile.ai[1] < 90f + projectile.ai[0] / value)
                {
                    float progress = 1f - (projectile.ai[1] - 90f) / (projectile.ai[0] / value);
                    Projectile.NewProjectile(projectile.Center + projectile.velocity * (projectile.ai[1] - 90f) * value, Vector2.UnitX.RotatedBy(projectile.velocity.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 4 * progress - 0.1f, MathHelper.Pi / 4 * progress + 0.1f)) * value, ModContent.ProjectileType<Spike>(), projectile.damage, 1f, Main.myPlayer, progress, projectile.whoAmI);
                }
            }
            else
            {
                projectile.alpha += 15;

                if (projectile.alpha > 255) projectile.alpha = 255;
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Extra_60");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            if (projectile.timeLeft > 20)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(texture, projectile.Center - new Vector2(0, 15) - Main.screenPosition, rect, Color.Lerp(new Color(0, 255, 191), Color.Transparent, (float)Math.Sin(projectile.ai[1] / 15f)), projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }
    }
}