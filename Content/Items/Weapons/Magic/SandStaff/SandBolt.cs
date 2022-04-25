using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Magic.SandStaff
{
    public class SandBolt : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        private int delay = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sand");
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            // The distance from the Projectile's center to the mouse position
            Vector2 mousePosition = new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center;

            if (delay == 0)
            {
                // Move towards the mouse
                if (mousePosition.Length() > Projectile.velocity.Length())
                {
                    mousePosition.Normalize();
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity.RotatedBy(Math.PI / 180), mousePosition * 14, 0.1f);
                }
                else // The Projectile has reached where the mouse was
                {
                    delay = 3;
                    Projectile.velocity = Projectile.velocity.RotatedByRandom(Math.PI) * 1.5f;
                }
            }
            else
            {
                delay--;
            }


            Projectile.netUpdate = true;
            Projectile.localAI[0] += 1f;

            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1.5f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(Projectile.position.X, Projectile.position.Y), 32, Projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }
    }
}