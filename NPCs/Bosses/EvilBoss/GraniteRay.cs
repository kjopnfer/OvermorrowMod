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


        private int UnholyTimer = 0;
        private int UnholyAttTimer = 0;
        int RandomAttackTime = Main.rand.Next(0, 21);


        private int LaserTimer = 0;
        private int attacktimer = 0;


        private int CircleAttTimer = 0;
        private int CircleLaserTimer = 0;

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
                        int randomX = Main.rand.Next(-3, 4);
                        int randomY = Main.rand.Next(-3, 4);
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
                    RandomAtt = Main.rand.Next(0, 3);
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
                }

                if(CircleAttTimer == 600)  
                {
                    RandomAtt = Main.rand.Next(0, 3);
                    CircleAttTimer = 0;
                    CircleLaserTimer = 0;
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

                if(UnholyAttTimer == 10 && UnholyTimer < 541)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, ModContent.ProjectileType<UnholyLight>(), projectile.damage + 15, 1f, projectile.owner, 0f);
                    UnholyAttTimer = 0;
                }

                if(UnholyTimer > 490 && UnholyTimer < 541)
                {
                    projectile.melee = true;
                }
                else
                {
                    projectile.melee = false;
                }

                if(UnholyTimer > 540)
                {
                    projectile.ranged = true;
                }


                if(UnholyTimer > 590)
                {
                    UnholyTimer = 0;
                    UnholyAttTimer = 0;
                    RandomAtt = Main.rand.Next(0, 3);
                    projectile.ranged = false;
                }
            }


            float between = Vector2.Distance(npc.Center, projectile.Center);
            if(between > 600f)
            {
                TPtimer++;
                if(TPtimer == 55)
                {

                    Vector2 value1 = new Vector2(0f, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 5, value1.Y + 5, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 5, value1.Y + 5, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 5, value1.Y - 5, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 5, value1.Y - 5, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);

                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X + 6, value1.Y, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X - 6, value1.Y, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y + 6, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value1.X, value1.Y - 6, mod.ProjectileType("GraniteBolt"), npc.damage + 15, 1f, projectile.owner, 0f);

                    npc.Center = projectile.Center;
                    otherTPtimer = 0;
                }
                Main.PlaySound(SoundID.Item8, projectile.Center);
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
                teleporting = false;
                TPtimer = 0;
                otherTPtimer++;
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

                spriteBatch.Draw(chainTexture, drawPos, null, color, projectile.rotation, new Vector2(6, 6), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
