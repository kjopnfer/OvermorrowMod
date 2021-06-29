using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.HellFire
{
    public class HellFireDown : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Magic/HellFire/HellFireDraw";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("HellFire");
        }

        Vector2 PosCheck;
	    Vector2 endPoint;
        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 275;
            projectile.light = 1f;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.extraUpdates = 1;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 150);
        }


        public override void AI()
        {
            timer++;
            if(timer == 1)
            {
                PosCheck = projectile.Center;
                endPoint = PosCheck;
            }
            if(timer == 3)
            {
                projectile.alpha = 0;
            }
            if(timer < 45)
            {
                projectile.velocity.Y = 5;
            }
            else
            {
                projectile.velocity.Y = 0;
            }
            if(timer == 40)
            {
                Projectile.NewProjectile(projectile.Center.X - 60, projectile.Center.Y - 35, 0, 0, mod.ProjectileType("HellFireRight"), projectile.damage, 0f, projectile.owner, 0f);
            }

            if(timer > 44)
            {
                int Random1 = Main.rand.Next(-40, 40);
                int Random2 = Main.rand.Next(-200, 20);

                float XDustposition = projectile.Center.X + Random1 - 18;
                float YDustposition = projectile.Center.Y + Random2 - 20;
                Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                Vector2 Dusttarget = projectile.Center;
                Vector2 Dustdirection = Dusttarget - VDustposition;
                Dustdirection.Normalize();

                Color granitedustc = Color.Orange;
                {
                    int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 400, granitedustc, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Vector2 velocity = Dustdirection * 2;
                    Main.dust[dust].velocity = Dustdirection * 2;
                }
            }
        }

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float point = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];

            Vector2 mountedCenter = endPoint;
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);

            var drawPosition = projectile.Center;
            var remainingVectorToPlayer = mountedCenter - drawPosition;

            float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

            if (projectile.alpha == 0)
            {
                int direction = -1;

                if (projectile.Center.X < mountedCenter.X)
                    direction = 1;

            }

            // This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
            while (true)
            {
                float length = remainingVectorToPlayer.Length();

                if (length < 25f || float.IsNaN(length))
                    break;

                // drawPosition is advanced along the vector back to the player by 12 pixels
                // 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
                drawPosition += remainingVectorToPlayer * 20 / length;
                remainingVectorToPlayer = mountedCenter - drawPosition;

                // Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
                spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, color, 0, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
