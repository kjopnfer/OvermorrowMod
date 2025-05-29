using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class MovementState : SuperState<BaseMovementState>
    {
        public bool HasValidMovement { get; private set; } = false;

        public MovementState(List<BaseMovementState> availableSubstates, OvermorrowNPC npc) : base(availableSubstates, npc)
        {
        }

        public override void Enter()
        {
            currentSubstate = PickSubstate(OvermorrowNPC);

            HasValidMovement = currentSubstate != null;
            if (HasValidMovement)
            {
                OvermorrowNPC.AIStateMachine.RegisterSubstate(currentSubstate);
                currentSubstate?.Enter();
            }
        }

        public override void Exit()
        {
            currentSubstate?.Exit();
        }

        public override void Update()
        {
            if (currentSubstate == null || currentSubstate.IsFinished)
            {
                currentSubstate = PickSubstate(OvermorrowNPC);
                HasValidMovement = currentSubstate != null;
                if (!HasValidMovement)
                {
                    return;
                }

                currentSubstate.Enter();

            }

            currentSubstate?.Update();
        }

        private BaseMovementState PickSubstate(OvermorrowNPC npc)
        {
            if (substates == null || substates.Count == 0)
                return null;

            return substates
                .Where(s => s.CanExecute())
                .OrderByDescending(s => s.Weight) // or random weighted choice
                .FirstOrDefault();
        }
    }
}