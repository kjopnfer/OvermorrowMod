using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public enum AIStateType
    {
        Idle,
        Moving,
        Attacking
    }

    public class AIStateMachine
    {
        private State currentState;

        /// <summary>
        /// Maximum number of states to remember
        /// </summary>
        private readonly int historySize = 5;

        /// <summary>
        /// Queue to store past states, up to 'historySize' number of states.
        /// </summary>
        private readonly Queue<State> stateHistory = new Queue<State>();

        /// <summary>
        /// Accessor for previous states, returned from most recent to oldest.
        /// </summary>
        public IEnumerable<State> GetPreviousStates() => stateHistory.Reverse();

        /// <summary>
        /// Get the current active state.
        /// </summary>
        public State GetCurrentState() => currentState;

        //private List<State> availableStates = new List<State> { new IdleState(), new AttackState(), new MovementState() };
        /// <summary>
        /// Dictionary of available superstates (Idle, Moving, Attacking).
        /// </summary>
        private readonly Dictionary<AIStateType, State> availableStates = new Dictionary<AIStateType, State>();

        /// <summary>
        /// Initializes the state machine and injects substates into each superstate.
        /// </summary>
        public AIStateMachine(List<BaseIdleState> idleSubstates, List<BaseMovementState> movementSubstates, List<BaseAttackState> attackSubstates)
        {
            availableStates[AIStateType.Idle] = new IdleState(idleSubstates);
            availableStates[AIStateType.Moving] = new MovementState(movementSubstates);
            availableStates[AIStateType.Attacking] = new AttackState(attackSubstates);

            SetInitialState();
        }


        /// <summary>
        /// Sets the initial default state (Idle) and enters it.
        /// </summary>
        private void SetInitialState()
        {
            currentState = availableStates[AIStateType.Idle];
            currentState.Enter(null); // Null if NPC not yet passed
            stateHistory.Enqueue(currentState);
        }

        /// <summary>
        /// Change to a new state if it's different from the current one.
        /// </summary>
        public void ChangeState(AIStateType newState, OvermorrowNPC npc)
        {
            if (availableStates.TryGetValue(newState, out var nextState) && currentState != nextState && (currentState?.CanExit ?? true))
            {
                currentState?.Exit(npc);  // Exit the current state if any

                if (stateHistory.Count >= historySize)
                {
                    stateHistory.Dequeue(); // Remove the oldest state
                }

                currentState = nextState;
                currentState.Enter(npc);  // Enter the new state

                stateHistory.Enqueue(currentState); // Add the new state to the history
            }
        }

        public void Update(OvermorrowNPC npc)
        {
            EvaluateState(npc);
            currentState?.Update(npc);
        }

        // Select the next state based on conditions
        public void EvaluateState(OvermorrowNPC npc)
        {
            // Prevent evaluating state change if locked in current state
            if (!(currentState?.CanExit ?? true))
                return;

            if (npc.TargetingModule.HasTarget())
            {
                // Somehow decide between either moving towards the target or attacking.
                // If the distance is too far away, move:
                if (Vector2.Distance(npc.NPC.Center, npc.TargetingModule.Target.Center) < npc.TargetingConfig().MaxTargetRange)
                {

                }
                else // Otherwise:
                {

                }

                ChangeState(AIStateType.Attacking, npc); // Switch to AttackState if NPC has a target
            }
            else
            {
                ChangeState(AIStateType.Idle, npc);
            }
        }
    }
}
