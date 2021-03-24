using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WardenClass;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class BlazePiercerProjectileAlt : PiercingProjectile
    {
        public override string Texture => "OvermorrowMod/Projectiles/Piercing/BlazePiercerProjectile";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Spike");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;

            // Fire particle effects
            if (!projectile.wet) // Check if projectile is not in water
            {
                for (int num453 = 0; num453 < 2; num453++)
                {
                    int num451 = Dust.NewDust(new Vector2(projectile.Center.X, projectile.Center.Y - 10), projectile.width, projectile.height, 6, projectile.velocity.X * 0.2f + (float)(projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, default(Color), 2.5f);
                    Main.dust[num451].noGravity = true;
                    Dust expr_D6EA_cp_0 = Main.dust[num451];
                    expr_D6EA_cp_0.velocity.X = expr_D6EA_cp_0.velocity.X * 2f;
                    Dust expr_D70A_cp_0 = Main.dust[num451];
                    expr_D70A_cp_0.velocity.Y = expr_D70A_cp_0.velocity.Y * 2f;
                }
            }

            // Light effect
            Lighting.AddLight(projectile.Center, 0.5f, 0, 0);

            // Fetch projectile owner
            var player = Main.player[projectile.owner];

            // Kill projectile if the owner is dead
            if (player.dead)
            {
                projectile.Kill();
                return;
            }

            // This prevents the item from being able to be used again prior to this projectile dying
            player.itemAnimation = 5;
            player.itemTime = 5;

            // Here we turn the player and projectile projectiled on the relative positioning of the player and projectile.
            if (projectile.alpha == 0)
            {
                if (projectile.position.X + (projectile.width / 2) > player.position.X + (player.width / 2))
                {
                    player.ChangeDir(1);
                }
                else
                {
                    player.ChangeDir(-1);
                }
            }

            if(projectile.ai[0] == 0f)
            {
                projectile.extraUpdates = 0;
            }
            else
            {
                projectile.extraUpdates = 1;
            }

            Vector2 vector221 = new Vector2(projectile.position.X + projectile.width * 0.5f, projectile.position.Y + projectile.height * 0.5f);
            float num494 = player.position.X + (player.width / 2) - vector221.X;
            float num499 = player.position.Y + (player.height / 2) - vector221.Y;
            float num501 = (float)Math.Sqrt(num494 * num494 + num499 * num499); // Max length?

            // ai[0] == 0 means the projectile has neither hit any tiles yet or reached maxChainLength
            if (projectile.ai[0] == 0f)
            {
                if (num501 > 700f) // Max limit
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }else if (num501 > 350f) // Projectile's max length
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }

                projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;
                projectile.ai[1] += 1f;
                if(projectile.ai[1] > 5f)
                {
                    projectile.alpha = 0;
                }

                if(projectile.ai[1] > 8f)
                {
                    projectile.ai[1] = 8f;
                }

                if (projectile.ai[1] >= 10f)
                {
                    projectile.ai[1] = 15f;
                    projectile.velocity.Y = projectile.velocity.Y + 0.3f;
                }
            } // When ai[0] == 1f, the projectile has either hit a tile or has reached maxChainLength, so now we retract the projectile
            else if (projectile.ai[0] == 1f) 
            {
                projectile.tileCollide = false; // Allows for retraction without collision
                projectile.rotation = (float)Math.Atan2(num499, num494) - 1.57f;
                float num502 = 20f;
                if (num501 < 50f)
                {
                    projectile.Kill();
                }
                num501 = num502 / num501;
                num494 *= num501;
                num499 *= num501;
                projectile.velocity.X = num494;
                projectile.velocity.Y = num499;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var player = Main.player[projectile.owner];
            Vector2 mountedCenter = player.MountedCenter;
            Texture2D chainTexture = mod.GetTexture("Projectiles/Piercing/BlazePiercerChain");

            float num751 = projectile.Center.X;
            float num750 = projectile.Center.Y;
            float num749 = projectile.velocity.X;
            float num748 = projectile.velocity.Y;
            float num747 = (float)Math.Sqrt(num749 * num749 + num748 * num748);
            num747 = 4f / num747;
            if (projectile.ai[0] == 0f)
            {
                num751 -= projectile.velocity.X * num747;
                num750 -= projectile.velocity.Y * num747;
            }
            else
            {
                num751 += projectile.velocity.X * num747;
                num750 += projectile.velocity.Y * num747;
            }
            Vector2 vector73 = new Vector2(num751, num750);
            num749 = mountedCenter.X - vector73.X;
            num748 = mountedCenter.Y - vector73.Y;
            float rotation20 = (float)Math.Atan2(num748, num749) - 1.57f;
            if (projectile.alpha == 0)
            {
                int num741 = -1;
                if (projectile.position.X + (float)(projectile.width / 2) < mountedCenter.X)
                {
                    num741 = 1;
                }
                if (player.direction == 1)
                {
                    player.itemRotation = (float)Math.Atan2(num748 * (float)num741, num749 * (float)num741);
                }
                else
                {
                    player.itemRotation = (float)Math.Atan2(num748 * (float)num741, num749 * (float)num741);
                }
            }
            bool flag17 = true;
            while (flag17)
            {
                float num740 = 0.85f;
                float num739 = (float)Math.Sqrt(num749 * num749 + num748 * num748);
                float num738 = num739;
                if ((double)num739 < (double)chainTexture.Height * 1.5)
                {
                    flag17 = false;
                    continue;
                }
                if (float.IsNaN(num739))
                {
                    flag17 = false;
                    continue;
                }
                num739 = (float)chainTexture.Height * num740 / num739;
                num749 *= num739;
                num748 *= num739;
                vector73.X += num749;
                vector73.Y += num748;
                num749 = mountedCenter.X - vector73.X;
                num748 = mountedCenter.Y - vector73.Y;
                if (num738 > (float)(chainTexture.Height * 2))
                {
                    for (int num734 = 0; num734 < 2; num734++)
                    {
                        float num733 = 0.75f;
                        float num732 = (num734 != 0) ? Math.Abs(player.velocity.Y) : Math.Abs(player.velocity.X);
                        if (num732 > 10f)
                        {
                            num732 = 10f;
                        }
                        num732 /= 10f;
                        num733 *= num732;
                        num732 = num738 / 80f;
                        if (num732 > 1f)
                        {
                            num732 = 1f;
                        }
                        num733 *= num732;
                        if (num733 < 0f)
                        {
                            num733 = 0f;
                        }
                        if (float.IsNaN(num733))
                        {
                            continue;
                        }
                        if (num734 == 0)
                        {
                            if (player.velocity.X < 0f && projectile.Center.X < mountedCenter.X)
                            {
                                num748 *= 1f - num733;
                            }
                            if (player.velocity.X > 0f && projectile.Center.X > mountedCenter.X)
                            {
                                num748 *= 1f - num733;
                            }
                        }
                        else
                        {
                            if (player.velocity.Y < 0f && projectile.Center.Y < mountedCenter.Y)
                            {
                                num749 *= 1f - num733;
                            }
                            if (player.velocity.Y > 0f && projectile.Center.Y > mountedCenter.Y)
                            {
                                num749 *= 1f - num733;
                            }
                        }
                    }
                }
                Color color100 = Lighting.GetColor((int)vector73.X / 16, (int)(vector73.Y / 16f));
                spriteBatch.Draw(chainTexture, new Vector2(vector73.X - Main.screenPosition.X, vector73.Y - Main.screenPosition.Y), new Rectangle(0, 0, chainTexture.Width, chainTexture.Height), color100, rotation20, new Vector2((float)chainTexture.Width * 0.5f, (float)chainTexture.Height * 0.5f), num740, SpriteEffects.None, 0f);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Get the projectile owner
            Player player = Main.player[projectile.owner];

            // Get the class info from the player
            var modPlayer = WardenDamagePlayer.ModPlayer(player);

            if (!projectile.wet) // Check if projectile is not in water
            {
                target.AddBuff(BuffID.OnFire, 300); // Fire Debuff
                // TODO: Add Additional Fire Debuff
            }
            target.immune[projectile.owner] = 3;
        }
    }
}