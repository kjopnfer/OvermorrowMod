using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ChairBook : LivingGrimoire
    {
        int projectilesNeeded = 0;
        public override bool AttackCondition()
        {
            var chairSummons = Main.npc
                .Where(npc => npc.active
                    && npc.ModNPC is ChairSummon chairSummon
                    && chairSummon.ParentID == NPC.whoAmI)
                .Select(npc => npc.ModNPC as ChairSummon)
                .ToList();

            if (chairSummons.Count == 3) return false;

            projectilesNeeded = 3 - chairSummons.Count;
            float xDistance = Math.Abs(NPC.Center.X - Player.Center.X);

            bool xDistanceCheck = xDistance <= tileAttackDistance * 18;
            bool yDistanceCheck = Math.Abs(NPC.Center.Y - Player.Center.Y) < 100;

            return xDistanceCheck && yDistanceCheck && Collision.CanHitLine(Player.Center, 1, 1, NPC.Center, 1, 1);
        }

        public override void CastSpell()
        {
            if (projectilesNeeded == 0) return;

            if (AICounter % 10 == 0 && AICounter < 40)
            {
                float angle = MathHelper.ToRadians(75);
                float randomDirection = Main.rand.NextBool() ? 1 : -1;

                // Make sure the x-offset is never zero
                Vector2 projectileVelocity = new Vector2(Main.rand.Next(-3, 2) + 1, Main.rand.Next(8, 10)).RotatedByRandom(angle);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, ModContent.ProjectileType<ChairBolt>(), 1, 1f, Main.myPlayer, 0f, NPC.whoAmI);

                projectilesNeeded--;
            }
        }
    }
}