using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class MovementState : State
    {
        private List<(BaseMovementState state, int weight)> movementStates = new List<(BaseMovementState, int)>();
        public BaseMovementState currentMovementSubstate { get; private set; }

        public MovementState(List<BaseMovementState> availableSubstates)
        {
            foreach (var substate in availableSubstates)
                movementStates.Add((substate, substate.Weight));  // Assume BaseIdleState has a `Weight` property
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Movement state");
            currentMovementSubstate = PickSubstate(npc);
            currentMovementSubstate?.Enter(npc);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentMovementSubstate?.Exit(npc);
            Main.NewText("NPC exits Move state.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (currentMovementSubstate == null || currentMovementSubstate.IsFinished)
            {
                currentMovementSubstate = PickSubstate(npc);
                currentMovementSubstate.Enter(npc);
            }

            currentMovementSubstate?.Update(npc);
        }

        private BaseMovementState PickSubstate(OvermorrowNPC npc)
        {
            return movementStates
                .Where(s => s.state.CanExecute(npc))
                .OrderByDescending(s => s.weight) // or random weighted choice
                .FirstOrDefault().state;
        }
    }
}