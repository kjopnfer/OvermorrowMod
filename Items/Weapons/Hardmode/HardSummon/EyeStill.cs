using OvermorrowMod.Items.Weapons.Hardmode.HardSummon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace OvermorrowMod.Items.Weapons.Hardmode.HardSummon
{
    public class EyeStill : ModProjectile
    {
        private bool returntoply = false;
        private bool charge = false;
        private int chargetimer = 0;
        private int returntimer = 0;
        private int SaveTimer = 0;
        private float savedMouseX = 0;
        private float savedMouseY = 0;
        Vector2 savedMouseVec;
        Vector2 RETrot;

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.minionSlots = 1.0f;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.light = 3f;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 7200;
        }


        public override void AI()
        {



            Player player = Main.player[projectile.owner];

            #region Active check
            if (player.dead || !player.active)
            {
                player.ClearBuff(ModContent.BuffType<EyeStillBuff>());
            }
            if (player.HasBuff(ModContent.BuffType<EyeStillBuff>()))
            {
                projectile.timeLeft = 2;
            }
            #endregion


            if(projectile.velocity.X < 10 && projectile.velocity.X > 0 && charge)
            {
                projectile.velocity.X *= 2f;
            }

            if(projectile.velocity.X > -10 && projectile.velocity.X < 0 && charge)
            {
                projectile.velocity.X *= 2f;
            }

            if(projectile.velocity.Y < 10 && projectile.velocity.Y > 0 && charge)
            {
                projectile.velocity.Y *= 2f;
            }

            if(projectile.velocity.Y > -10 && projectile.velocity.Y < 0 && charge)
            {
                projectile.velocity.Y *= 2f;
            }


            if(charge && !returntoply)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(0f);

            }


            if(!charge && !returntoply)
            {
                projectile.rotation = (Main.MouseWorld - projectile.Center).ToRotation();
            }

            if(returntoply)
            {
                projectile.rotation = (RETrot - projectile.Center).ToRotation();
            }

			if (!charge) 
            {
                savedMouseVec = Main.MouseWorld;
                savedMouseX = Main.MouseWorld.X;
                savedMouseY = Main.MouseWorld.Y;
                projectile.position.X = Main.player[projectile.owner].Center.X - 16;
                projectile.position.Y = Main.player[projectile.owner].Center.Y - 100;
            }

			if (Main.player[projectile.owner].channel && !charge) 
            {
                charge = true; 
            }


            if(charge && returntimer == 0)
            {
                Vector2 position = projectile.Center;
                Vector2 targetPosition = savedMouseVec;
                Vector2 direction = targetPosition - position;
                direction.Normalize();
                projectile.velocity = direction * 18;
                chargetimer++;
                SaveTimer++;
                Main.PlaySound(SoundID.Item, projectile.position, 103);
            }

            if(SaveTimer == 120)
            {
                returntoply = true;
            }

            if(chargetimer > 0 && projectile.velocity.X > 0 && projectile.velocity.Y < 0)
            {
                if(projectile.Center.X > savedMouseX - 17 && projectile.Center.Y < savedMouseY + 25)
                {
                    returntoply = true;
                    chargetimer = -1000;
                }
            }

            if(chargetimer > 0 && projectile.velocity.X > 0 && projectile.velocity.Y > 0)
            {
                if(projectile.Center.X > savedMouseX - 17 && projectile.Center.Y > savedMouseY - 25)
                {
                    returntoply = true;
                    chargetimer = -1000;
                }
            }

            if(chargetimer > 0 && projectile.velocity.X < 0 && projectile.velocity.Y < 0)
            {
                if(projectile.Center.X < savedMouseX + 17 && projectile.Center.Y < savedMouseY + 25)
                {
                    returntoply = true;
                    chargetimer = -1000;
                }
            }

            if(chargetimer > 0 && projectile.velocity.X < 0 && projectile.velocity.Y > 0)
            {
                if(projectile.Center.X < savedMouseX+ 17 && projectile.Center.Y > savedMouseY - 25)
                {
                    returntoply = true;
                    chargetimer = -1000;
                }
            }

            if(returntoply)
            {
                float positionX = projectile.Center.X;
                float positionY = projectile.Center.Y;
                float targetPositionX = Main.player[projectile.owner].Center.X;
                float targetPositionY = Main.player[projectile.owner].Center.Y - 100;
                float directionX = targetPositionX - positionX;
                float directionY = targetPositionY - positionY;
                Vector2 directionRET = new Vector2(directionX, directionY);
                RETrot = new Vector2(targetPositionX, targetPositionY);
                directionRET.Normalize();
                projectile.velocity = directionRET * 13;
                returntimer++;
            }
            

            if(returntimer > 300005)
            {
                charge = false;
                returntoply = false;
                returntimer = 0;
                chargetimer = 0;
            }


            if(returntimer > 0)
            {

                float playerreturnspotX = Main.player[projectile.owner].Center.X;
                float playerreturnspotY = Main.player[projectile.owner].Center.Y;

                if(projectile.velocity.X > 0)
                {
                    
                    if(projectile.Center.X > playerreturnspotX - 27 && projectile.Center.Y > playerreturnspotY - 40 || projectile.Center.X > playerreturnspotX - 27 && projectile.Center.Y < playerreturnspotY + 40)
                    {
                        charge = false;
                        returntoply = false;
                        returntimer = 0;
                        chargetimer = 0;
                    }
                }

                if(projectile.velocity.X < 0)
                {
                    if(projectile.Center.X < playerreturnspotX + 27 && projectile.Center.Y > playerreturnspotY - 40 || projectile.Center.X < playerreturnspotX + 27 && projectile.Center.Y < playerreturnspotY + 40)
                    {
                        charge = false;
                        returntoply = false;
                        returntimer = 0;
                        chargetimer = 0;
                        SaveTimer = 0;
                    }
                }
            }
        }




		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) 
        {
            if(charge && !returntoply) 
                {
                //Redraw the projectile with the color not influenced by light
			    Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			    for (int k = 0; k < projectile.oldPos.Length; k++) {
				    Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				    Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				    spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			    }
            }
			return true;
		}




        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye of Stillhu");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;   
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;   
        }
    }
}