using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Overmorrow.Projectiles.Melee
{
    class YourMomIsHotter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 84;
            projectile.height = 98;
            projectile.aiStyle = 75;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.ownerHitCheck = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red; //Filler till Grass or someone else does funny sprite
            return new Color(255, 255, 255, 0) * (1f - (float)projectile.alpha / 255f);
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            Vector2 vector = player.RotatedRelativePoint(player.MountedCenter);
            if (++projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.soundDelay--;
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item1, projectile.Center);
                projectile.soundDelay = 12;
            }
            if (Main.myPlayer == projectile.owner)
            {
                if (player.channel && !player.noItems && !player.CCed)
                {
                    float num33 = 1f;
                    if (player.inventory[player.selectedItem].shoot == projectile.type)
                    {
                        num33 = player.inventory[player.selectedItem].shootSpeed * projectile.scale;
                    }
                    Vector2 vector8 = Main.MouseWorld - vector;
                    vector8.Normalize();
                    if (vector8.HasNaNs())
                    {
                        vector8 = Vector2.UnitX * player.direction;
                    }
                    vector8 *= num33;
                    if (vector8.X != projectile.velocity.X || vector8.Y != projectile.velocity.Y)
                    {
                        projectile.netUpdate = true;
                    }
                    projectile.velocity = vector8;
                }
                else
                {
                    projectile.Kill();
                }
            }
            Vector2 vector9 = projectile.Center + projectile.velocity * 3f;
            Lighting.AddLight(vector9, 0.8f, 0.8f, 0.8f);
            if (Main.rand.Next(3) == 0)
            {
                int num34 = Dust.NewDust(vector9 - projectile.Size / 2f, projectile.width, projectile.height, DustID.WhiteTorch, projectile.velocity.X, projectile.velocity.Y, 100, default(Color), 2f);
                Main.dust[num34].noGravity = true;
                Main.dust[num34].position -= projectile.velocity;
            }
        }

        // Some advanced drawing because the texture image isn't centered or symetrical.
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int startY = frameHeight * projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)((projectile.spriteDirection == 1) ? (sourceRectangle.Width - 40) : 40);

            Color drawColor = projectile.GetAlpha(lightColor);
            Main.spriteBatch.Draw(texture,
            projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY),
            sourceRectangle, drawColor, projectile.rotation, origin, projectile.scale, spriteEffects, 0f);

            return false;
        }
    }
}