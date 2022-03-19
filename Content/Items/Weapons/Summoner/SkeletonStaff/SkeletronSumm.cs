using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Summoner.SkeletonStaff
{
    public class SkeletronSumm : ModProjectile
    {
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool CanDamage() => false;

        private int timer = 0;
        private int PosCheck = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);


        private bool up = true;
        private bool down = false;
        int Flamerot = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("PufferFish");
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.sentry = true;
            projectile.width = 52;
            projectile.height = 60;
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


            Player player = Main.player[projectile.owner];
            player.UpdateMaxTurrets();
            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<SkullBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<SkullBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion


            PosCheck++;

            projectile.position.X = Main.player[projectile.owner].Center.X - 22;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 100;
            projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation();


            projectile.rotation = (Main.MouseWorld - projectile.Center).ToRotation();

            projectile.spriteDirection = -1;

            if (Main.player[projectile.owner].channel)
            {
                timer++;
                if (timer == 5)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.ToRadians(Flamerot));
                    float speed = 15.5f;
                    Main.PlaySound(SoundID.Item, projectile.position, 34);
                    Projectile.NewProjectile(projectile.Center + new Vector2(0, 10).RotatedBy(MathHelper.ToRadians(projectile.rotation)), newpoint2 * speed, ModContent.ProjectileType<SpritFlame>(), projectile.damage, 1f, projectile.owner, 0f);

                    if (up)
                    {
                        Flamerot += 3;
                    }
                    if (down)
                    {
                        Flamerot -= 3;
                    }

                    if (Flamerot > 20)
                    {
                        up = false;
                        down = true;
                    }
                    if (Flamerot < -20)
                    {
                        up = true;
                        down = false;
                    }
                    timer = 0;

                }
            }

            if (Main.MouseWorld.X > projectile.Center.X && !Main.player[projectile.owner].channel)
            {
                projectile.frame = 1;
            }
            if (Main.MouseWorld.X > projectile.Center.X && Main.player[projectile.owner].channel)
            {
                projectile.frame = 0;
            }

            if (Main.MouseWorld.X < projectile.Center.X && !Main.player[projectile.owner].channel)
            {
                projectile.frame = 2;
            }
            if (Main.MouseWorld.X < projectile.Center.X && Main.player[projectile.owner].channel)
            {
                projectile.frame = 3;
            }
        }
    }
}

