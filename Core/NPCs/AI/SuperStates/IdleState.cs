using Ionic;
using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Utilities;

namespace OvermorrowMod.Core.NPCs
{
    public class IdleState : SuperState<BaseIdleState>
    {
        private List<(BaseIdleState state, int weight)> states;
        public BaseIdleState currentIdleSubstate { get; private set; }

        public IdleState(List<BaseIdleState> availableSubstates) : base(availableSubstates)
        {
            states = new List<(BaseIdleState, int)>();
            foreach (var sub in availableSubstates)
                states.Add((sub, sub.Weight));
        }

        public override void Enter(OvermorrowNPC npc)
        {
            //currentIdleSubstate = PickSubstate(npc);
            //currentIdleSubstate.Enter(npc);
            Main.NewText(npc.Name + " enters Idle state.");
            currentIdleSubstate = null;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentIdleSubstate?.Exit(npc);
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.IdleCounter > 0)
            {
                currentIdleSubstate = null;
                npc.IdleCounter--;
                return; // Don't switch substates yet
            }

            if (currentIdleSubstate == null || currentIdleSubstate.IsFinished)
            {
                if (currentIdleSubstate != null)
                    Main.NewText($"Previous substate finished: {currentIdleSubstate.GetType().Name}");

                // Pick a new idle substate
                currentIdleSubstate = PickSubstate(npc);

                if (currentIdleSubstate != null)
                {
                    Main.NewText("Switching to new Idle substate: " + currentIdleSubstate.GetType().Name);
                    npc.AIStateMachine.RegisterSubstate(currentIdleSubstate);
                    currentIdleSubstate.Enter(npc);
                }
            }

            currentIdleSubstate?.Update(npc);
        }

        private BaseIdleState PickSubstate(OvermorrowNPC npc)
        {
            return states
                .Where(s => s.state.CanExecute(npc))
                .OrderByDescending(s => s.weight) // or random weighted choice
                .FirstOrDefault().state;
        }
    }
}