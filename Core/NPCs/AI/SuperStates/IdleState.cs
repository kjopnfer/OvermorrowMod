using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class IdleState : SuperState<BaseIdleState>
    {
        public IdleState(List<BaseIdleState> availableSubstates, OvermorrowNPC npc) : base(availableSubstates, npc)
        {
        }

        public override void Enter()
        {
            //currentSubstate = PickSubstate(npc);
            //currentSubstate.Enter(npc);
            //Main.NewText(OvermorrowNPC.Name + " enters Idle state.");
            currentSubstate = null;
        }

        public override void Exit()
        {
            currentSubstate?.Exit();
        }

        public override void Update()
        {
            if (OvermorrowNPC.IdleCounter > 0)
            {
                currentSubstate = null;
                OvermorrowNPC.IdleCounter--;
                return; // Don't switch substates yet
            }

            if (currentSubstate == null || currentSubstate.IsFinished)
            {
                if (currentSubstate != null)
                {
                    currentSubstate.Exit();
                    //Main.NewText($"Previous substate finished: {currentSubstate.GetType().Name}");
                }

                // Pick a new idle substate
                currentSubstate = PickSubstate(OvermorrowNPC);

                if (currentSubstate != null)
                {
                    Main.NewText("Switching to new Idle substate: " + currentSubstate.GetType().Name);
                    OvermorrowNPC.AIStateMachine.RegisterSubstate(currentSubstate);
                    currentSubstate.Enter();
                }
            }

            currentSubstate?.Update();
        }

        private BaseIdleState PickSubstate(OvermorrowNPC npc)
        {
            if (substates == null || substates.Count == 0)
                return null;

            return substates
                .Where(s => s.CanExecute())
                .OrderByDescending(s => s.Weight)
                .FirstOrDefault();
            /*return states
                .Where(s => s.state.CanExecute(npc))
                .OrderByDescending(s => s.weight) // or random weighted choice
                .FirstOrDefault().state;*/
        }
    }
}