using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class MovementState : SuperState<BaseMovementState>
    {
        private List<(BaseMovementState state, int weight)> states = new List<(BaseMovementState, int)>();

        public MovementState(List<BaseMovementState> availableSubstates) : base(availableSubstates)
        {
            foreach (var substate in availableSubstates)
                states.Add((substate, substate.Weight));  // Assume BaseIdleState has a `Weight` property
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Movement state");
            currentSubstate = PickSubstate(npc);

            npc.AIStateMachine.RegisterSubstate(currentSubstate);
            currentSubstate?.Enter(npc);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentSubstate?.Exit(npc);
            Main.NewText("NPC exits Move state.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (currentSubstate == null || currentSubstate.IsFinished)
            {
                currentSubstate = PickSubstate(npc);
                currentSubstate.Enter(npc);
            }

            currentSubstate?.Update(npc);
        }

        private BaseMovementState PickSubstate(OvermorrowNPC npc)
        {
            return states
                .Where(s => s.state.CanExecute(npc))
                .OrderByDescending(s => s.weight) // or random weighted choice
                .FirstOrDefault().state;
        }
    }
}