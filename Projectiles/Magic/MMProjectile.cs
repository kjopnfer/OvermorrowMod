using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.IO;
using OvermorrowMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Magic
{
	public class MMProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magic Missile");
		}
		public override void SetDefaults()
		{
			projectile.height = 64;
			projectile.width = 64;
			projectile.friendly = true;
			projectile.light = 0.8f;
			projectile.magic = true;
		}

		public override void AI()
		{
			if (Main.myPlayer == projectile.owner && projectile.ai[0] == 0f)
			{

				Player player = Main.player[projectile.owner];
				// If the player channels the weapon, do something. This check only works if item.channel is true for the weapon.
				if (player.channel)
				{
					float maxDistance = 18f; // This also sets the maximun speed the projectile can reach while following the cursor.
					Vector2 vectorToCursor = Main.MouseWorld - projectile.Center;
					float distanceToCursor = vectorToCursor.Length();

					// Here we can see that the speed of the projectile depends on the distance to the cursor.
					if (distanceToCursor > maxDistance)
					{
						distanceToCursor = maxDistance / distanceToCursor;
						vectorToCursor *= distanceToCursor;
					}

					int velocityXBy1000 = (int)(vectorToCursor.X * 1000f);
					int oldVelocityXBy1000 = (int)(projectile.velocity.X * 1000f);
					int velocityYBy1000 = (int)(vectorToCursor.Y * 1000f);
					int oldVelocityYBy1000 = (int)(projectile.velocity.Y * 1000f);

					// This code checks if the precious velocity of the projectile is different enough from its new velocity, and if it is, syncs it with the server and the other clients in MP.
					// We previously multiplied the speed by 1000, then casted it to int, this is to reduce its precision and prevent the speed from being synced too much.
					if (velocityXBy1000 != oldVelocityXBy1000 || velocityYBy1000 != oldVelocityYBy1000)
					{
						projectile.netUpdate = true;
					}

					projectile.velocity = vectorToCursor;

				}
				// If the player stops channeling, do something else.
				else if (projectile.ai[0] == 0f)
				{

					// This code block is very similar to the previous one, but only runs once after the player stops channeling their weapon.
					projectile.netUpdate = true;

					float maxDistance = 14f; // This also sets the maximun speed the projectile can reach after it stops following the cursor.
					Vector2 vectorToCursor = Main.MouseWorld - projectile.Center;
					float distanceToCursor = vectorToCursor.Length();

					//If the projectile was at the cursor's position, set it to move in the oposite direction from the player.
					if (distanceToCursor == 0f)
					{
						vectorToCursor = projectile.Center - player.Center;
						distanceToCursor = vectorToCursor.Length();
					}

					distanceToCursor = maxDistance / distanceToCursor;
					vectorToCursor *= distanceToCursor;

					projectile.velocity = vectorToCursor;

					if (projectile.velocity == Vector2.Zero)
					{
						projectile.Kill();
					}

					projectile.ai[0] = 1f;
				}
			}

			// Set the rotation so the projectile points towards where it's going.
			if (projectile.velocity != Vector2.Zero)
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
			}
		}
		public override void Kill(int timeLeft)
		{
			// If the projectile dies without hitting an enemy, crate a small explosion that hits all enemies in the area.
			if (projectile.penetrate == 1)
			{
				// Makes the projectile hit all enemies as it circunvents the penetrate limit.
				projectile.maxPenetrate = -1;
				projectile.penetrate = -1;

				int explosionArea = 60;
				Vector2 oldSize = projectile.Size;
				// Resize the projectile hitbox to be bigger.
				projectile.position = projectile.Center;
				projectile.Size += new Vector2(explosionArea);
				projectile.Center = projectile.position;

				projectile.tileCollide = false;
				projectile.velocity *= 0.01f;
				// Damage enemies inside the hitbox area
				projectile.Damage();
				projectile.scale = 0.01f;

				//Resize the hitbox to its original size
				projectile.position = projectile.Center;
				projectile.Size = new Vector2(10);
				projectile.Center = projectile.position;
			}
		}
		public Color TrailColor(float progress)
		{
			return Color.Lerp(Color.Green, Color.Black, progress) * (1f - progress);
		}
		public float TrailSize(float progress)
        {
			return 16f * (1f - progress);
        }
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			/*TrailHelper th = new TrailHelper(projectile, TrailColor, TrailSize);
			th.Draw();*/
			return true;
        }
    }
}
