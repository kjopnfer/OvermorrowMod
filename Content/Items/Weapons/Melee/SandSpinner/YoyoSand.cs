using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Melee.SandSpinner
{
    public class YoyoSand : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        Projectile parentProjectile;

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
            Projectile.penetrate = -1;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            // Parent Projectile will have passed in the ID (Projectile.whoAmI) for the Projectile through AI fields when spawned
            for (int i = 0; i < Main.maxProjectiles; i++) // Loop through the Projectile array
            {
                // Check that the Projectile is the same as the parent Projectile and it is active
                if (Main.projectile[i] == Main.projectile[(int)Projectile.ai[0]] && Main.projectile[i].active)
                {
                    // Set the parent Projectile
                    parentProjectile = Main.projectile[i];
                    Projectile.netUpdate = true;
                }
            }

            if (parentProjectile.active)
            {
                Projectile.timeLeft = 2;
            }

            // Orbit around the parent Projectile
            DoProjectile_OrbitPosition(Projectile, parentProjectile.Center, 25);

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 3f)
            {
                int num1110 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Sand, Projectile.velocity.X, Projectile.velocity.Y, 50, default(Color), 1f);
                Main.dust[num1110].position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                Main.dust[num1110].noGravity = true;
                Dust dust81 = Main.dust[num1110];
                dust81.velocity *= 0.5f;

                Dust dustTrail = Dust.NewDustPerfect(new Vector2(Projectile.position.X, Projectile.position.Y), 32, Projectile.velocity);
                dustTrail.position = (Main.dust[num1110].position + Projectile.Center) / 2f;
                dustTrail.noGravity = true;
            }
        }

        public void DoProjectile_OrbitPosition(Projectile modProjectile, Vector2 position, double distance, double speed = 1.75)
        {
            Projectile Projectile = modProjectile;
            double deg = speed * (double)Projectile.ai[1];
            double rad = deg * (Math.PI / 180);

            Projectile.ai[1] += 3f;

            Projectile.position.X = position.X - (int)(Math.Cos(rad) * distance) - Projectile.width / 2;
            Projectile.position.Y = position.Y - (int)(Math.Sin(rad) * distance) - Projectile.height / 2;
            Projectile.netUpdate = true;
        }
    }
}