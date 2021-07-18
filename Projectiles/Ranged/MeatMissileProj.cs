using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
	public class MeatMissileProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 14;
			projectile.height = 14;
			projectile.timeLeft = 200;
			projectile.penetrate = 1;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.tileCollide = true;
			drawOffsetX = -6;
		}

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();

			Terraria.Dust.NewDustPerfect(projectile.Center, 5, new Vector2(0f, 0f), 0, new Color(255, 255, 255), 1f);
		}

		public override void Kill(int timeLeft)
		{
			Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodExplosion>(), projectile.damage, 10f, projectile.owner);
		}
	}
}