using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System;
using Terraria;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    public class AttackState : State
    {
        private Random _random = new Random();
        private float _timeBetweenAttacks = 1.0f;
        private float _attackTimer = 0.0f;
        private int _attackCount = 0;
        private int _maxAttacks = 3;
        private float _attackDuration = 5.0f;
        private float _stateTimer = 0.0f;
        private bool _isAttacking = false;

        private List<(BaseAttackState state, int weight)> attackStates;
        private BaseAttackState currentAttackSubstate;
        public AttackState(List<BaseAttackState> availableSubstates)
        {
            attackStates = new List<(BaseAttackState, int)>();
            foreach (var substate in availableSubstates)
            {
                attackStates.Add((substate, substate.Weight));  // Assume BaseIdleState has a `Weight` property
            }
        }

        public override void Enter(OvermorrowNPC npc)
        {
            _maxAttacks = _random.Next(1, 4);
            _attackCount = 0;
            _stateTimer = 0.0f;
            Main.NewText($"Starting attack sequence with {_maxAttacks} attacks.");
            currentAttackSubstate.Enter(npc);
        }

        public override void Exit(OvermorrowNPC npc)
        {
            currentAttackSubstate?.Exit(npc);
            Main.NewText("Exiting Attack State.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (!npc.TargetingModule.HasTarget())
            {
                // No target? Let AIStateMachine move us out of attack state
                return;
            }

            currentAttackSubstate?.Update(npc);
            /*_stateTimer += 0.1f;

            if (_stateTimer >= _attackDuration)
            {
                Console.WriteLine("Attack duration exceeded, transitioning to idle.");
                return;
            }

            if (!_isAttacking)
            {
                _attackTimer += 0.1f;
                if (_attackTimer >= _timeBetweenAttacks)
                {
                    _attackTimer = 0.0f;
                    PerformAttack(npc);
                }
            }*/
        }

        private void PerformAttack(OvermorrowNPC npc)
        {
            var selectedAttack = GetRandomAttack();
            selectedAttack.Enter(npc);
            selectedAttack.Update(npc);

            _attackCount++;

            if (_attackCount >= _maxAttacks)
            {
                _isAttacking = false;
                Console.WriteLine("Attack sequence complete.");
            }
            else
            {
                _isAttacking = true;
                Console.WriteLine("Continuing attack...");
            }
        }

        /*private State GetRandomAttack()
        {
            int totalWeight = 0;
            foreach (var attack in attackStates) totalWeight += attack.weight;

            int randomValue = _random.Next(0, totalWeight);
            int accumulatedWeight = 0;
            foreach (var attack in attackStates)
            {
                accumulatedWeight += attack.weight;
                if (randomValue < accumulatedWeight) return attack.attackState;
            }

            return attackStates[0].attackState;
        }*/
    }
}
