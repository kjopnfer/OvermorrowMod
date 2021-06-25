using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System;
using Terraria.ID;

namespace OvermorrowMod.NPCs.GraniteMini
{
    public class GraniteRay : ModProjectile
    {
        public override bool CanDamage() => false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Ray");
        }

        Vector2 endPoint;
        private int projshot = 0;
        private int timer = 0;
        private int attacktimer = 0;
        private bool teleporting = false;
        private int TPtimer = 0;
        private int otherTPtimer = 0;
        float speed = 0f;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/GraniteMini/GraniteChain";
        Vector2 LaserPos;
        Vector2 TargetPos;

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
            endPoint = npc.Center;
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
                projectile.rotation = (npc.Center - projectile.Center).ToRotation();
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


            if (!teleporting)
            {
                attacktimer++;
                if(attacktimer > 87 && projshot == 0)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 6f;
                    int type = mod.ProjectileType("GranLaser");
                    int damage = npc.damage + 5;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    attacktimer = 0;
                    projshot = 1;
                }
                if (attacktimer > 87 && projshot == 1)
                {
                    Vector2 position = projectile.Center;
                    Vector2 targetPosition = Main.player[projectile.owner].Center;
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed = 6f;
                    int type = mod.ProjectileType("GranLaser");
                    int damage = npc.damage;
                    Projectile.NewProjectile(position, direction * speed, type, damage, 0f, Main.myPlayer);
                    attacktimer = 0;
                    projshot = 0;
                }
            }


            float between = Vector2.Distance(npc.Center, projectile.Center);
            if(between > 565f || otherTPtimer > 650)
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
