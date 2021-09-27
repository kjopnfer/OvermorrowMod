using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic.Upgrades
{
    public class EvilShotBlast1 : ModProjectile
    {

        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 540;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            //aiType = 0;
            //projectile.aiStyle = 0;
        }




        public override void Kill(int timeLeft)
        {
            Vector2 RotValue1 = new Vector2(-projectile.velocity.X, -projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(10f));
            Vector2 RotValue2 = new Vector2(-projectile.velocity.X, -projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-10f));
            Vector2 RotValue3 = new Vector2(-projectile.velocity.X, -projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(20f));
            Vector2 RotValue4 = new Vector2(-projectile.velocity.X, -projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-20f));
            Projectile.NewProjectile(projectile.Center, -projectile.velocity, ModContent.ProjectileType<EvilShotBlast2>(), projectile.damage / 2, 3f, projectile.owner, 0f);    
            Projectile.NewProjectile(projectile.Center, RotValue1, ModContent.ProjectileType<EvilShotBlast2>(), projectile.damage / 2, 3f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center, RotValue2, ModContent.ProjectileType<EvilShotBlast2>(), projectile.damage / 2, 3f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center, RotValue3, ModContent.ProjectileType<EvilShotBlast2>(), projectile.damage / 2, 3f, projectile.owner, 0f);
            Projectile.NewProjectile(projectile.Center, RotValue4, ModContent.ProjectileType<EvilShotBlast2>(), projectile.damage / 2, 3f, projectile.owner, 0f);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                projectile.velocity.X *= 2;
                projectile.velocity.Y *= 2;
            }
            return true;
        }

        public override void AI()
        {


            timer++;
            if(timer == 1)
            {
                if(Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    projectile.velocity.X = 10;
                    projectile.velocity.Y = 0;
                }
                else
                {
                    projectile.velocity.X = -10;
                    projectile.velocity.Y = 0;
                }
            }





            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            Color Bloodc = Color.Red;
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.HeartCrystal, projectile.velocity.X, projectile.velocity.Y, 1, Bloodc, 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }
    }


    public class EvilShotBlast2 : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        int timer = 0;
        int SavedDMG = 0;
        int bounce = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vile Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 100;
            projectile.alpha = 255;
            projectile.tileCollide = true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            {
                if(bounce < 3)
                {
                    projectile.velocity.X *= -1;
                    projectile.velocity.Y *= -1;
                    bounce++;
                }
                else
                {
                    projectile.Kill();
                }
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if(bounce < 3)
            {
                projectile.velocity.X *= -1;
                projectile.velocity.Y *= -1;
            }
            else
            {
                projectile.Kill();
            }
        }
        
        public override void AI()
        {

            timer++;
            if(timer == 1)
            {
                SavedDMG = projectile.damage;
                projectile.damage = 0;
            }
            if(timer > 5)
            {
                projectile.damage = SavedDMG;
            }


            Lighting.AddLight(projectile.Center, 1f, 0f, 0f);
            {
                int num1110 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Vile, projectile.velocity.X, projectile.velocity.Y, 50, new Color(), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;
            }
        }
    }
}