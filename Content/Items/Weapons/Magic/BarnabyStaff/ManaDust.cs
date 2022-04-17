using OvermorrowMod.Core;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.BarnabyStaff
{
    public class ManaDust : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mana Storm");
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            /*if (initProperties)
            {
                // AI[0] is the parent Projectile, AI[1] is the direction
                parentProjectile = Main.Projectile[(int)Projectile.ai[0]];
                if (Main.netMode != NetmodeID.Server && Projectile.owner == Main.myPlayer)
                {
                    if(Projectile.ai[1] == 0)
                    {
                        Projectile.NewProjectile(parentProjectile.Center, Vector2.Zero, ModContent.ProjectileType<ManaDust>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, parentProjectile.whoAmI, 1);
                    }
                }

                Projectile.ai[0] = Projectile.ai[1] == 0 ? 0 : 1;
                Projectile.ai[1] = 0;
                initProperties = false;
            }
            Lighting.AddLight(Projectile.Center, 0f, 0f, .5f);

            float radius = 30;

            for (int i = 0; i < 2; i++)
            {
                //Vector2 dustPos = Projectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians(i * 10 + Projectile.ai[1]));
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 113, Vector2.Zero, 0, default, 1f);
                dust.noGravity = true;
            }

            Projectile.ai[1] += Projectile.ai[0] == 0 ? 16 : -16;

            if (Projectile.ai[0] == 0)
            {
                Projectile.position = parentProjectile.Center + new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((int)Projectile.knockBack * 10 + Projectile.ai[1]));
            }
            else
            {
                Projectile.position = parentProjectile.Center - new Vector2(radius, 0).RotatedBy(MathHelper.ToRadians((int)Projectile.knockBack * 10 + Projectile.ai[1]));
            }

            if (parentProjectile.active)
            {
                Projectile.timeLeft = 2;
            }*/
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 3;
        }
    }
}