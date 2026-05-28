using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    public class AttackState : SuperState<BaseAttackState>
    {
        public bool HasValidAttack { get; private set; } = false;

        public AttackState(List<BaseAttackState> availableSubstates, OvermorrowNPC npc) : base(availableSubstates, npc)
        {
        }


        public override void Enter()
        {
            currentSubstate = PickSubstate(OvermorrowNPC);

            HasValidAttack = currentSubstate != null;
            if (HasValidAttack)
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
            if (!OvermorrowNPC.TargetingModule.HasTarget())
            {
                // If the NPC is somehow stuck attacking but cannot finish, exit out of it.
                if (currentSubstate != null)
                {
                    currentSubstate.Exit();
                }

                // No target, exit back to AI decision elsewhere.
                return;
            }

            HasValidAttack = currentSubstate != null;
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

        /// <summary>
        /// Picks a valid attack substate weighted by each substate's weight.
        /// </summary>
        private BaseAttackState PickSubstate(OvermorrowNPC npc)
        {
            if (substates == null || substates.Count == 0)
                return null;

            return PickWeightedRandom(substates.Where(s => s.CanExecute()).ToList());
        }
    }
}
