using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class TopazProj : ModProjectile
    {
        private int timer = 0;
        Vector2 targetPosition;

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 14;
            projectile.timeLeft = 200;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            timer++;
            if (timer == 1)
            {
                targetPosition = Main.MouseWorld;
            }
            if (timer < 10)
            {
                Vector2 position = projectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity += direction * 1.5f;
            }
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, projectile.velocity.X, projectile.velocity.Y, 75, Color.Yellow, 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }


        }
    }
}
