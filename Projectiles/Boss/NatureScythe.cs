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
            if (projectile.ai[1] < 15)
            { 
                Vector2 origin = projectile.Center;
                float radius = 15;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Vector2 dustvelocity = new Vector2(0f, 15f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));
                    int dust = Dust.NewDust(position, 2, 2, DustID.TerraBlade, dustvelocity.X, dustvelocity.Y, 0, default, 1);
                    Main.dust[dust].noGravity = true;
                }
                projectile.ai[1]++;
            }

            projectile.rotation += .55f;

            if (projectile.ai[0] == 0)
            {
                storeVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
            }

            projectile.ai[0]++;

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