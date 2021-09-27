using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MeteorBall : ModProjectile
    {
        private float length = 1;
        private int timer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[base.projectile.type] = 4;

        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 26;
            projectile.penetrate = 1;
            projectile.hostile = true;
            projectile.light = 1f;
            projectile.friendly = true;
            projectile.hostile = false;

            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 190;
        }

        public override void AI()
        {

            for (int i = 0; i < 200; i++)
            {

            Projectile parentProjectile = Main.projectile[i];
            if (parentProjectile.active && parentProjectile.type == mod.ProjectileType("MeteorStill"))
            {
                timer++;
                if (timer == 1)
                {
                    length = Vector2.Distance(Main.MouseWorld, parentProjectile.Center);
                    projectile.rotation = MathHelper.ToRadians(270f);
                
                if(length > 190)
                {
                    length = 190;
                }
                }
                
                
                if(length > 190)
                {
                    length = 190;
                }
                
                    projectile.position.X = length * (float)Math.Cos(projectile.rotation) + parentProjectile.Center.X - projectile.width / 2;
                    projectile.position.Y = length * (float)Math.Sin(projectile.rotation) + parentProjectile.Center.Y - projectile.height / 2;
                    projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 125 / 10)); // 200 is the speed, god only knows what dividing by 10 does


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
    }
}
