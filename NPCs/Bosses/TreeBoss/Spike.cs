using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class Spike : ModProjectile
    {
        private bool canDespawn = false;
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.damage = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 45;
            projectile.scale = 0f;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.alpha = Main.projectile[(int)projectile.ai[1]].alpha;

            if (projectile.alpha == 255) projectile.Kill();

            if (projectile.timeLeft < 5 && Main.projectile[(int)projectile.ai[1]].active)
            {
                projectile.timeLeft = 5;
                canDespawn = true;
            }


            if (!canDespawn)
            {
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                float progress = (float)(45 - projectile.timeLeft) / 45f;
                float val = (float)Math.Sin(progress * Math.PI);
                val *= 3;
                if (val > 1) val = 1;

                if (projectile.timeLeft > 5)
                {
                    projectile.scale = val * projectile.ai[0] / 2 + 0.1f;
                }
            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * 200f * projectile.scale, 32f * projectile.scale, ref a);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteBatch SpriteBatch = Main.spriteBatch;
            Texture2D texture = Main.projectileTexture[projectile.type];
            Vector2 origin = new Vector2(0, texture.Height / 2);

            Color color = Lighting.GetColor((int)projectile.Center.X / 16, (int)(projectile.Center.Y / 16f));

            SpriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Lerp(color, Color.Transparent, projectile.alpha / 255f), projectile.velocity.ToRotation(), origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class Spike_Rainbow : ModProjectile
    {
        private bool canDespawn = false;
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.damage = 1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 45;
            projectile.scale = 0f;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.alpha = Main.projectile[(int)projectile.ai[1]].alpha;

            if (projectile.alpha == 255) projectile.Kill();

            if (projectile.timeLeft < 5 && Main.projectile[(int)projectile.ai[1]].active)
            {
                projectile.timeLeft = 5;
                canDespawn = true;
            }


            if (!canDespawn)
            {
                projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY);
                float progress = (float)(45 - projectile.timeLeft) / 45f;
                float val = (float)Math.Sin(progress * Math.PI);
                val *= 3;
                if (val > 1) val = 1;

                if (projectile.timeLeft > 5)
                {
                    projectile.scale = val * projectile.ai[0] / 2 + 0.1f;
                }
            }

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * 200f * projectile.scale, 32f * projectile.scale, ref a);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteBatch SpriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/NPCs/Bosses/TreeBoss/Spike_Rainbow");
            Vector2 origin = new Vector2(0, texture.Height / 2);

            Color color = Main.DiscoColor;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            SpriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Lerp(color, Color.Transparent, projectile.alpha / 255f), projectile.velocity.ToRotation(), origin, projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}