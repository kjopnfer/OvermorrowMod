using Microsoft.Xna.Framework;
using OvermorrowMod.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MeteorStill : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool CanDamage() => false;

        private int timer = 0;
        private int PosCheck = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PufferFish");
            Main.projFrames[base.projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.sentry = true;
            projectile.width = 26;
            projectile.height = 26;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 80000;
        }

        public override void AI()
        {



            Vector2 origin = projectile.Center + new Vector2(-7f, -7f);
            float radius = 210;
            int numLocations = 30;
            for (int i = 0; i < 30; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, -5f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                int dust = Dust.NewDust(position, 2, 2, DustID.InfernoFork, dustvelocity.X, dustvelocity.Y, 0, default, 1.1f);
                Main.dust[dust].noGravity = true;
            }



            Player player = Main.player[projectile.owner];
            player.UpdateMaxTurrets();
            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<MeteorBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<MeteorBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion


            PosCheck++;

            projectile.position.X = Main.player[projectile.owner].Center.X - 13;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 100;
            projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation();

            if (Main.player[projectile.owner].channel)
            {
                timer++;
                if (timer == 17)
                {
                    int Random = Main.rand.Next(-15, 16);
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(7f));
                    float speed = 0;
                    Main.PlaySound(SoundID.Item, projectile.position, 20);
                    if (Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<MeteorBall>(), projectile.damage, 1f, projectile.owner, 0f);
                    }
                    else
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<MeteorBall2>(), projectile.damage, 1f, projectile.owner, 0f);
                    }
                    timer = 0;
                }
            }

            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}

