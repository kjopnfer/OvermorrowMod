using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class MovementState : State
    {
        private List<(BaseMovementState attackState, int weight)> movementStates = new List<(BaseMovementState, int)>();
        private BaseMovementState currentMovementSubstate;

        public MovementState(List<BaseMovementState> availableSubstates)
        {
            foreach (var substate in availableSubstates)
            {
                movementStates.Add((substate, substate.Weight));  // Assume BaseIdleState has a `Weight` property
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
            }
            else
            {
                // Do normal idle stuff.
            }
        }
    }
}