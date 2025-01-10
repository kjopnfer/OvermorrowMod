using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class PlantBook : LivingGrimoire
    {
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
                float angle = MathHelper.ToRadians(75);
                Vector2 projectileVelocity = new Vector2(100 * NPC.direction, 0).RotatedByRandom(angle) * 50;
                Projectile.NewProjectile(NPC.GetSource_FromAI(), Player.Center, Vector2.Zero, ModContent.ProjectileType<PlaneProjectile>(), 1, 1f, Main.myPlayer);
            }
        }
    }
}