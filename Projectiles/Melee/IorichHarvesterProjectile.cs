using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Melee
{
    public class IorichHarvesterProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester of Iorich");

            // Afterimage effect
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            //projectile.CloneDefaults(ProjectileID.PaladinsHammerFriendly);
            projectile.width = 36;
            projectile.height = 36;
            //projectile.aiStyle = 3;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 600;
            projectile.tileCollide = true;
            projectile.extraUpdates = 1;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 1f, 0f);
            if (projectile.soundDelay == 0)
            {
                projectile.soundDelay = 8;
                Main.PlaySound(SoundID.Item7, projectile.position);
            }

            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] <= 5)
                {
                    projectile.width = 4;
                    projectile.height = 4;
                }
                else
                {
                    projectile.width = 36;
                    projectile.height = 36;
                }
                projectile.ai[1] += 1f;
                if (projectile.ai[1] >= 30f) // Return back code
                {
                    projectile.ai[0] = 1f;
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
            }
            else
            {
                // Projectile is returning
                projectile.tileCollide = false;
                float num149 = 16f;
                float num150 = 1.2f;
                num149 = 15f;
                num150 = 3f;


                Vector2 vector163 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                float num152 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector163.X;
                float num154 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector163.Y;
                float num155 = (float)Math.Sqrt(num152 * num152 + num154 * num154);

                // Maximum distance
                if (num155 > 3000f)
                {
                    projectile.Kill();
                }
                num155 = num149 / num155;
                num152 *= num155;
                num154 *= num155;
                if (projectile.velocity.X < num152)
                {
                    projectile.velocity.X = projectile.velocity.X + num150;
                    if (projectile.velocity.X < 0f && num152 > 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num150;
                    }
                }
                else if (projectile.velocity.X > num152)
                {
                    projectile.velocity.X = projectile.velocity.X - num150;
                    if (projectile.velocity.X > 0f && num152 < 0f)
                    {
                        projectile.velocity.X = projectile.velocity.X - num150;
                    }
                }
                if (projectile.velocity.Y < num154)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num150;
                    if (projectile.velocity.Y < 0f && num154 > 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num150;
                    }
                }
                else if (projectile.velocity.Y > num154)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num150;
                    if (projectile.velocity.Y > 0f && num154 < 0f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num150;
                    }
                }

                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle rectangle = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
                    Rectangle value99 = new Rectangle((int)Main.player[projectile.owner].position.X, (int)Main.player[projectile.owner].position.Y, Main.player[projectile.owner].width, Main.player[projectile.owner].height);
                    if (rectangle.Intersects(value99))
                    {
                        projectile.Kill();
                    }
                }
            }

            projectile.rotation += 0.35f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(Color.Green) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, new Rectangle?(), color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.ai[0] = 1f;
            projectile.velocity.X = 0f - projectile.velocity.X;
            projectile.velocity.Y = 0f - projectile.velocity.Y;
            projectile.netUpdate = true;
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            return false;
        }
    }
}