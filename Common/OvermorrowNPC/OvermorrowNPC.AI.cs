using Microsoft.Xna.Framework;
using OvermorrowMod.Content.NPCs;
using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public abstract partial class OvermorrowNPC : ModNPC
    {
        public ref float AICounter => ref NPC.ai[0];
        public ref float IdleCounter => ref NPC.ai[1];

        public AIStateMachine AIStateMachine = null;

        // TODO: Make these abstract instead.
        public virtual List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
                new Wander(this)
        };
        
        public virtual List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
                new GroundDashAttack(this)
        };

        public virtual List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
                new MeleeWalk(this)
        };
    }
}