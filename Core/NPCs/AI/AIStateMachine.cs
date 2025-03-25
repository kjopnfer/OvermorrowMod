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
        /// Maximum number of substates to remember
        /// </summary>
        private readonly int substateHistorySize = 5;

        /// <summary>
        /// Queue to store past states, up to 'historySize' number of states.
        /// </summary>
        private readonly Queue<State> stateHistory = new Queue<State>();

        /// <summary>
        /// Queue to store past substates, up to 'substateHistorySize' number of substates.
        /// </summary>
        private readonly Queue<State> substateHistory = new Queue<State>();

        /// <summary>
        /// Accessor for previous states, returned from most recent to oldest.
        /// </summary>
        public IEnumerable<State> GetPreviousStates() => stateHistory.Reverse();

        /// <summary>
        /// Get the current active state.
        /// </summary>
        public State GetCurrentState() => currentState;

        /// <summary>
        /// Accessor for previous substates, returned from most recent to oldest.
        /// </summary>
        public IEnumerable<State> GetPreviousSubstates() => substateHistory.Reverse();

        /// <summary>
        /// Get current active substate (most recent one registered).
        /// </summary>
        public State GetCurrentSubstate() => substateHistory.LastOrDefault();

        /// <summary>
        /// Dictionary of available superstates (Idle, Moving, Attacking).
        /// </summary>
        private readonly Dictionary<AIStateType, State> availableStates = new Dictionary<AIStateType, State>();

        /// <summary>
        /// Initializes the state machine and injects substates into each superstate.
        /// </summary>
        public AIStateMachine(OvermorrowNPC npc, List<BaseIdleState> idleSubstates, List<BaseMovementState> movementSubstates, List<BaseAttackState> attackSubstates)
        {
            availableStates[AIStateType.Idle] = new IdleState(idleSubstates);
            availableStates[AIStateType.Moving] = new MovementState(movementSubstates);
            availableStates[AIStateType.Attacking] = new AttackState(attackSubstates);

            SetInitialState(npc);
        }

        public void RemoveSubstate<T>(AIStateType superstateType) where T : BaseState
        {
            if (availableStates.TryGetValue(superstateType, out var state))
            {
                if (state is SuperState<BaseState> superstate)
                {
                    var removeMethod = state.GetType().GetMethod("RemoveSubstate")?.MakeGenericMethod(typeof(T));
                    removeMethod?.Invoke(state, null);
                }
            }
        }

        /// <summary>
        /// Sets the initial default state (Idle) and enters it.
        /// </summary>
        private void SetInitialState(OvermorrowNPC npc)
        {
            currentState = availableStates[AIStateType.Idle];
            currentState.Enter(npc); // Null if NPC not yet passed
            stateHistory.Enqueue(currentState);
        }

        /// <summary>
        /// Register a new substate in history.
        /// </summary>
        public void RegisterSubstate(State substate)
        {
            if (substate == null) return;

            if (substateHistory.Count >= substateHistorySize)
                substateHistory.Dequeue(); // Remove oldest

            substateHistory.Enqueue(substate); // Add new
        }


        /// <summary>
        /// Change to a new state if it's different from the current one.
        /// </summary>
        public void ChangeState(AIStateType newState, OvermorrowNPC npc)
        {
            if (availableStates.TryGetValue(newState, out var nextState) && currentState != nextState && (currentState?.CanExit ?? true))
            {
                if (newState == AIStateType.Attacking && stateHistory.LastOrDefault() == nextState)
                {
                    Main.NewText("prevent duplicate attack");
                    return;
                }

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
                float distanceToTarget = Vector2.Distance(npc.NPC.Center, npc.TargetingModule.Target.Center);

                // If already in an attack state, do not switch to moving unless no valid attack remains
                if (currentState is AttackState attackState)
                {
                    if (!attackState.HasValidAttack) // No valid attack, allow movement
                    {
                        ChangeState(AIStateType.Moving, npc);
                    }
                    else
                    {
                        return; // Stay in the attack state
                    }
                }
                else
                {
                    // If too far away, move toward the target
                    if (distanceToTarget > 10 * 16)
                    {
                        ChangeState(AIStateType.Moving, npc);
                    }
                    else
                    {
                        ChangeState(AIStateType.Attacking, npc);

                        // If no valid attack found after switching, fallback to moving
                        if (currentState is AttackState newAttackState && !newAttackState.HasValidAttack)
                        {
                            ChangeState(AIStateType.Moving, npc);
                        }
                    }
                }
            }
            else
            {
                if (npc.SpawnerID.HasValue)
                {
                    //Main.NewText("yo 2" + npc.Name);

                }
                ChangeState(AIStateType.Idle, npc);
            }
        }
    }
}
