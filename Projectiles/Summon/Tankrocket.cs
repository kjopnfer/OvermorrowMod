using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Projectiles.Summon
{
    public class Tankrocket : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tank Rocket");
        }

        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.width = 16;
            projectile.height = 12;
            projectile.penetrate = 1;
            projectile.aiStyle = 34;
            projectile.timeLeft = 120; //The amount of time the projectile is alive for
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.immune[projectile.owner] = 1;
		} 

        public override void Kill(int timeLeft)
        {
            int explode = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0, 0, 338, projectile.damage, 3f, projectile.owner, 0f);
            Main.projectile[explode].timeLeft = 0;
            Main.projectile[explode].friendly = true;
            Main.projectile[explode].hostile = false;
        }
    }
}
