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
        private BaseAttackState currentAttackSubstate;

        public AttackState(List<BaseAttackState> availableSubstates)
        {
            // Build weighted list of available attack substates
            attackStates = new List<(BaseAttackState, int)>();
            foreach (var sub in availableSubstates)
                attackStates.Add((sub, sub.Weight));
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("NPC enters Attack state.");
            currentAttackSubstate = PickSubstate(npc);
            currentAttackSubstate?.Enter(npc);
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

            // Update current attack substate if active
            currentAttackSubstate?.Update(npc);

            // If the substate has finished, pick a new one
            if (currentAttackSubstate == null || currentAttackSubstate.IsFinished)
            {
                currentAttackSubstate?.Exit(npc); // Ensure proper exit
                currentAttackSubstate = PickSubstate(npc);
                currentAttackSubstate?.Enter(npc);
            }
        }

        /// <summary>
        /// Picks a valid attack substate based on CanExecute and weight.
        /// </summary>
        private BaseAttackState PickSubstate(OvermorrowNPC npc)
        {
            var validAttacks = attackStates.Where(s => s.state.CanExecute(npc)).ToList();
            if (validAttacks.Count == 0)
                return null; // No valid attacks, optionally fallback to something

            // Option 1: Pick based on highest weight (deterministic)
            return validAttacks.OrderByDescending(s => s.weight).FirstOrDefault().state;

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
