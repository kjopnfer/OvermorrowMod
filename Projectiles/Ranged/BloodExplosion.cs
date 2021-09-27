using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace OvermorrowMod.Projectiles.Ranged
{
	public class BloodExplosion : ModProjectile
	{
		public override string Texture => "OvermorrowMod/Projectiles/Ranged/MeatMissileProj";
		public override void SetDefaults()
		{
			projectile.alpha = 255;
			projectile.hostile = false;
			projectile.friendly = true;
			projectile.hostile = true;
			projectile.tileCollide = false;
			projectile.width = 64;
			projectile.height = 64;
			projectile.penetrate = -1;
			projectile.timeLeft = 3;
		}

		public override void Kill(int timeLeft)
		{

			Vector2 position = projectile.Center;
			Main.PlaySound(SoundID.NPCDeath1, (int)position.X, (int)position.Y);

			for (int num864 = 0; num864 < 30; num864++)
			{
				int num865 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
				Dust dust = Main.dust[num865];
				dust.velocity *= 1.4f;
			}
			for (int num866 = 0; num866 < 20; num866++)
			{
				int num867 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.VampireHeal, 0f, 0f, 100, default(Color), 2.5f);
				Main.dust[num867].noGravity = true;
				Dust dust = Main.dust[num867];
				dust.velocity *= 3.5f;
				num867 = Dust.NewDust(new Vector2(position.X, position.Y), projectile.width, projectile.height, DustID.VampireHeal, 0f, 0f, 100, default(Color), 1f);
				dust = Main.dust[num867];
				dust.velocity *= 1.5f;
			}
			for (int num868 = 0; num868 < 2; num868++)
			{
				float num869 = 0.4f;
				if (num868 == 1)
				{
					num869 = 0.8f;
				}
				int num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
				Gore gore = Main.gore[num870];
				gore.velocity *= num869;
				Main.gore[num870].velocity.X += 1f;
				Main.gore[num870].velocity.Y += 1f;
				num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num870];
				gore.velocity *= num869;
				Main.gore[num870].velocity.X -= 1f;
				Main.gore[num870].velocity.Y += 1f;
				num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num870];
				gore.velocity *= num869;
				Main.gore[num870].velocity.X += 1f;
				Main.gore[num870].velocity.Y -= 1f;
				num870 = Gore.NewGore(new Vector2(position.X, position.Y), default(Vector2), Main.rand.Next(61, 64));
				gore = Main.gore[num870];
				gore.velocity *= num869;
				Main.gore[num870].velocity.X -= 1f;
				Main.gore[num870].velocity.Y -= 1f;
			}
		}
	}
}