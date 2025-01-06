using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Archives
{
    public class ChairBook : LivingGrimoire
    {
        public override void CastSpell()
        {
            if (AICounter % 10 == 0 && AICounter < 40)
            {
                float angle = MathHelper.ToRadians(75);
                Vector2 projectileVelocity = new Vector2(12 * NPC.direction, 0).RotatedByRandom(angle);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, ModContent.ProjectileType<ChairBolt>(), 1, 1f, Main.myPlayer);
            }
        }
    }
}