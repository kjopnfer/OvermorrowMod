using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.GemStaves
{
    public class EmeraldProj : ModProjectile
    {
        private int timer = 0;
        Vector2 targetPosition;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.timeLeft = 200;
            Projectile.penetrate = 1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            timer++;
            if (timer == 1)
            {
                targetPosition = Main.MouseWorld;
            }
            if (timer < 10)
            {
                Vector2 position = Projectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                Projectile.velocity += direction * 1.5f;
            }
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), 2, 2, DustID.Grass, Projectile.velocity.X, Projectile.velocity.Y, 10, Color.Green, 1.2f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }


        }
    }
}
