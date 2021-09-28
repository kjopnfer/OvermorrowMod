using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class EvilRay5 : ModProjectile
    {

        private int timer = 0;
        private int rot = 700;
        Color alpha = Color.Purple;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evil Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.timeLeft = 300;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            projectile.Center = parentProjectile.Center;
            timer++;
            if (timer == 1)
            {
                projectile.rotation = MathHelper.ToRadians(45f);
            }

            if (timer == 150)
            {
                alpha = Color.Red;
                rot = -700;
            }
        }




        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 endPoint;
            endPoint.X = -405 * (float)Math.Cos(projectile.rotation) + projectile.Center.X;
            endPoint.Y = -405 * (float)Math.Sin(projectile.rotation) + projectile.Center.Y;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            projectile.alpha = 0;

            projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * rot / 10)); // 200 is the speed, god only knows what dividing by 10 does

            projectile.velocity = Vector2.Zero;
            Vector2 endPoint;
            endPoint.X = -405 * (float)Math.Cos(projectile.rotation) + projectile.Center.X; // 5000 is basically the length
            endPoint.Y = -405 * (float)Math.Sin(projectile.rotation) + projectile.Center.Y;

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 10f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, alpha, projectile.rotation, new Vector2(5f, 5f), 1f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
