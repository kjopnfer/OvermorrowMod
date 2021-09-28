using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class UnholyLight : ModProjectile
    {
        private int length = 1;
        private int timer = 0;
        bool HasActivedGo = false;
        bool HasActivedSprite = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Un-Holy Light");
            Main.projFrames[base.projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = 1;
            projectile.hostile = true;
            projectile.light = 5f;
            projectile.friendly = false;
            projectile.alpha = 40;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            projectile.damage = 40;
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            timer++;
            if (!HasActivedGo)
            {
                projectile.timeLeft = 100;
                if (timer == 1)
                {
                    length++;
                }

                if (timer == 2)
                {
                    timer = 0;
                }
                //float point = 0f;
                projectile.velocity.Y = 0f;
                //Vector2 endPoint;
                projectile.position.X = length * (float)Math.Cos(projectile.rotation) + parentProjectile.Center.X - 20;
                projectile.position.Y = length * (float)Math.Sin(projectile.rotation) + parentProjectile.Center.Y - 20;
                projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 50 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }

            if (parentProjectile.ranged)
            {
                HasActivedGo = true;
            }


            if (parentProjectile.melee)
            {
                HasActivedSprite = true;
            }

            if (HasActivedSprite)
            {
                projectile.frame = 1;
            }


            if (HasActivedGo)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = parentProjectile.Center;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                float speed = 11f;
                projectile.velocity = -direction * speed;
            }

        }

        public override void Kill(int timeLeft)
        {
            Vector2 position = projectile.Center;
            Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
            Main.PlaySound(SoundID.NPCHit, projectile.Center, 1);
            int radius = 5;     //this is the explosion radius, the highter is the value the bigger is the explosion

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                    {
                        Color alpha = Color.Red;
                        Dust.NewDust(position, 5, 5, DustID.Enchanted_Gold, 0.0f, 0.0f, 120, alpha, 1f);
                    }
                }
            }
        }
    }
}
