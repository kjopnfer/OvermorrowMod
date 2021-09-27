using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class BloodBall : ModProjectile
    {
        private float CircleArr = 1;
        private float length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("BloodBall");
            Main.projFrames[base.projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = 1;
            projectile.hostile = true;
            projectile.light = 5f;
            projectile.friendly = false;
            projectile.alpha = 40;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 100;
        }

        public override void AI()
        {
            projectile.damage = 40;
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            timer++;
                if (timer == 1)
                {
                    length = Vector2.Distance(Main.player[projectile.owner].Center, parentProjectile.Center);
                    if(Main.player[projectile.owner].Center.X > parentProjectile.Center.X)
                    {
                        CircleArr = 90;
                    }
                    else
                    {
                        CircleArr = 270;
                    }

                }
                

                double deg = (double)CircleArr; //The degrees, you can multiply projectile.ai[1] to make it orbit faster, may be choppy depending on the value
                double rad = deg * (Math.PI / 180); //Convert degrees to radian

                projectile.position.X = length * (float)Math.Cos(rad) + parentProjectile.Center.X - projectile.width / 2;
                projectile.position.Y = length * (float)Math.Sin(rad) + parentProjectile.Center.Y - projectile.height / 2;

                if(length < 250)
                {
                    CircleArr += length / 100f;
                }
                else
                {
                    CircleArr += length / 165f;
                }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Texture2D texture = mod.GetTexture("NPCs/Bosses/EvilBoss/BloodBall");

            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin;
                Color color = projectile.GetAlpha(Color.White) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override void Kill(int timeLeft)
        {
                Vector2 position = projectile.Center;
                Main.PlaySound(SoundID.Item14, (int)position.X, (int)position.Y);
                Main.PlaySound(SoundID.NPCHit, projectile.Center, 1);
                int radius = 5;     //this is the explosion radius, the highter is the value the bigger is the explosion

                for (int x = -radius; x <= radius; x++)
                {
                    for (int y = -radius; y <= radius; y++)
                    {

                        if (Math.Sqrt(x * x + y * y) <= radius + 0.5)   //this make so the explosion radius is a circle
                        {
                            Color alpha = Color.Red;
                            Dust.NewDust(position, 5, 5, DustID.Enchanted_Gold, 0.0f, 0.0f, 120, alpha, 1f);
                        }
                    }
                }
        }
    }
}
