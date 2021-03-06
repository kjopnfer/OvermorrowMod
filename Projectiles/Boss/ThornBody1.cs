using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class ThornBody1 : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.NettleBurstLeft; // TODO: Get actual texture
        private Projectile parentProjectile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn of Iorich");
        }

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.scale = 1.33f;
        }

        public override void AI()
        {
            // Get parent projectile
            if (Main.projectile[(int)projectile.ai[0]].active)
            {
                parentProjectile = Main.projectile[(int)projectile.ai[0]];
            }

            // Check parent projectile's time left
            if (parentProjectile.timeLeft > 60)
            {
                projectile.timeLeft = 60;
            }
            else
            {
                // Make projectiles gradually disappear
                if (projectile.timeLeft <= 60)
                {
                    projectile.alpha += 5;
                }
            }
        }
    }
}