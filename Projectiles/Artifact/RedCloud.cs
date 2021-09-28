using Microsoft.Xna.Framework;
using OvermorrowMod.WardenClass;
using Terraria;

namespace OvermorrowMod.Projectiles.Artifact
{
    public class RedCloud : ArtifactProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Moon");
            Main.projFrames[projectile.type] = 10;
        }

        public override void SafeSetDefaults()
        {
            projectile.width = 144;
            projectile.height = 106;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 5400; // 1.5 minutes
        }


        /*public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1.2f, 0f, 0f);

            Player player = Main.player[projectile.owner];
            if (player.dead || !player.active)
            {
                return;
            }

            projectile.ai[0] += 1;
            if(projectile.ai[1] < AuraRadius) // The radius
            {
                projectile.ai[1] += 15;
            }
            else
            {
                isActive = true;
            }
           
            Vector2 dustVelocity = Vector2.UnitX * 18f;
            dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
            Vector2 spawnPos = projectile.Center + dustVelocity;
            Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f);
            Vector2 velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * 6 / 10f;

            Vector2 origin = projectile.Center/* some position ;
            //float radius = 450;
            //Get 30 locations in a circle around 'origin'
            //int numLocations = 2;
            /*for (int i = 0; i < 4; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                //Dust dust = Main.dust[Dust.NewDust(position, 5, 5, 60, velocity.X, velocity.Y / 2f, 0, default, 2.04f)];
                Dust dust = Dust.NewDustPerfect(position, 60, new Vector2(velocity.X, velocity.Y / 2f), 0, default, 2.04f);
                dust.noGravity = true;

                double deg = (double)projectile.ai[0] + position.X; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double degY = (double)projectile.ai[0]; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double radY = degY * (Math.PI / 180); //Convert degrees to radians
                double dist = 450; //Distance away from the Center
                //dust.position.X = projectile.Center.X - (int)(Math.Cos(rad) * dist);
                //dust.position.Y = projectile.Center.Y - (int)(Math.Sin(rad) * dist);

                dust.position = projectile.Center + new Vector2(450, 0).RotatedBy(MathHelper.ToRadians(projectile.ai[0] * 2));
            }

            for (int i = 0; i < 36; i++)
            {
                Vector2 dustPos = projectile.Center + new Vector2(projectile.ai[1], 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[0]));
                Dust dust = Main.dust[Dust.NewDust(dustPos, 15, 15, 60, 0f, 0f, 0, default, 2.04f)];
                dust.noGravity = true;
            }

            if (isActive)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                    if (distance <= AuraRadius)
                    {
                        Main.player[i].AddBuff(ModContent.BuffType<MoonBuff>(), 60);
                    }
                }
            }

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
                if(projectile.timeLeft >= 30)
                {
                    projectile.ai[1] -= 15;
                }
            }

            // Weird spooky dust movement
            /* Vector2 dustVelocity = Vector2.UnitX * 18f;
            dustVelocity = dustVelocity.RotatedBy(projectile.rotation - 1.57f);
            Vector2 spawnPos = projectile.Center + dustVelocity;
            Vector2 spawn = spawnPos + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f);
            Vector2 velocity = Vector2.Normalize(spawnPos - spawn) * 1.5f * 6 / 10f;

            Vector2 origin = projectile.Center/* some position ;
            float radius = 450;
            //Get 30 locations in a circle around 'origin'
            int numLocations = 32;
            for (int i = 0; i < 32; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Dust dust = Main.dust[Dust.NewDust(position, 5, 5, 60, velocity.X, velocity.Y / 2f, 0, default, 2.04f)];
                dust.noGravity = true;

                double deg = (double)projectile.ai[0] + position.X; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double degY = (double)projectile.ai[0] + position.Y; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radians
                double radY = degY * (Math.PI / 180); //Convert degrees to radians
                double dist = 450; //Distance away from the Center

                dust.position.X = projectile.Center.X - (int)(Math.Cos(rad) * dist);
                dust.position.Y = projectile.Center.Y - (int)(Math.Sin(radY) * dist);
            }

            // Loop through the 10 animation frames, spending 12 ticks on each.
            if (++projectile.frameCounter >= 12)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }*/

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}