using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs.Summon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class PufferFish : ModProjectile
    {
        int righttimer = 0;
        int lefttimer = 0;
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
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.sentry = true;
            projectile.width = 44;
            projectile.height = 48;
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
                player.ClearBuff(ModContent.BuffType<PufferBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<PufferBuff>()))
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
                if (timer == 10)
                {
                    int Random = Main.rand.Next(-15, 16);
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(7f));
                    float speed = 15.5f + (Random * 0.10f);
                    Main.PlaySound(SoundID.Item, projectile.position, 85);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, newpoint2.X * speed, newpoint2.Y * speed, ModContent.ProjectileType<PuffBubble>(), projectile.damage, 1f, projectile.owner, 0f);
                    timer = 0;
                }
            }

            if (Main.MouseWorld.X > projectile.Center.X)
            {
                righttimer = 0;
                lefttimer++;
                if (lefttimer == 1)
                {
                    projectile.frame = 2;
                }
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame > 1)
                    {
                        projectile.frame = 0;
                    }
                }
            }
            else
            {
                lefttimer = 0;
                righttimer++;
                if (righttimer == 1)
                {
                    projectile.frame = 0;
                }
                if (++projectile.frameCounter >= 4)
                {
                    projectile.frameCounter = 0;
                    if (++projectile.frame > 3)
                    {
                        projectile.frame = 2;
                    }
                }
            }
        }
    }
}

