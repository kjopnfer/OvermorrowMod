using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class IdleState : State
    {
        private List<(BaseIdleState attackState, int weight)> idleStates = new List<(BaseIdleState, int)>();
        private BaseIdleState currentIdleSubstate;

        public IdleState(List<BaseIdleState> availableSubstates)
        {
            foreach (var substate in availableSubstates)
            {
                idleStates.Add((substate, substate.Weight));  // Assume BaseIdleState has a `Weight` property
            }
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Idle state.");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("NPC exits Idle state.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.TargetingModule.HasTarget())
            {
                // Do attacks.
                //npc.AIStateMachine.ChangeState()
            }
            else
            {
                // Do normal idle stuff.
            }
        }
    }
}