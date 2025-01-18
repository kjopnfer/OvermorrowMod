using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class BlasterBook : LivingGrimoire
    {
        protected override void DrawCastEffect(SpriteBatch spriteBatch)
        {
          
        }


        public override bool AttackCondition()
        {
            float xDistance = Math.Abs(NPC.Center.X - Player.Center.X);

            bool xDistanceCheck = xDistance <= tileAttackDistance * 18;
            bool yDistanceCheck = Math.Abs(NPC.Center.Y - Player.Center.Y) < 100;

            return xDistanceCheck && yDistanceCheck && Collision.CanHitLine(Player.Center, 1, 1, NPC.Center, 1, 1);
        }

        public override void CastSpell()
        {
            if (AICounter == 20)
            {
                Vector2 directionToPlayer = (Player.Center - NPC.Center).SafeNormalize(Vector2.Zero); // Direction vector to the player

                float angleSpread = MathHelper.ToRadians(25); // Spread angle for randomness
                Vector2 projectileVelocity = directionToPlayer.RotatedByRandom(angleSpread) * 8; // Randomized rotation towards the player
                Vector2 spawnPosition = NPC.Center + new Vector2(32, 0).RotatedBy(directionToPlayer.ToRotation());
                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, projectileVelocity, ModContent.ProjectileType<BlastRune>(), NPC.damage, 1f, Main.myPlayer, NPC.whoAmI);
            }
        }
    }
}