using Ionic;
using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;

namespace OvermorrowMod.Core.NPCs
{
    public class IdleState : State
    {
        private List<(BaseIdleState state, int weight)> idleStates;
        private BaseIdleState currentIdleSubstate;
        private readonly WeightedRandom<BaseIdleState> idleSelector;

        public IdleState(List<BaseIdleState> availableSubstates)
        {
            idleStates = new List<(BaseIdleState, int)>();
            foreach (var sub in availableSubstates)
                idleStates.Add((sub, sub.Weight));
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Idle state.");
            currentIdleSubstate = PickSubstate(npc);
            currentIdleSubstate.Enter(npc);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentIdleSubstate?.Exit(npc);
            Main.NewText("NPC exits Idle state.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            /*if (npc.TargetingModule.HasTarget())
            {
                // Do attacks.
                //npc.AIStateMachine.ChangeState()
            }
            else
            {
                // Do normal idle stuff.
            }*/
            if (currentIdleSubstate == null || currentIdleSubstate.IsFinished)
            {
                currentIdleSubstate = PickSubstate(npc);
                currentIdleSubstate.Enter(npc);
            }

            currentIdleSubstate?.Update(npc);
        }

        private BaseIdleState PickSubstate(OvermorrowNPC npc)
        {
            return idleStates
                .Where(s => s.state.CanExecute(npc))
                .OrderByDescending(s => s.weight) // or random weighted choice
                .FirstOrDefault().state;
        }
    }
}