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

        public IdleState(List<BaseIdleState> availableSubstates) : base(availableSubstates)
        {
            states = new List<(BaseIdleState, int)>();
            foreach (var sub in availableSubstates)
                states.Add((sub, sub.Weight));
        }

        public override void Enter(OvermorrowNPC npc)
        {
            //currentSubstate = PickSubstate(npc);
            //currentSubstate.Enter(npc);
            Main.NewText(npc.Name + " enters Idle state.");
            currentSubstate = null;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentSubstate?.Exit(npc);
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.IdleCounter > 0)
            {
                currentSubstate = null;
                npc.IdleCounter--;
                return; // Don't switch substates yet
            }

            if (currentSubstate == null || currentSubstate.IsFinished)
            {
                if (currentSubstate != null)
                {
                    currentSubstate.Exit(npc);
                    //Main.NewText($"Previous substate finished: {currentSubstate.GetType().Name}");
                }

                // Pick a new idle substate
                currentSubstate = PickSubstate(npc);

                if (currentSubstate != null)
                {
                    Main.NewText("Switching to new Idle substate: " + currentSubstate.GetType().Name);
                    npc.AIStateMachine.RegisterSubstate(currentSubstate);
                    currentSubstate.Enter(npc);
                }
            }

            currentSubstate?.Update(npc);
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