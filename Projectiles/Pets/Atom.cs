using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Pets
{
    public class Atom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atom");
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 20;
            projectile.penetrate = -1;
            projectile.netImportant = true;
            projectile.timeLeft *= 5;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.scale = 1f;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1, 0.6f, 0.8f);

            Player player = Main.player[projectile.owner];
            OvermorrowModPlayer modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.atomBuff = false;
            }
            if (modPlayer.atomBuff)
            {
                projectile.timeLeft = 2;
            }

            // Loop through the 8 animation frames, spending 3 ticks on each.
            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }

            float num26 = 0.4f;

            num26 = 0.3f;


            projectile.tileCollide = false;
            int num27 = 100;
            float num28 = 50f;
            float num29 = 400f;
            float num30 = num29 / 2f;
            float num31 = 2000f;
            bool flag6 = false;
            Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num32 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector3.X;
            float num33 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector3.Y;


            num33 += (float)Main.rand.Next(-10, 21);
            num32 += (float)Main.rand.Next(-10, 21);

            num32 += (float)(60 * -Main.player[projectile.owner].direction);
            num33 -= 60f;

            Vector2 vector4 = new Vector2(num32, num33);

            float num36 = (float)Math.Sqrt(num32 * num32 + num33 * num33);
            float num37 = num36;
            float num38 = 14f;

            num38 = 6f;


            if (num36 < (float)num27 && Main.player[projectile.owner].velocity.Y == 0f && projectile.position.Y + (float)projectile.height <= Main.player[projectile.owner].position.Y + (float)Main.player[projectile.owner].height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (num36 < num28)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;

                }
                num26 = 0.01f;
            }
            else
            {

                if (num36 < 100f)
                {
                    num26 = 0.1f;
                }
                if (num36 > 300f)
                {
                    num26 = 0.4f;
                }
                if (num36 > num31)
                {
                    flag6 = true;
                }

                num36 = num38 / num36;
                num32 *= num36;
                num33 *= num36;
            }
            if (projectile.velocity.X < num32)
            {
                projectile.velocity.X += num26;
                if (num26 > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += num26;
                }
            }
            if (projectile.velocity.X > num32)
            {
                projectile.velocity.X -= num26;
                if (num26 > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= num26;
                }
            }
            if (projectile.velocity.Y < num33)
            {
                projectile.velocity.Y += num26;
                if (num26 > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += num26 * 2f;
                }
            }
            if (projectile.velocity.Y > num33)
            {
                projectile.velocity.Y -= num26;
                if (num26 > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= num26 * 2f;
                }
            }

            projectile.rotation = projectile.velocity.X * 0.05f;
            if (flag6)
            {
                int num39 = 33;

                for (int k = 0; k < 12; k++)
                {
                    float speedX3 = 1f - Main.rand.NextFloat() * 2f;
                    float speedY3 = 1f - Main.rand.NextFloat() * 2f;
                    int num40 = Dust.NewDust(projectile.position, projectile.width, projectile.height, num39, speedX3, speedY3);
                    Main.dust[num40].noLight = true;
                    Main.dust[num40].noGravity = true;
                }
                projectile.Center = Main.player[projectile.owner].Center;
                projectile.velocity = Vector2.Zero;
                if (Main.myPlayer == projectile.owner)
                {
                    projectile.netUpdate = true;
                }
            }

            return;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}