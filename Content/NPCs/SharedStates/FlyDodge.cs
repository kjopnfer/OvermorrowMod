using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    /// <summary>
    /// Airborne perpendicular sidestep when an incoming projectile is detected. Scales with Reactivity.
    /// </summary>
    public class FlyDodge : BaseDefenseState
    {
        public override int Weight => 8;

        private int timer;
        private Vector2 dodgeDir;
        private int lastFireTick = -1000;

        public FlyDodge(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (Main.GameUpdateCount - lastFireTick < 40) return false;
            if (!OvermorrowNPC.HasIncomingProjectile(out Projectile threat)) return false;
            if (Main.rand.NextFloat() >= OvermorrowNPC.Personality.Reactivity) return false;

            Vector2 perpendicular = threat.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitX);
            dodgeDir = Vector2.Dot(perpendicular, NPC.Center - threat.Center) >= 0 ? perpendicular : -perpendicular;
            return true;
        }

        public override void Enter()
        {
            timer = (int)MathHelper.Lerp(16, 3, OvermorrowNPC.Personality.Reactivity);
            lastFireTick = (int)Main.GameUpdateCount;
            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.velocity *= 0.5f;
        }

        public override void Update()
        {
            if (timer-- > 0) return;

            NPC.velocity = dodgeDir * 6f;
            if (timer < -20)
                IsFinished = true;
        }
    }
}
