using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Players;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Pets
{
    public class Atom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Atom");
            Main.projFrames[Projectile.type] = 8;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 20;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft *= 5;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 1, 0.6f, 0.8f);

            Player player = Main.player[Projectile.owner];
            OvermorrowModPlayer modPlayer = player.GetModPlayer<OvermorrowModPlayer>();
            if (!player.active)
            {
                Projectile.active = false;
                return;
            }
            if (player.dead)
            {
                modPlayer.atomBuff = false;
            }
            if (modPlayer.atomBuff)
            {
                Projectile.timeLeft = 2;
            }

            // Loop through the 8 animation frames, spending 3 ticks on each.
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            float num26 = 0.4f;

            num26 = 0.3f;


            Projectile.tileCollide = false;
            int num27 = 100;
            float num28 = 50f;
            float num29 = 400f;
            float num30 = num29 / 2f;
            float num31 = 2000f;
            bool flag6 = false;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float num32 = Main.player[Projectile.owner].position.X + (float)(Main.player[Projectile.owner].width / 2) - vector3.X;
            float num33 = Main.player[Projectile.owner].position.Y + (float)(Main.player[Projectile.owner].height / 2) - vector3.Y;


            num33 += (float)Main.rand.Next(-10, 21);
            num32 += (float)Main.rand.Next(-10, 21);

            num32 += (float)(60 * -Main.player[Projectile.owner].direction);
            num33 -= 60f;

            Vector2 vector4 = new Vector2(num32, num33);

            float num36 = (float)Math.Sqrt(num32 * num32 + num33 * num33);
            float num37 = num36;
            float num38 = 14f;

            num38 = 6f;


            if (num36 < (float)num27 && Main.player[Projectile.owner].velocity.Y == 0f && Projectile.position.Y + (float)Projectile.height <= Main.player[Projectile.owner].position.Y + (float)Main.player[Projectile.owner].height && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }
            if (num36 < num28)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.99f;

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
            if (Projectile.velocity.X < num32)
            {
                Projectile.velocity.X += num26;
                if (num26 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X += num26;
                }
            }
            if (Projectile.velocity.X > num32)
            {
                Projectile.velocity.X -= num26;
                if (num26 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X -= num26;
                }
            }
            if (Projectile.velocity.Y < num33)
            {
                Projectile.velocity.Y += num26;
                if (num26 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y += num26 * 2f;
                }
            }
            if (Projectile.velocity.Y > num33)
            {
                Projectile.velocity.Y -= num26;
                if (num26 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y -= num26 * 2f;
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            if (flag6)
            {
                int num39 = 33;

                for (int k = 0; k < 12; k++)
                {
                    float speedX3 = 1f - Main.rand.NextFloat() * 2f;
                    float speedY3 = 1f - Main.rand.NextFloat() * 2f;
                    int num40 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, num39, speedX3, speedY3);
                    Main.dust[num40].noLight = true;
                    Main.dust[num40].noGravity = true;
                }
                Projectile.Center = Main.player[Projectile.owner].Center;
                Projectile.velocity = Vector2.Zero;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.netUpdate = true;
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