using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class DefendingState : SuperState<BaseDefenseState>
    {
        public bool HasValidDefense { get; private set; } = false;

        public DefendingState(List<BaseDefenseState> availableSubstates, OvermorrowNPC npc) : base(availableSubstates, npc)
        {
        }

        /// <summary>
        /// Whether any defending substate currently wants to run. Read by the
        /// state machine selector before entering this superstate.
        /// </summary>
        public bool AnyValid() => substates != null && substates.Any(s => s.CanExecute());

        public override void Enter()
        {
            currentSubstate = PickSubstate();

            HasValidDefense = currentSubstate != null;
            if (HasValidDefense)
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
            HasValidDefense = currentSubstate != null;
            if (currentSubstate?.IsFinished ?? true)
            {
                currentSubstate?.Exit();
                currentSubstate = null;
            }
            else
            {
                currentSubstate?.Update();
            }
        }

        private BaseDefenseState PickSubstate()
        {
            if (substates == null || substates.Count == 0)
                return null;

            return PickWeightedRandom(substates.Where(s => s.CanExecute()).ToList());
        }
    }
}
