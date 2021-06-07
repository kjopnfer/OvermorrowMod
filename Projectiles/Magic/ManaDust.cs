using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class ManaDust : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        private Projectile parentProjectile;
        private bool initProperties = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Storm");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 1;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            /*if (initProperties)
            {
                // AI[0] is the parent projectile, AI[1] is the direction
                parentProjectile = Main.projectile[(int)projectile.ai[0]];
                if (Main.netMode != NetmodeID.Server && projectile.owner == Main.myPlayer)
                {
                    if(projectile.ai[1] == 0)
                    {
                        Projectile.NewProjectile(parentProjectile.Center, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), projectile.damage / 2, projectile.knockBack, projectile.owner, parentProjectile.whoAmI, 1);
                    }
                }

                projectile.ai[0] = projectile.ai[1] == 0 ? 0 : 1;
                projectile.ai[1] = 0;
                initProperties = false;
            }
            Lighting.AddLight(projectile.Center, 0f, 0f, .5f);

            float radius = 30;

            for (int i = 0; i < 2; i++)
            {
                //Vector2 dustPos = projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(projectile.Center, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
            }

            projectile.ai[1] += projectile.ai[0] == 0 ? 16 : -16;

            if (projectile.ai[0] == 0)
            {
                projectile.position = parentProjectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((int)projectile.knockBack * 10 + projectile.ai[1]));
            }
            else
            {
                projectile.position = parentProjectile.Center - new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((int)projectile.knockBack * 10 + projectile.ai[1]));
            }

            if (parentProjectile.active)
            {
                projectile.timeLeft = 2;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 3;
        }
    }
}