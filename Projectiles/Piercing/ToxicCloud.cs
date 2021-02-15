using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class ToxicCloud : ModProjectile
    {
        public override string Texture => "Terraria/Item_" + ProjectileID.ToxicCloud;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.timeLeft = 90;
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (projectile.ai[1] % 30 == 0)
                {
                    int num898 = Main.rand.Next(5, 10);
                    for (int num897 = 0; num897 < num898; num897++)
                    {
                        Vector2 value28 = new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));
                        value28.Normalize();
                        value28 *= (float)Main.rand.Next(10, 51) * 0.01f;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value28.X, value28.Y, 511 + Main.rand.Next(3), (int)projectile.ai[0], 1f, projectile.owner, 0f, Main.rand.Next(-45, 1));
                    }
                }
            }
            projectile.ai[1]++;
        }
    }
}