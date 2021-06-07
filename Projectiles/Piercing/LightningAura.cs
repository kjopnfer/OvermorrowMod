using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class LightningAura : ModProjectile
    {
		private Projectile parentProjectile;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Cutter");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
			projectile.timeLeft = 180;
        }

        public override void AI()
        {
			/*// Parent projectile will have passed in the ID (projectile.whoAmI) for the projectile through AI fields when spawned
			for (int i = 0; i < Main.maxProjectiles; i++) // Loop through the projectile array
			{
				// Check that the projectile is the same as the parent projectile and it is active
				if (Main.projectile[i] == Main.projectile[(int)projectile.ai[0]] && Main.projectile[i].active)
				{
					// Set the parent projectile
					parentProjectile = Main.projectile[i];
				}
			}

			if (!parentProjectile.active) // Kill projectile if the parent is dead
			{
				projectile.Kill();
			}*/

			// projectile.center = parentProjectile.center;

			float num738 = 30f;
			float num737 = num738 * 4f;
			projectile.ai[0] += 1f;
			if (projectile.ai[0] > num737)
			{
				projectile.ai[0] = 0f;
			}
			Vector2 vector262 = -Vector2.UnitY.RotatedBy((float)Math.PI * 2f * projectile.ai[0] / num738);
			float val3 = 0.75f + vector262.Y * 0.25f;
			float val2 = 0.8f - vector262.Y * 0.2f;
			float num735 = Math.Max(val3, val2);
			projectile.position += new Vector2(projectile.width, projectile.height) / 2f;
			projectile.width = (projectile.height = (int)(80f * num735));
			projectile.position -= new Vector2(projectile.width, projectile.height) / 2f;
			projectile.frameCounter++;
			if (projectile.frameCounter >= 3)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
			}
			for (int num733 = 0; num733 < 1; num733++)
			{
				float num732 = 55f * num735;
				float num731 = 11f * num735;
				float num730 = 0.5f;
				int num728 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, default(Color), 0.5f);
				Main.dust[num728].noGravity = true;
				Dust dust81 = Main.dust[num728];
				dust81.velocity *= 2f;
				Main.dust[num728].position = ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * (num731 + num730 * (float)Main.rand.NextDouble() * num732) + projectile.Center;
				Main.dust[num728].velocity = Main.dust[num728].velocity / 2f + Vector2.Normalize(Main.dust[num728].position - projectile.Center);
				if (Main.rand.Next(2) == 0)
				{
					num728 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, default(Color), 0.9f);
					Main.dust[num728].noGravity = true;
					dust81 = Main.dust[num728];
					dust81.velocity *= 1.2f;
					Main.dust[num728].position = ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * (num731 + num730 * (float)Main.rand.NextDouble() * num732) + projectile.Center;
					Main.dust[num728].velocity = Main.dust[num728].velocity / 2f + Vector2.Normalize(Main.dust[num728].position - projectile.Center);
				}
				if (Main.rand.Next(4) == 0)
				{
					num728 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 226, 0f, 0f, 100, default(Color), 0.7f);
					Main.dust[num728].noGravity = true;
					dust81 = Main.dust[num728];
					dust81.velocity *= 1.2f;
					Main.dust[num728].position = ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * (num731 + num730 * (float)Main.rand.NextDouble() * num732) + projectile.Center;
					Main.dust[num728].velocity = Main.dust[num728].velocity / 2f + Vector2.Normalize(Main.dust[num728].position - projectile.Center);
				}
			}
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 3;
        }
    }
}