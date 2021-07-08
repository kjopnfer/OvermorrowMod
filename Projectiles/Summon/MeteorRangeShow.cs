using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System;

namespace OvermorrowMod.Projectiles.Summon
{
    public class MeteorRangeShow : ModProjectile
    {
        public override bool CanDamage() => false;
        private int KeepAliveTime2 = 1; 
        private int KeepAliveTime = 1;
        public override string Texture => "Terraria/Projectile_" + ProjectileID.HeatRay;
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.HeatRay);
            aiType = ProjectileID.HeatRay;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.ignoreWater = true;
            projectile.hostile = true;
            projectile.timeLeft = 500;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile parentProjectile = Main.projectile[(int)projectile.ai[0]];
            projectile.timeLeft += 1;
            if(parentProjectile.type == mod.ProjectileType("MeteorStill"))
            {
                projectile.position.X = 200 * (float)Math.Cos(projectile.rotation) + parentProjectile.Center.X;
                projectile.position.Y = 200 * (float)Math.Sin(projectile.rotation) + parentProjectile.Center.Y;
                projectile.rotation += (float)((2 * Math.PI) / (Math.PI * 2 * 50 / 10)); // 200 is the speed, god only knows what dividing by 10 does
            }
        }
    }
}