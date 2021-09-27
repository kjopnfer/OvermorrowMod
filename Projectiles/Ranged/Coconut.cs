using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Ranged
{
    public class Coconut : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.timeLeft = 200;
			projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.extraUpdates = 2;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
		public override void AI() 
		{
			projectile.velocity.Y += 0.13f;

			if (projectile.ai[0] > 3f)
			{				
				projectile.ai[0] += projectile.ai[1];
				if (projectile.ai[0] > 30f)
				{
					projectile.velocity.Y += 0.1f;
				}
				
				int num508 = 175;
				Color newColor2 = new Color(255, 255, 255, 100);
				for (int num509 = 0; num509 < 6; num509++)
				{
					Vector2 vector41 = projectile.velocity * num509 / 6f;
					int num510 = 6;
					int num511 = Dust.NewDust(projectile.position + Vector2.One * 6f, projectile.width - num510 * 2, projectile.height - num510 * 2, DustID.t_Slime, 0f, 0f, num508, newColor2, 1.2f);
					Dust dust;
					if (Main.rand.Next(2) == 0)
					{
						dust = Main.dust[num511];
						dust.alpha += 25;
					}
					if (Main.rand.Next(2) == 0)
					{
						dust = Main.dust[num511];
						dust.alpha += 25;
					}
					if (Main.rand.Next(2) == 0)
					{
						dust = Main.dust[num511];
						dust.alpha += 25;
					}
					Main.dust[num511].noGravity = true;
					dust = Main.dust[num511];
					dust.velocity *= 0.3f;
					dust = Main.dust[num511];
					dust.velocity += projectile.velocity * 0.5f;
					Main.dust[num511].position = projectile.Center;
					Main.dust[num511].position.X -= vector41.X;
					Main.dust[num511].position.Y -= vector41.Y;
					dust = Main.dust[num511];
					dust.velocity *= 0.2f;
				}
				if (Main.rand.Next(4) == 0)
				{
					int num512 = 6;
					int num513 = Dust.NewDust(projectile.position + Vector2.One * 6f, projectile.width - num512 * 2, projectile.height - num512 * 2, DustID.t_Slime, 0f, 0f, num508, newColor2, 1.2f);
					Dust dust = Main.dust[num513];
					dust.velocity *= 0.5f;
					dust = Main.dust[num513];
					dust.velocity += projectile.velocity * 0.5f;
				}
			}
			else
			{
				projectile.ai[0] += 1f;
			}
		}
    }
}