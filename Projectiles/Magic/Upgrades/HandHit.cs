using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.Upgrades
{
    public class HandHit : ModProjectile
    {
        private const string ChainTexturePath = "OvermorrowMod/Projectiles/Magic/Upgrades/HanDraw";
        private int timer = 0;

        bool right = false;
        bool left = false;

        int goright = 0;

        bool comebackright = false;
        bool comebackleft = false;

        int direction = -1;

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.timeLeft = 1000;
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        // This AI code is adapted from the aiStyle 15. We need to re-implement this to customize the behavior of our flail
        public override void AI()
        {


            if (projectile.Center.X < Main.player[projectile.owner].Center.X)
            {
                direction = 1;
            }



            float BetweenKill = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);

            Main.player[projectile.owner].itemRotation = (float)Math.Atan2(Main.player[projectile.owner].Center.Y - projectile.Center.Y * direction, Main.player[projectile.owner].Center.X - projectile.Center.X * direction);

            projectile.rotation = (Main.player[projectile.owner].Center - projectile.Center).ToRotation() + MathHelper.ToRadians(45f);
            timer++;
            if (timer == 1)
            {
                projectile.damage = projectile.damage - 5;
                if (Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    right = true;
                }
                else
                {
                    left = true;
                }
            }

            projectile.position.Y = Main.player[projectile.owner].Center.Y - projectile.height / 2;

            if (right)
            {
                goright += 10;
                projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + goright;
            }

            if (left)
            {
                goright -= 10;
                projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + goright;
            }

            if (projectile.Center.X > Main.player[projectile.owner].Center.X + 250)
            {
                comebackright = true;
                right = false;
            }

            if (projectile.Center.X < Main.player[projectile.owner].Center.X - 250)
            {
                comebackleft = true;
                left = false;
            }


            if (comebackright)
            {
                goright -= 10;
                projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + goright;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }

            if (comebackleft)
            {
                goright += 10;
                projectile.position.X = Main.player[projectile.owner].Center.X - projectile.width / 2 + goright;
                if (BetweenKill < 22)
                {
                    projectile.Kill();
                }
            }



        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 endPoint;
            endPoint.X = Main.player[projectile.owner].Center.X;
            endPoint.Y = Main.player[projectile.owner].Center.Y;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);
            projectile.alpha = 0;
            var drawPosition = projectile.Center;

            Vector2 unit = Main.player[projectile.owner].Center - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 24f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                spriteBatch.Draw(chainTexture, drawPos, null, alpha, (Main.player[projectile.owner].Center - projectile.Center).ToRotation() + MathHelper.ToRadians(45f), new Vector2(10f, 10f), 1f, SpriteEffects.None, 0f);
            }

            return true;
        }
    }
}
