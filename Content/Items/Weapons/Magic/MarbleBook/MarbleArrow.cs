using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.MarbleBook
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
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Vector2 origin = Projectile.Center;
                float radius = 10;
                int numLocations = 30;
                for (int i = 0; i < 30; i++)
                {
                    Vector2 position = origin + Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                    Dust dust = Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
                }
                Projectile.alpha = 255;
                storeVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
            }

            if (Projectile.ai[0] == 60)
            {
                Projectile.velocity = storeVelocity;
                float distance = 250f; // Search distance

                if (!target)
                {
                    for (int k = 0; k < Main.maxNPCs; k++) // Loop through the player array
                    {
                        if (Main.npc[k].active && Main.npc[k].chaseable && !Main.npc[k].friendly)
                        {
                            Vector2 move = Vector2.Zero;
                            Vector2 moveTo = Main.npc[k].Center - Projectile.Center;
                            float magnitude = (float)Math.Sqrt(moveTo.X * moveTo.X + moveTo.Y * moveTo.Y);
                            if (magnitude < distance)
                            {
                                distance = magnitude;
                                int launchSpeed = 22;
                                move = moveTo;
                                move *= launchSpeed / magnitude;
                                Projectile.velocity = move;
                                target = true;
                                Projectile.netUpdate = true;
                            }
                        }
                    }
                }
            }

            if (Projectile.ai[0] >= 61)
            {
                Projectile.alpha = 0;
                Vector2 position = Projectile.Center;
                Dust dust = Dust.NewDustPerfect(position, 57, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0);

            Projectile.ai[0]++;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[0] >= 61)
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