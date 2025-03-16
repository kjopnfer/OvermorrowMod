using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    public class AttackState : State
    {
        private List<(BaseAttackState state, int weight)> attackStates;
        public BaseAttackState currentAttackSubstate { get; private set; }

        public bool HasValidAttack { get; private set; } = false;

        public AttackState(List<BaseAttackState> availableSubstates)
        {
            attackStates = availableSubstates.Select(sub => (sub, sub.Weight)).ToList();
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Attack state.");
            currentAttackSubstate = PickSubstate(npc);

            HasValidAttack = currentAttackSubstate != null;
            if (HasValidAttack)
            {
                npc.AIStateMachine.RegisterSubstate(currentAttackSubstate);
                currentAttackSubstate?.Enter(npc);
            }
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentAttackSubstate?.Exit(npc);
            Main.NewText("NPC exits Attack state.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (!npc.TargetingModule.HasTarget())
            {
                // No target, exit back to AI decision elsewhere.
                return;
            }

            HasValidAttack = currentAttackSubstate != null;
            if (currentAttackSubstate?.IsFinished ?? true)
            {
                Main.NewText("is finished");
                currentAttackSubstate?.Exit(npc);
                currentAttackSubstate = null;
            }
            else
            {
                currentAttackSubstate?.Update(npc);
            }
        }

        /// <summary>
        /// Picks a valid attack substate based on CanExecute and weight.
        /// </summary>
        private BaseAttackState PickSubstate(OvermorrowNPC npc)
        {
            return attackStates
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
