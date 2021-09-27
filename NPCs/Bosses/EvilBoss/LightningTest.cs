using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.EvilBoss
{
    public class LightningTest : ModProjectile
    {
        private int KeepAliveTime2 = 1;
        private int KeepAliveTime = 1;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.HeatRay;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HeatRay);
            aiType = ProjectileID.HeatRay;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 500;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            KeepAliveTime2++;
            if (KeepAliveTime2 == 200)
            {
                KeepAliveTime = 0;
            }
            projectile.timeLeft = projectile.timeLeft + KeepAliveTime;
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            projectile.position.X = 405 * (float)Math.Cos(projectile.rotation) + parentProjectile.Center.X - 7;
            projectile.position.Y = 405 * (float)Math.Sin(projectile.rotation) + parentProjectile.Center.Y - 7;
            projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 50 / 10)); // 200 is the speed, god only knows what dividing by 10 does
        }
    }
}