using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    /// <summary>
    /// A grounded recoil for when the NPC is taking sustained damage from any source.
    /// After a short reaction delay it leaps away from the target, then settles.
    /// Trigger frequency and reaction speed scale with Reactivity.
    /// </summary>
    public class RecoilLeap : BaseDefenseState
    {
        public override int Weight => 5;

        private const int MaxAirborneTicks = 90;

        private int delay;
        private bool leaped;
        private int airborne;

        public RecoilLeap(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (!OvermorrowNPC.TookSustainedDamage()) return false;
            return Main.rand.NextFloat() < OvermorrowNPC.Personality.Reactivity;
        }

        public override void Enter()
        {
            OvermorrowNPC.ClearDamageWindow();
            delay = (int)MathHelper.Lerp(14, 4, OvermorrowNPC.Personality.Reactivity);
            leaped = false;
            airborne = 0;
            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.velocity.X = 0;
        }

        public override void Update()
        {
            if (delay > 0)
            {
                delay--;
                NPC.velocity.X = 0;
                return;
            }

            if (!leaped)
            {
                int away = AwayDirection();
                NPC.direction = away;
                NPC.velocity.X = 6f * away;
                NPC.velocity.Y = -5f;
                leaped = true;
                return;
            }

            if (NPC.collideY && NPC.velocity.Y >= 0)
                IsFinished = true;

            if (++airborne >= MaxAirborneTicks)
                IsFinished = true;
        }

        private int AwayDirection()
        {
            if (OvermorrowNPC.TargetingModule.HasTarget())
                return OvermorrowNPC.TargetingModule.Target.Center.X < NPC.Center.X ? 1 : -1;

            return NPC.direction == 0 ? 1 : -NPC.direction;
        }
    }
}
