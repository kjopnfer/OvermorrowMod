using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class FireRay2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 455;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)projectile.ai[0]];
            projectile.Center = npc.Center;
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 5, new Color(), 2f);
            }
        }



        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 endPoint;
            endPoint.X = -90 * (float)Math.Cos(projectile.rotation) + projectile.Center.X;
            endPoint.Y = -90 * (float)Math.Sin(projectile.rotation) + projectile.Center.Y;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }



        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            projectile.alpha = 0;
            if (!Main.gamePaused)
            {
                projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 200 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }
            projectile.velocity = Vector2.Zero;
            Vector2 endPoint;
            endPoint.X = -90 * (float)Math.Cos(projectile.rotation) + projectile.Center.X; // 5000 is basically the length
            endPoint.Y = -90 * (float)Math.Sin(projectile.rotation) + projectile.Center.Y;

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 4f)
            {
                Vector2 drawPos = projectile.Center +  unit * k - Main.screenPosition;
                Color alpha = Color.Orange * ((255 - projectile.alpha) / 255f);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, alpha, projectile.rotation, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
