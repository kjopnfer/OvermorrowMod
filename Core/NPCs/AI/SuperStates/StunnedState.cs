using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Hit-stun state. Holds the NPC in place for AICounter ticks.
    /// </summary>
    public class StunnedState : State
    {
        public StunnedState(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExit => OvermorrowNPC.AICounter <= 0f;

        public override void Enter()
        {
            NPC.velocity.X = 0;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0f;
        }

        public override void Update()
        {
            NPC.velocity.X = 0;
            if (OvermorrowNPC.AICounter > 0f)
                OvermorrowNPC.AICounter--;
        }
    }
}
