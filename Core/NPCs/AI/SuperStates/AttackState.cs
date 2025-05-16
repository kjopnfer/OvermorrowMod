using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    public class AttackState : SuperState<BaseAttackState>
    {
        private List<(BaseAttackState state, int weight)> states;
        public bool HasValidAttack { get; private set; } = false;

        public AttackState(List<BaseAttackState> availableSubstates, OvermorrowNPC npc): base(availableSubstates, npc)
        {
            states = availableSubstates.Select(sub => (sub, sub.Weight)).ToList();
        }


        public override void Enter()
        {
            Main.NewText("NPC enters Attack state.");
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
            Main.NewText("NPC exits Attack state.");
        }

        public override void Update()
        {
            if (!OvermorrowNPC.TargetingModule.HasTarget())
            {
                // No target, exit back to AI decision elsewhere.
                return;
            }

            HasValidAttack = currentSubstate != null;
            if (currentSubstate?.IsFinished ?? true)
            {
                Main.NewText("attack state update: is finished");
                currentSubstate?.Exit();
                currentSubstate = null;
            }
            else
            {
                currentSubstate?.Update();
            }
        }

        /// <summary>
        /// Picks a valid attack substate based on CanExecute and weight.
        /// </summary>
        private BaseAttackState PickSubstate(OvermorrowNPC npc)
        {
            return states
             .Where(s => s.state.CanExecute(npc))
             .OrderByDescending(s => s.weight) // Or random weighted if preferred
             .FirstOrDefault().state;

            // Option 2: Weighted random pick (uncomment to use)
            // return PickWeightedRandom(validAttacks);
        }

        /// <summary>
        /// Optional method for weighted random attack selection.
        /// </summary>
        private BaseAttackState PickWeightedRandom(List<(BaseAttackState state, int weight)> weightedList)
        {
            int totalWeight = weightedList.Sum(s => s.weight);
            int randomValue = Main.rand.Next(totalWeight); // Terraria random
            int accumulatedWeight = 0;

            foreach (var (state, weight) in weightedList)
            {
                accumulatedWeight += weight;
                if (randomValue < accumulatedWeight)
                    return state;
            }

            return weightedList[0].state; // Fallback
        }
    }
}
