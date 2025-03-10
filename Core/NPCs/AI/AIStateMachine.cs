using OvermorrowMod.Common;
using System.Collections.Generic;
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
        //private List<State> availableStates = new List<State> { new IdleState(), new AttackState(), new MovementState() };
        private Dictionary<AIStateType, State> availableStates = new Dictionary<AIStateType, State>();

        public AIStateMachine(List<BaseIdleState> idleSubstates, List<BaseMovementState> movementSubstates, List<BaseAttackState> attackSubstates)
        {
            availableStates[AIStateType.Idle] = new IdleState(idleSubstates);
            availableStates[AIStateType.Moving] = new MovementState(movementSubstates);
            availableStates[AIStateType.Attacking] = new AttackState(attackSubstates);

            SetInitialState();
        }

        // Set initial state to IdleState
        private void SetInitialState()
        {
            currentState = availableStates[AIStateType.Idle];
            currentState.Enter(null); // Enter initial state
        }

        public void ChangeState(AIStateType newState, OvermorrowNPC npc)
        {
            if (availableStates.ContainsKey(newState) && currentState != availableStates[newState])
            {
                currentState?.Exit(npc);  // Exit the current state if any
                currentState = availableStates[newState];
                currentState.Enter(npc);  // Enter the new state
            }
        }

        // Update the current state
        public void Update(OvermorrowNPC npc)
        {
            currentState?.Update(npc);
        }

        // Select the next state based on conditions
        public void EvaluateState(OvermorrowNPC npc)
        {
            if (npc.TargetingModule.HasTarget())
            {
                ChangeState(AIStateType.Attacking, npc); // Switch to AttackState if NPC has a target
            }
            else
            {
                ChangeState(AIStateType.Idle, npc);
            }
        }
    }
}
