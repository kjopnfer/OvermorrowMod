using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    /// <summary>
    /// A short reposition for when the NPC is wedged against terrain while approaching
    /// its target. Moves away from the target for a moment, hopping if still blocked,
    /// so it cannot grind into geometry indefinitely.
    /// </summary>
    public class BreakOff : BaseDefenseState
    {
        public override int Weight => 6;

        private int timer;
        private int dir;
        private int lastFireTick = -1000;

        public BreakOff(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (Main.GameUpdateCount - lastFireTick < 90) return false;
            return OvermorrowNPC.IsCornered;
        }

        public override void Enter()
        {
            OvermorrowNPC.ClearCornered();
            lastFireTick = (int)Main.GameUpdateCount;

            bool targetLeft = OvermorrowNPC.TargetingModule.HasTarget() && OvermorrowNPC.TargetingModule.Target.Center.X < NPC.Center.X;
            dir = targetLeft ? 1 : -1;

            timer = 40;
            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.velocity.X = 0;
        }

        public override void Update()
        {
            NPC.direction = dir;
            NPC.velocity.X = 2f * dir;

            if (NPC.collideX && NPC.collideY)
                NPC.velocity.Y = -4f;

            if (timer-- <= 0)
                IsFinished = true;
        }
    }
}
