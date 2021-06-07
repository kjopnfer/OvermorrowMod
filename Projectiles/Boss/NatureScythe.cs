using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class NatureScythe : ModProjectile
    {
        private Vector2 storeVelocity;
        private bool reverseDirection = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nature Scythe");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;    //The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;        //The recording mode, this tracks rotation
        }

        public override void SetDefaults()
        {
            projectile.width = 106;
            projectile.height = 124;
            projectile.penetrate = -1;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 540;
        }

        public override void AI()
        {
            projectile.rotation += .55f;

            if (projectile.ai[0] == 0)
            {
                storeVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
            }

            projectile.ai[0]++;
            projectile.ai[1]++;

            if (!reverseDirection)
            {
                if (projectile.ai[0] == 60) // Begin accelerating
                {
                    projectile.velocity = storeVelocity;
                }

                if (projectile.ai[0] > 66)
                {
                    if (projectile.ai[0] % 15 == 0) // Increase velocity by 25% every 6 ticks
                    {
                        projectile.velocity *= 1.25f;
                    }
                }

                if (projectile.ai[0] == 140)
                {
                    reverseDirection = true;
                    projectile.velocity = Vector2.Zero;
                }
            }
            else
            {
                if(projectile.ai[0] == 161)
                {
                    // Reverse the direction
                    projectile.velocity = storeVelocity * -1;
                }

                if (projectile.ai[0] > 226)
                {
                    if (projectile.ai[0] % 15 == 0) // Increase velocity by 25% every 6 ticks
                    {
                        projectile.velocity *= 1.25f;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            //Redraw the projectile with the color not influenced by light
            /*Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.GreenYellow) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }


            // this gets the npc's frame
            Vector2 vector47 = drawOrigin;
            Color color55 = Color.White; // This is just white lol
            float amount10 = 0f; // I think this controls amount of color
            int num178 = 120; // i think this controls the distance of the pulse, maybe color too, if we make it high: it is weaker
            int num179 = 60; // changing this value makes the pulsing effect rapid when lower, and slower when higher


            // default value
            int num177 = 6; // ok i think this controls the number of afterimage frames
            float num176 = 1f - (float)Math.Cos((projectile.ai[1] - (float)num178) / (float)num179 * ((float)Math.PI * 2f));  // this controls pulsing effect
            num176 /= 3f;
            float scaleFactor10 = 10f; // Change scale factor of the pulsing effect and how far it draws outwards

            Color color47 = Color.Lerp(Color.White, Color.Blue, 0.5f);
            color55 = Color.Cyan;
            amount10 = 1f;

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);


            // ok this is the pulsing effect drawing
            for (int num164 = 1; num164 < num177; num164++)
            {
                // these assign the color of the pulsing
                Color color45 = color47;
                color45 = Color.Lerp(color45, color55, amount10);
                color45 = projectile.GetAlpha(color45);
                color45 *= 1f - num176; // num176 is put in here to effect the pulsing

                // num176 is used here too
                Vector2 vector45 = projectile.Center + Utils.ToRotationVector2((float)num164 / (float)num177 * ((float)Math.PI * 2f) + projectile.rotation) * scaleFactor10 * num176 - Main.screenPosition;
                vector45 -= new Vector2(texture2D16.Width, texture2D16.Height / Main.npcFrameCount[projectile.type]) * projectile.scale / 2f;
                vector45 += vector47 * projectile.scale + new Vector2(0f, 4f + projectile.gfxOffY);

                // the actual drawing of the pulsing effect
                spriteBatch.Draw(texture2D16, vector45, drawRectangle, color45, projectile.rotation, vector47, projectile.scale, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }*/
            Color color26 = Color.GreenYellow;
            Texture2D texture2D16 = mod.GetTexture("Projectiles/Boss/NatureScythe");

            int num154 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y2 = num154 * projectile.frame;
            Rectangle drawRectangle = new Microsoft.Xna.Framework.Rectangle(0, y2, Main.projectileTexture[projectile.type].Width, num154);

            Vector2 origin2 = drawRectangle.Size() / 2f;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                Color color27 = color26;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[projectile.type];
                Vector2 value4 = projectile.oldPos[i];
                float num165 = projectile.oldRot[i];
                Main.spriteBatch.Draw(texture2D16, value4 + projectile.Size / 2f - Main.screenPosition + new Vector2(0, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(drawRectangle), color27, num165, origin2, projectile.scale, SpriteEffects.None, 0f);
            }


            return base.PreDraw(spriteBatch, lightColor);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}