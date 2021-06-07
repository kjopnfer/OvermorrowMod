using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class MarbleArrow : ModProjectile
    {
        private Vector2 storeVelocity;
        private bool target = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magical Arrow");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 3;
            projectile.timeLeft = 500;
            projectile.tileCollide = true;
            projectile.magic = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                Vector2 origin = projectile.Center;
                float radius = 10;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Dust dust = Terraria.Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
                }
                projectile.alpha = 255;
                storeVelocity = projectile.velocity;
                projectile.velocity = Vector2.Zero;
            }

            if (projectile.ai[0] == 60)
            {
                projectile.velocity = storeVelocity;
                float distance = 600f; // Search distance

                if (!target)
                {
                    for (int k = 0; k < Main.maxNPCs; k++) // Loop through the player array
                    {
                        if (Main.npc[k].active && Main.npc[k].chaseable && !Main.npc[k].friendly)
                        {
                            Vector2 move = Vector2.Zero;
                            Vector2 moveTo = Main.npc[k].Center - projectile.Center;
                            float magnitude = (float)Math.Sqrt(moveTo.X * moveTo.X + moveTo.Y * moveTo.Y);
                            if (magnitude < distance)
                            {
                                distance = magnitude;
                                int launchSpeed = 22;
                                move = moveTo;
                                move *= launchSpeed / magnitude;
                                projectile.velocity = move;
                                target = true;
                                projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }

            if (projectile.ai[0] >= 61)
            {
                projectile.alpha = 0;
                Vector2 position = projectile.Center;
                Dust dust = Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }


            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);

            projectile.ai[0]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.ai[0] >= 61)
            {
                return Color.White;
            }
            else
            {
                return null;
            }
        }
    }
}