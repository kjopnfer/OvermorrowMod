using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ChairBook : LivingGrimoire
    {
        public override void CastSpell()
        {
            var chairSummons = Main.npc
                .Where(npc => npc.active
                    && npc.ModNPC is ChairSummon chairSummon
                    && chairSummon.ParentID == NPC.whoAmI)
                .Select(npc => npc.ModNPC as ChairSummon)
                .ToList();

            if (AICounter % 10 == 0 && AICounter < 40 && chairSummons.Count < 3)
            {
                float angle = MathHelper.ToRadians(75);
                float randomDirection = Main.rand.NextBool() ? 1 : -1;
                Vector2 projectileVelocity = new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(8, 10)).RotatedByRandom(angle);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, ModContent.ProjectileType<ChairBolt>(), 1, 1f, Main.myPlayer, 0f, NPC.whoAmI);
            }
        }
    }
}