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
        public bool RunOnce = true;
        public bool Rainbow = false;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.LostSoulFriendly;
        public override bool ShouldUpdatePosition() => false;

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
            if (RunOnce)
            {
                Main.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, projectile.Center);
                RunOnce = false;
            }

            if (projectile.timeLeft > 20)
            {
                float value = 10;
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                projectile.ai[1]++;

                if (projectile.ai[1] > 90f && projectile.ai[1] < 90f + projectile.ai[0] / value)
                {
                    float progress = 1f - (projectile.ai[1] - 90f) / (projectile.ai[0] / value);
                    Vector2 velocity = Vector2.UnitX.RotatedBy(projectile.velocity.ToRotation() + Main.rand.NextFloat(-MathHelper.Pi / 4 * progress - 0.1f, MathHelper.Pi / 4 * progress + 0.1f)) * value;

                    // This makes it so the early thorns don't slam into your face at a 90 degree angle
                    if (progress > 0.95)
                    {
                        velocity = Vector2.UnitX.RotatedBy(projectile.velocity.ToRotation() + Main.rand.NextFloat(-MathHelper.PiOver2 / 4 * progress - 0.1f, MathHelper.PiOver2 / 4 * progress - 0.1f)) * value;
                    }

                    // Based on the sheer amount of projectiles that spawn I cannot do the following: 
                    //int proj = Projectile.NewProjectile(projectile.Center + projectile.velocity * (projectile.ai[1] - 90f) * value, velocity, ModContent.ProjectileType<Spike>(), projectile.damage, 1f, Main.myPlayer, progress, projectile.whoAmI);
                    //((Spike)Main.projectile[proj].modProjectile).Rainbow = Rainbow;

                    // Therefore I need to spawn another version that is literally the exact same except on how it draws
                    if (Rainbow)
                    {
                        Projectile.NewProjectile(projectile.Center + projectile.velocity * (projectile.ai[1] - 90f) * value, velocity, ModContent.ProjectileType<Spike_Rainbow>(), projectile.damage, 1f, Main.myPlayer, progress, projectile.whoAmI);
                    }
                    else
                    {
                        Projectile.NewProjectile(projectile.Center + projectile.velocity * (projectile.ai[1] - 90f) * value, velocity, ModContent.ProjectileType<Spike>(), projectile.damage, 1f, Main.myPlayer, progress, projectile.whoAmI);
                    }
                }
            }
            else
            {
                projectile.alpha += 15;

                if (projectile.alpha > 255) projectile.alpha = 255;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Extra_60");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Color color = Rainbow ? Main.DiscoColor : Color.Yellow;

            if (projectile.timeLeft > 20)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

                spriteBatch.Draw(texture, projectile.Center - new Vector2(0, 15) - Main.screenPosition, rect, Color.Lerp(color, Color.Transparent, (float)Math.Sin(projectile.ai[1] / 15f)), projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }
    }
}