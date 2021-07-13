using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Magic
{
    public class Blast3 : ModProjectile
    {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.ChargedBlasterOrb;

        private int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[base.projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 34;
            projectile.timeLeft = 250;
            projectile.penetrate = -1;
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
                projectile.damage *= 4;
                if(Main.MouseWorld.X > Main.player[projectile.owner].Center.X)
                {
                    projectile.velocity.X = 17;
                    projectile.velocity.Y = 0;
                }
                else
                {
                    projectile.velocity.X = -17;
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
