using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using System;
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
        private bool hasInitializedInitialState = false;

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
        /// Get current active substate (most recent one registered).
        /// </summary>
        public State GetCurrentSubstate()
        {
            if (currentState == null)
                return null;

            // Look for a base type matching SuperState<something>
            var type = currentState.GetType();
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SuperState<>))
                {
                    var substateProperty = type.GetProperty("currentSubstate");
                    if (substateProperty != null)
                        return substateProperty.GetValue(currentState) as State;
                }

                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Accessor for previous substates, returned from most recent to oldest.
        /// </summary>
        public IEnumerable<State> GetPreviousSubstates() => substateHistory.Reverse();

        /// <summary>
        /// Dictionary of available superstates (Idle, Moving, Attacking).
        /// </summary>
        private readonly Dictionary<AIStateType, State> availableStates = new Dictionary<AIStateType, State>();

        /// <summary>
        /// Initializes the state machine and injects substates into each superstate.
        /// </summary>
        public AIStateMachine(OvermorrowNPC npc, List<BaseIdleState> idleSubstates, List<BaseMovementState> movementSubstates, List<BaseAttackState> attackSubstates)
        {
            availableStates[AIStateType.Idle] = new IdleState(idleSubstates, npc);
            availableStates[AIStateType.Moving] = new MovementState(movementSubstates, npc);
            availableStates[AIStateType.Attacking] = new AttackState(attackSubstates, npc);

            SetInitialState(npc);
        }

        public void RemoveSubstate<T>(AIStateType superstateType, T substate) where T : BaseState
        {
            if (availableStates.TryGetValue(superstateType, out var state))
            {
                var type = state.GetType();
                var method = state.GetType().GetMethod("RemoveSubstate", Type.EmptyTypes);

                if (method != null)
                {
                    var generic = method?.MakeGenericMethod(typeof(T));
                    generic?.Invoke(state, null);
                }
                else
                {
                    Main.NewText("method not found");
                }
            }
        }

        public void AddSubstate<T>(AIStateType superstateType, T substate) where T : BaseState
        {
            if (availableStates.TryGetValue(superstateType, out var state))
            {
                var type = state.GetType();
                var method = type.GetMethod("AddSubstate");

                if (method != null)
                {
                    method.Invoke(state, new object[] { substate });
                }
                else
                {
                    Main.NewText("method not found");
                }
            }
        }

        /*public void ForceSetSubstate(State substate, OvermorrowNPC npc)
        {
            foreach (var state in availableStates.Values)
            {
                if (state is SuperState<BaseState> superstate && superstate.ContainsSubstate(substate))
                {
                    superstate.SetSubstate(substate, npc);
                    RegisterSubstate(substate); // Optional: track it in substate history
                    return;
                }
            }

            Main.NewText("ForceSetSubstate failed: no superstate found for substate " + substate.GetType().Name, Color.Red);
        }
        */

        // This fucking shit doesnt work
        public void SetSubstate<T>(AIStateType superstateType, OvermorrowNPC npc) where T : State
        {
            if (!availableStates.TryGetValue(superstateType, out State rawState))
            {
                throw new Exception($"Superstate {superstateType} not found.");
            }

            // Use dynamic to bypass the generic constraint issue
            dynamic dynamicSuperstate = rawState;

            // Find the substate of type T in the dynamic list of substates
            State substate = ((IEnumerable<State>)dynamicSuperstate.Substates)
                .FirstOrDefault(s => s is T);

            if (substate == null)
            {
                throw new Exception($"Substate of type {typeof(T).Name} not found in superstate {superstateType}.");
            }

            //ChangeState(superstateType, npc);

            // Call the dynamic SetSubstate method
            //Main.NewText("set substate to " + substate.ToString());
            dynamicSuperstate.SetSubstate(substate, npc);
        }


        /// <summary>
        /// Checks if a given substate is present in the superstate's list.
        /// </summary>
        private bool HasSubstate(SuperState<BaseState> superstate, State substate)
        {
            return superstate.Substates.Any(s => s.GetType() == substate.GetType());
        }

        /// <summary>
        /// Sets the initial default state (Idle) and enters it.
        /// </summary>
        private void SetInitialState(OvermorrowNPC npc)
        {
            currentState = availableStates[AIStateType.Idle];
            currentState.Enter(); // Null if NPC not yet passed
            stateHistory.Enqueue(currentState);

            hasInitializedInitialState = false;
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

                currentState?.Exit();  // Exit the current state if any

                if (stateHistory.Count >= historySize)
                {
                    stateHistory.Dequeue(); // Remove the oldest state
                }

                currentState = nextState;
                currentState.Enter();  // Enter the new state

                stateHistory.Enqueue(currentState); // Add the new state to the history
            }
        }

        public void Update(OvermorrowNPC npc)
        {
            EvaluateState(npc);
            currentState?.Update();
        }

        // Select the next state based on conditions
        public void EvaluateState(OvermorrowNPC npc)
        {
            // Prevent evaluating state change if locked in current state
            if (!(currentState?.CanExit ?? true))
            {
                return;
            }

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
                    if (currentState is MovementState moveState && !moveState.HasValidMovement)
                    {
                        ChangeState(AIStateType.Idle, npc);
                        return;
                    }

                    // If too far away, move toward the target
                    if (distanceToTarget > npc.TargetingModule.Config.MaxAttackRange)
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

                }
                ChangeState(AIStateType.Idle, npc);
            }
        }
    }
}
