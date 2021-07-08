using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using OvermorrowMod.Buffs.Summon;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MeteorStill : ModProjectile
    {
        int righttimer = 0;
        int lefttimer = 0;
        int colorcooldown = 0;
        readonly int frame = 1;
        Vector2 Rot;
        int Random2 = Main.rand.Next(-15, 12);
        int Random = Main.rand.Next(1, 3);
        public override bool CanDamage() => false;
        private readonly int timer2 = 0;
        private int eyetimer = 0;
        private int timer = 0;
        private int PosCheck = 0;
        private int PosPlay = 0;
        private int Pos = 0;
        private int movement = 0;
        private int NumProj = 0;
        private int movement2 = 0;
        float NPCtargetX = 0;
        float NPCtargetY = 0;
        int mrand = Main.rand.Next(-100, 101);
        int mrand2 = Main.rand.Next(-100, 101);

        private int ShowTime = 0;



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



            ShowTime++;
            if(ShowTime == 1)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<MeteorRangeShow>(), 0, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
            }

            PosCheck++;

            projectile.position.X = Main.player[projectile.owner].Center.X - 13;
            projectile.position.Y = Main.player[projectile.owner].Center.Y - 100;
            projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation();

            if (Main.player[projectile.owner].channel)
            {
                timer++;
                if(timer == 20)
                {
                    int Random = Main.rand.Next(-15, 16);
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.MouseWorld;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    Vector2 newpoint2 = new Vector2(direction.X, direction.Y).RotatedByRandom(MathHelper.ToRadians(7f));
                    float speed = 0;
                    Main.PlaySound(2, projectile.position, 20);
                    if(Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
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

