using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class BarrierBook : LivingGrimoire
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
            if (AICounter == 10)
            {
                float radius = 500f;
                var nearbyHostileEnemies = Main.npc
                    .Where(enemy => enemy.active && !enemy.friendly && Vector2.Distance(NPC.Center, enemy.Center) <= radius && enemy.whoAmI != NPC.whoAmI)
                    .ToList();

                foreach (NPC enemy in nearbyHostileEnemies)
                {
                    enemy.AddBarrier(50, 100);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), enemy.Center, Vector2.Zero, ModContent.ProjectileType<BarrierEffect>(), 1, 1f, Main.myPlayer, ai0: enemy.whoAmI);
                }
            }
        }
    }
}