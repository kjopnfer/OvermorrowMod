using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class GraniteRay : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Ray");
        }

        Vector2 endPoint;
        private int projshot = 0;
        private int timer = 0;
        private bool teleporting = false;
        private int TPtimer = 0;
        int RandomAtt = Main.rand.Next(2, 3);
        private int otherTPtimer = 0;
        float speed = 0f;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/Bosses/EvilBoss/GraniteChain";
        Vector2 LaserPos;
        Vector2 TargetPos;
        private int eggcooldown = 0;

        private int UnholyTimer = 0;
        private int UnholyAttTimer = 0;
        int RandomAttackTime = Main.rand.Next(0, 21);


        private int LaserTimer = 0;
        private int attacktimer = 0;


        private int PixieTimer = 0;
        private int RoundaboutTimer = 0;

        private int CircleAttTimer = 0;
        private int CircleLaserTimer = 0;


        private int SinTimer = 0;
        private int SinTimerTimer = 0;

        private int BloodBallTimer = 0;
        private int BloodAttTimer = 0;



        private int Stopper0 = 0;
        private int Stopper1 = 0;
        private int Stopper2 = 0;
        private int Stopper3 = 0;
        private int Stopper4 = 0;
        private int Stopper5 = 0;



        float CircleArr = 0f;
        float CircleArr2 = 0f;
        float CircleArr3 = 0f;
        float CircleArr4 = 0f;
        float CircleArr5 = 0f;


        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = 2;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)projectile.ai[0]];
            endPoint = npc.Center + new Vector2(- 5, -25);
            timer++;


            if (timer == 1)
            {
                Vector2 TargetPos = Main.player[projectile.owner].Center; 
                projectile.position = endPoint;
            }
            if(npc.life > 0)
            {
                projectile.timeLeft = 2;
            }

            float betweenPLY = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
            if (betweenPLY > 300)
            {
                speed = 1f;
                if(projectile.velocity.X > 7)
                {
                    projectile.velocity.X = 7;
                }
                if (projectile.velocity.X < -7)
                {
                    projectile.velocity.X = -7;
                }

                if (projectile.velocity.Y > 7)
                {
                    projectile.velocity.Y = 7;
                }
                if (projectile.velocity.Y < -7)
                {
                    projectile.velocity.Y = -7;
                }
            }
            else
            {
                speed = 0.55f;
                if (projectile.velocity.X > 5)
                {
                    projectile.velocity.X = 5;
                }
                if (projectile.velocity.X < -5)
                {
                    projectile.velocity.X = -5;
                }

                if (projectile.velocity.Y > 5)
                {
                    projectile.velocity.Y = 5;
                }
                if (projectile.velocity.Y < -5)
                {
                    projectile.velocity.Y = -5;
                }
            }

            if (timer > 1 && !teleporting)
            {
                projectile.rotation = (endPoint - projectile.Center).ToRotation();
                if (Main.player[projectile.owner].Center.X > projectile.position.X)
                {
                    projectile.velocity.X += speed;
                }
                if (Main.player[projectile.owner].Center.X < projectile.position.X)
                {
                    projectile.velocity.X -= speed;
                }
                if (Main.player[projectile.owner].Center.Y > projectile.position.Y)
                {
                    projectile.velocity.Y += speed;
                }
                if (Main.player[projectile.owner].Center.Y < projectile.position.Y)
                {
                    projectile.velocity.Y -= speed;
                }
            }



            Stopper0--;
            if (Stopper0 > 1 && RandomAtt == 0)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper0 = -10;
            }

            Stopper1--;
            if (Stopper1 > 1 && RandomAtt == 1)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper1 = -10;
            }

            Stopper2--;
            if (Stopper2 > 1 && RandomAtt == 2)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper2 = -10;
            }

            Stopper3--;
            if (Stopper3 > 1 && RandomAtt == 3)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper3 = -10;
            }

            Stopper4--;
            if (Stopper4 > 1 && RandomAtt == 4)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper4 = -10;
            }

            Stopper5--;
            if (Stopper5 > 1 && RandomAtt == 5)
            {
                RandomAtt = Main.rand.Next(0, 6);
                Stopper5 = -10;
            }

            if (RandomAtt == 0)
            {
                LaserTimer++;
                attacktimer++;





                if(attacktimer < 51)
                {
                    int Random1 = Main.rand.Next(-70, 70);
                    int Random2 = Main.rand.Next(-70, 70);

                    float XDustposition = projectile.Center.X + Random1 - 16;
                    float YDustposition = projectile.Center.Y + Random2 - 16;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = projectile.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc = Color.White;
                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 185, 0.0f, 0.0f, 10, granitedustc, 2f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 3f;
                        Main.dust[dust].velocity = Dustdirection * 3f;
                    }
                }
                if (attacktimer > 50 && attacktimer < 75)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 3f;
                    npc.velocity.X = direction.X * speed;
                    npc.velocity.Y = direction.Y * speed;
                }
                int numberProjectiles = 3;
                for (int i = 0; i < numberProjectiles; i++)
                {
                    if(attacktimer > 55 && attacktimer < 60)
                    {
                        int randomX = Main.rand.Next(-1, 2);
                        int randomY = Main.rand.Next(-1, 2);
                        Vector2 position = npc.Center;
                        Vector2 targetPosition = Main.player[npc.target].Center;
                        Vector2 direction = targetPosition - position;
                        direction.Normalize();
                        float speed = 1.5f;
                        int damagebull = npc.damage;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, direction.X + randomX, direction.Y + randomY * speed, mod.ProjectileType("CrystalBulletNormal"), damagebull, 0f, Main.myPlayer);
                        Main.PlaySound(SoundID.Item36, npc.position);
                    }
                }

                if (attacktimer > 75 && attacktimer < 90)
                {
                    npc.velocity.X = npc.velocity.X / 2;
                    npc.velocity.Y = npc.velocity.Y / 2;
                }

                if (attacktimer == 90)
                {
                    npc.velocity.X = 0;
                    npc.velocity.Y = 0;
                }

                if(attacktimer == 100)  
                {
                    RandomAtt = Main.rand.Next(0, 6);
                    attacktimer = 0;
                    LaserTimer = 0;
                }
            }




            if (RandomAtt == 1)
            {
                CircleAttTimer++;
                if(CircleAttTimer > 199)
                {
                    CircleLaserTimer++;
                }

                if(CircleLaserTimer == 10 && CircleAttTimer < 500)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<LightningTest>(), 75, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    CircleLaserTimer = 0;
                }




                if(CircleAttTimer < 199)
                {

                    int Random1 = Main.rand.Next(-370, 370);
                    int Random2 = Main.rand.Next(-370, 370);

                    float XDustposition = projectile.Center.X + Random1 - 16;
                    float YDustposition = projectile.Center.Y + Random2 - 16;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = projectile.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    Color granitedustc1 = Color.Purple;
                    Color granitedustc2 = Color.Red;
                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 51, 0.0f, 0.0f, 10, granitedustc1, 4f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * -2;
                        Main.dust[dust].velocity = Dustdirection * -2;
                    }
                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 43, 0.0f, 0.0f, 10, granitedustc2, 4f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2;
                        Main.dust[dust].velocity = Dustdirection * 2;
                    }
                }

                float OutsideRing = Vector2.Distance(Main.player[projectile.owner].Center, projectile.Center);
                if(OutsideRing > 405f && CircleAttTimer > 199 && CircleAttTimer < 500)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 10f;
                    int type = mod.ProjectileType("Xbolt");
                    int damage = 75;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 1f, projectile.owner, 0f);
                }

                if(CircleAttTimer == 1)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<AmethystLW1>(), 1, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                }
                
                if(CircleAttTimer == 200)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay1>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay2>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay3>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay4>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay5>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay6>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay7>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<EvilRay8>(), 50, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                }
                if(CircleAttTimer == 600)  
                {
                    RandomAtt = Main.rand.Next(0, 6);
                    CircleAttTimer = 0;
                    CircleLaserTimer = 0;
                    Stopper1 = 50;
                }
            }



            if (RandomAtt == 2)
            {
                UnholyTimer++;
                UnholyAttTimer++;
                if(UnholyTimer == 1)
                {
                    RandomAttackTime = Main.rand.Next(0, 21);
                }

                if(UnholyAttTimer == 5 && UnholyTimer < 271)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<UnholyLight>(), projectile.damage + 15, 1f, projectile.owner, 0f);
                    UnholyAttTimer = 0;
                }

                if(UnholyTimer > 245 && UnholyTimer < 271)
                {
                    projectile.melee = true;
                }
                else
                {
                    projectile.melee = false;
                }

                if(UnholyTimer > 270)
                {
                    projectile.ranged = true;
                }


                if(UnholyTimer > 300)
                {
                    UnholyTimer = 0;
                    UnholyAttTimer = 0;
                    RandomAtt = Main.rand.Next(0, 6);
                    projectile.ranged = false;
                    Stopper2 = 50;
                }
            }





            if (RandomAtt == 3)
            {
                RoundaboutTimer++;
                PixieTimer++;
                int Rot = 0;
                if(PixieTimer < 50)
                {
                    int Random1 = Main.rand.Next(-70, 0);
                    int Random2 = Main.rand.Next(0, 70);
                    int Random3 = Main.rand.Next(-70, 70);

                    float XDustposition1 = projectile.Center.X + Random1 - 16;
                    float XDustposition2 = projectile.Center.X + Random2 - 16;
                    float YDustposition = projectile.Center.Y + Random3 - 16;
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;

                    Vector2 VDustposition = new Vector2(XDustposition1, YDustposition);
                    Vector2 Dusttarget = projectile.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;

                    Vector2 VDustposition2 = new Vector2(XDustposition2, YDustposition);
                    Vector2 Dusttarget2 = projectile.Center;
                    Vector2 Dustdirection2 = Dusttarget2 - VDustposition;

                    Dustdirection.Normalize();
                    Dustdirection2.Normalize();

                    Color Corp = Color.Purple;
                    Color Crim = Color.Red;
                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 16, 0.0f, 0.0f, 10, Crim, 2f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 3f;
                        Main.dust[dust].velocity = Dustdirection * 3f;
                    }
                    {
                        int dust = Dust.NewDust(VDustposition2, projectile.width, projectile.height, 16, 0.0f, 0.0f, 10, Corp, 2f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection2 * -3f;
                        Main.dust[dust].velocity = Dustdirection2 * -3f;
                    }
                }

                float RotDistance = Vector2.Distance(npc.Center, projectile.Center);
                float RotDistance2 = Vector2.Distance(projectile.Center, npc.Center);

                if(PixieTimer < 52)
                {
                    CircleArr = (npc.Center - projectile.Center).ToRotation();
                }

                if(PixieTimer > 50 && PixieTimer < 100)
                {
                    npc.position.X = RotDistance2 * (float)Math.Cos(CircleArr) + projectile.Center.X - projectile.width / 2;
                    npc.position.Y = RotDistance2 * (float)Math.Sin(CircleArr) + projectile.Center.Y - projectile.height / 2;
                    CircleArr += (float)((2 * Math.PI) / (Math.PI * 2 * 220 / 10)); // 200 is the speed, god only knows what dividing by 10 does
                    
                }

                if(PixieTimer < 102)
                {
                    CircleArr2 = (projectile.Center - npc.Center).ToRotation();
                }

                if(PixieTimer > 100 && PixieTimer < 150)
                {
                    projectile.position.X = RotDistance2 * (float)Math.Cos(CircleArr2) + npc.Center.X - npc.width / 2;
                    projectile.position.Y = RotDistance2 * (float)Math.Sin(CircleArr2) + npc.Center.Y - npc.height / 2;
                    CircleArr2 -= (float)((2 * Math.PI) / (Math.PI * 2 * 220 / 10)); // 200 is the speed, god only knows what dividing by 10 does
                }

                if(PixieTimer < 152)
                {
                    CircleArr3 = (npc.Center - projectile.Center).ToRotation();
                }

                if(PixieTimer > 150 && PixieTimer < 200)
                {
                    npc.position.X = RotDistance2 * (float)Math.Cos(CircleArr3) + projectile.Center.X - projectile.width / 2;
                    npc.position.Y = RotDistance2 * (float)Math.Sin(CircleArr3) + projectile.Center.Y - projectile.height / 2;
                    CircleArr3 += (float)((2 * Math.PI) / (Math.PI * 2 * 220 / 10)); // 200 is the speed, god only knows what dividing by 10 does
                }



                if(PixieTimer == 225)
                {
                    Stopper3 = 700;
                    PixieTimer = 0;
                    RandomAtt = Main.rand.Next(0, 6);
                }
            }




            if (RandomAtt == 4)
            {
                SinTimer++;
                if(SinTimer < 50)
                {
                    int Random1 = Main.rand.Next(-50, 50);
                    int Random2 = Main.rand.Next(-50, 50);

                    float XDustposition = projectile.Center.X + Random1 - 16;
                    float YDustposition = projectile.Center.Y + Random2 - 16;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = projectile.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();

                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 14, 0.0f, 0.0f, 10, new Color(), 3f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2.5f;
                        Main.dust[dust].velocity = Dustdirection * 2.5f;
                    }
                }
                if(SinTimer == 50)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 3f;
                    int dmg = 30;
                    Vector2 perturbedSpeed1 = new Vector2(direction.X, direction.Y);
                    Vector2 perturbedSpeed2 = new Vector2(perturbedSpeed1.X, perturbedSpeed1.Y).RotatedBy(MathHelper.ToRadians(45f));
                    Vector2 perturbedSpeed3 = new Vector2(perturbedSpeed1.X, perturbedSpeed1.Y).RotatedBy(MathHelper.ToRadians(22.5f));
                    Vector2 perturbedSpeed5 = new Vector2(perturbedSpeed1.X, perturbedSpeed1.Y).RotatedBy(MathHelper.ToRadians(-22.5f));
                    Vector2 perturbedSpeed6 = new Vector2(perturbedSpeed1.X, perturbedSpeed1.Y).RotatedBy(MathHelper.ToRadians(-45f));

                    Projectile.NewProjectile(projectile.Center, direction * speed * 2.5f, mod.ProjectileType("Envy"), dmg, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed2 * speed * 2.5f, mod.ProjectileType("Envy"), dmg, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed3 * speed * 2.5f, mod.ProjectileType("Envy"), dmg, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed5 * speed * 2.5f, mod.ProjectileType("Envy"), dmg, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                    Projectile.NewProjectile(projectile.Center, perturbedSpeed6 * speed * 2.5f, mod.ProjectileType("Envy"), dmg, 0f, Main.myPlayer, projectile.whoAmI, Main.myPlayer);
                }

                if(SinTimer == 100)
                {
                    SinTimer = 0;
                    RandomAtt = Main.rand.Next(0, 6);
                }
            }


            if (RandomAtt == 5)
            {
                BloodBallTimer++;
                BloodAttTimer++;

                if (BloodBallTimer > 1)
                {
                    int Random1 = Main.rand.Next(-50, 50);
                    int Random2 = Main.rand.Next(-50, 50);

                    float XDustposition = projectile.Center.X + Random1 - 16;
                    float YDustposition = projectile.Center.Y + Random2 - 16;
                    Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                    Vector2 Dusttarget = projectile.Center;
                    Vector2 Dustdirection = Dusttarget - VDustposition;
                    Dustdirection.Normalize();
                    Color Crim = Color.Red;

                    {
                        int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 14, 0.0f, 0.0f, 10, Crim, 3f);
                        Main.dust[dust].noGravity = true;
                        Vector2 velocity = Dustdirection * 2.5f;
                        Main.dust[dust].velocity = Dustdirection * 2.5f;
                    }
                }

                    if (BloodAttTimer == 45)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<BloodBall>(), projectile.damage + 15, 1f, projectile.owner, 0f);
                }
                if (BloodAttTimer == 90)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<BloodBall2>(), projectile.damage + 15, 1f, projectile.owner, 0f);
                    BloodAttTimer = 0;
                }

                if (BloodBallTimer > 45)
                {
                    projectile.velocity.X = 0f;
                    projectile.velocity.Y = 0f;
                }


                if (BloodBallTimer > 540)
                {
                    Stopper5 = 100;
                    BloodBallTimer = 0;
                    BloodAttTimer = 0;
                    RandomAtt = Main.rand.Next(0, 5);
                }
            }




            float between = Vector2.Distance(npc.Center, projectile.Center);
            if(between > 600f)
            {
                otherTPtimer++;
                if(otherTPtimer == 1)
                {
                    TPtimer = 50;
                }
                int Random1 = Main.rand.Next(-70, 70);
                int Random2 = Main.rand.Next(-70, 70);

                float XDustposition = projectile.Center.X + Random1 - 16;
                float YDustposition = projectile.Center.Y + Random2 - 16;
                projectile.velocity.X = 0f;
                projectile.velocity.Y = 0f;
                Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                Vector2 Dusttarget = projectile.Center;
                Vector2 Dustdirection = Dusttarget - VDustposition;
                Dustdirection.Normalize();

                Color granitedustc = Color.Blue;
                {
                    int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, 56, 0.0f, 0.0f, 400, granitedustc, 2.4f);
                    Main.dust[dust].noGravity = true;
                    Vector2 velocity = Dustdirection;
                    Main.dust[dust].velocity = Dustdirection;
                }
                teleporting = true;
            }
            else
            {
                npc.velocity.X *= 0.5f;
                npc.velocity.Y *= 0.5f;
                teleporting = false;
                TPtimer--;
                otherTPtimer = 0;
            }
            if(TPtimer < 0)
            {
                TPtimer = 0;
            }

                if(TPtimer > 10)
                {
                    Vector2 position = npc.Center;
                    Vector2 targetPosition = projectile.Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 17f;
                    npc.velocity.X = direction.X * speed;
                    npc.velocity.Y = direction.Y * speed;
                    otherTPtimer = 0;
                }


        }



        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, endPoint, 4f, ref point);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D chainTexture = ModContent.GetTexture(ChainTexturePath);
            projectile.alpha = 0;
            projectile.velocity = Vector2.Zero;
            var drawPosition = projectile.Center;

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 5f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Color.LightBlue * ((255 - projectile.alpha) / 255f);
                Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));

                spriteBatch.Draw(chainTexture, drawPos, null, color, (endPoint - projectile.Center).ToRotation(), new Vector2(6, 6), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
