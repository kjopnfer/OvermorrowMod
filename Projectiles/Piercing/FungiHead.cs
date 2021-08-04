using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Buffs;
using OvermorrowMod.NPCs.Bosses.StormDrake;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
	public class FungiHead : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungi Head");
		}

		public override void SetDefaults()
		{
			projectile.width = 28;
			projectile.height = 28;
			projectile.alpha = 255;
			projectile.timeLeft = 90;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<ShroomBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<ShroomBuff>()))
			{
				projectile.timeLeft = 2;
			}

			projectile.rotation = (projectile.Center - Main.MouseWorld).ToRotation() - MathHelper.PiOver2;

			// Various position stuff
			if (projectile.ai[1] == -1)
			{
				projectile.Center = player.Center - new Vector2(-45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
			}
			else if (projectile.ai[1] == 1)
			{
				projectile.Center = player.Center - new Vector2(45, MathHelper.Lerp(50, 65, (float)Math.Sin(projectile.ai[0] / 60f)));
			}
			else
			{
				projectile.Center = player.Center - new Vector2(0, MathHelper.Lerp(75, 85, (float)Math.Sin(projectile.ai[0] / 60f)));
			}

			projectile.damage = 0;
			projectile.velocity = Vector2.Zero;

			if (projectile.ai[0] < 80)
			{
				projectile.alpha -= 4;
			}

			projectile.ai[0]++;

			if (projectile.ai[0] % 180 == 0)
			{
				Vector2 shootVelocity = Main.MouseWorld - projectile.Center;
				shootVelocity.Normalize();
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.NewProjectile(projectile.Center, shootVelocity * Main.rand.Next(5, 7), ModContent.ProjectileType<FungiSpore>(), 21, 10f, projectile.owner);
				}
			}

			if (++projectile.frameCounter >= 8)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			var player = Main.player[projectile.owner];

			Vector2 mountedCenter = player.Center;
			Texture2D chainTexture = ModContent.GetTexture("OvermorrowMod/Projectiles/Piercing/FungiHeadChain");

			var drawPosition = projectile.Center;
			var remainingVectorToPlayer = mountedCenter - drawPosition;

			float rotation = remainingVectorToPlayer.ToRotation() - MathHelper.PiOver2;

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (true)
			{
				float length = remainingVectorToPlayer.Length();

				if (length < 25f || float.IsNaN(length))
					break;

				// drawPosition is advanced along the vector back to the player by 12 pixels
				// 12 comes from the height of ExampleFlailProjectileChain.png and the spacing that we desired between links
				drawPosition += remainingVectorToPlayer * 12 / length;
				remainingVectorToPlayer = mountedCenter - drawPosition;

				// Finally, we draw the texture at the coordinates using the lighting information of the tile coordinates of the chain section
				Color color = Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f));
				spriteBatch.Draw(chainTexture, drawPosition - Main.screenPosition, null, Color.Lerp(Color.Transparent, color, projectile.ai[0] < 64 ? projectile.ai[0] / 64 : 1), rotation, chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
			}

			return true;
		}
    }
}