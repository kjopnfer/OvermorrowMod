using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class MovementState : SuperState<BaseMovementState>
    {
        public MovementState(List<BaseMovementState> availableSubstates, OvermorrowNPC npc) : base(availableSubstates, npc)
        {
        }

        public override void Enter()
        {
            Main.NewText("NPC enters Movement state");
            currentSubstate = PickSubstate(OvermorrowNPC);

            OvermorrowNPC.AIStateMachine.RegisterSubstate(currentSubstate);
            currentSubstate?.Enter();
        }

        public override void Exit()
        {
            currentSubstate?.Exit();
            Main.NewText("NPC exits Move state.");
        }

        public override void Update()
        {
            if (currentSubstate == null || currentSubstate.IsFinished)
            {
                currentSubstate = PickSubstate(OvermorrowNPC);
                currentSubstate.Enter();
            }

            currentSubstate?.Update();
        }

        private BaseMovementState PickSubstate(OvermorrowNPC npc)
        {
            if (substates == null || substates.Count == 0)
                return null;

            return substates
                .Where(s => s.CanExecute(npc))
                .OrderByDescending(s => s.Weight) // or random weighted choice
                .FirstOrDefault();
        }
    }
}