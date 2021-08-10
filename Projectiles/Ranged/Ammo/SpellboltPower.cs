using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Projectiles.Ranged.Ammo
{
    public class SpellboltPower : ModProjectile
    {
        public override string Texture => "Terraria/Projectile_" + ProjectileID.UnholyArrow; // until we've got a new texture
        public override void SetDefaults() {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
        }
        public override void AI() {
			Vector2 move = Vector2.Zero;
			float distance = 100f;
			bool target = false;

			for (int k = 0; k < 200; k++) {
				if (Main.npc[k].active && !Main.npc[k].dontTakeDamage && !Main.npc[k].friendly && Main.npc[k].lifeMax > 5) {
					Vector2 newMove = Main.npc[k].Center - projectile.Center;
					float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);

					if (distanceTo < distance) {
						move = newMove;
						distance = distanceTo;
						target = true;
					}
				}
			}

			if (target) {
				AdjustMagnitude(ref move);
				projectile.velocity = (10 * projectile.velocity + move * 2f) / 11f;
                AdjustMagnitude(ref projectile.velocity);
            }

            projectile.rotation =
                projectile.velocity.ToRotation() +
                MathHelper.ToRadians(90f); 
        }
		private void AdjustMagnitude(ref Vector2 vector) {
			float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			if (magnitude > 5f) {
				vector *= 10f / magnitude;
			}
		}
    }
}