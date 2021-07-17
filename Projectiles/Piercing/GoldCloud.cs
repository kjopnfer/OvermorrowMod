using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
	public class GoldCloud : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("HeavenCloud");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.RainCloudRaining);
			aiType = 0;
			projectile.alpha = 255;
			projectile.timeLeft = 180;
		}

		public override void AI()
		{
			projectile.damage = 0;
			projectile.velocity = Vector2.Zero;

			if (projectile.ai[0] < 160)
			{
				projectile.alpha -= 2;
            }
            else
            {
				projectile.alpha += 15;
            }


			if (projectile.ai[0] == 140)
            {
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
					if (distance <= 1050)
					{
						Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 30;
					}
				}

				Projectile.NewProjectile(projectile.Center, new Vector2(0, 5), ModContent.ProjectileType<DivineLightning>(), 30, 10f, projectile.owner);
            }

			projectile.ai[0]++;

			if (++projectile.frameCounter >= 8)
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