using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class Blast2 : ModProjectile
    {
        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[base.projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 22;
            projectile.timeLeft = 250;
            projectile.penetrate = 2;
            projectile.scale = 1.15f;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            timer++;
            if(timer == 1)
            {
                projectile.damage = (projectile.damage * 2) + 7;
                if(Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    projectile.velocity.X = 14.5f;
                    projectile.velocity.Y = 0;
                }
                else
                {
                    projectile.velocity.X = -14.5f;
                    projectile.velocity.Y = 0;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation();
            if (++projectile.frameCounter >= 4)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= Main.projFrames[projectile.type])
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}
