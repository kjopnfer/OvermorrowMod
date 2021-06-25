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
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Granite Ray");
        }

        Vector2 endPoint;
        private int timer = 0;
        private int attacktimer = 0;
        private bool teleporting = false;
        private int TPtimer = 0;
        private const string ChainTexturePath = "OvermorrowMod/NPCs/GraniteMini/GraniteChain";
        Vector2 TargetPos;

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.timeLeft = 2;
            projectile.penetrate = -1;
            projectile.hostile = true;
            projectile.magic = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)projectile.ai[0]];
            endPoint = npc.Center;
            timer++;

            if(timer == 1)
            {
                Vector2 TargetPos = Main.player[projectile.owner].Center; 
                projectile.position = endPoint;
            }
            if(npc.life > 0)
            {
                projectile.timeLeft = 2;
            }

            if (timer > 1 && !teleporting)
            {
                projectile.rotation = (npc.Center - projectile.Center).ToRotation();
                if (Main.player[projectile.owner].Center.X > projectile.position.X)
                {
                    projectile.position.X += 1f;
                }
                if (Main.player[projectile.owner].Center.X < projectile.position.X)
                {
                    projectile.position.X -= 1f;
                }
                if (Main.player[projectile.owner].Center.Y > projectile.position.Y)
                {
                    projectile.position.Y += 1f;
                }
                if (Main.player[projectile.owner].Center.Y < projectile.position.Y)
                {
                    projectile.position.Y -= 1f;
                }
            }

            float between = Vector2.Distance(npc.Center, projectile.Center);
            if(between > 475f)
            {
                TPtimer++;
                if(TPtimer == 55)
                {
                    npc.Center = projectile.Center;
                }

                int Random1 = Main.rand.Next(-70, 70);
                int Random2 = Main.rand.Next(-70, 70);

                float XDustposition = projectile.Center.X + Random1;
                float YDustposition = projectile.Center.Y + Random2;

                Vector2 VDustposition = new Vector2(XDustposition, YDustposition);
                Vector2 Dusttarget = projectile.Center;
                Vector2 Dustdirection = Dusttarget - VDustposition;
                Dustdirection.Normalize();

                Color granitedustc = Color.Blue;
                {
                    int dust = Dust.NewDust(VDustposition, projectile.width, projectile.height, DustID.Fire, 0.0f, 0.0f, 400, granitedustc, 3f);
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

            Vector2 unit = endPoint - projectile.Center; // changing all endpoints it just how you change it, dont change other stuff it wont go well
            float length = unit.Length();
            unit.Normalize();
            for (float k = 0; k <= length; k += 20f)
            {
                Vector2 drawPos = projectile.Center + unit * k - Main.screenPosition;
                Color alpha = Color.Purple * ((255 - projectile.alpha) / 255f);
                spriteBatch.Draw(chainTexture, drawPos, null, alpha, projectile.rotation, new Vector2(2, 2), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }
}
