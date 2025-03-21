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
        // TODO: Make these abstract instead.
        public virtual List<BaseIdleState> InitializeIdleStates() => new List<BaseIdleState> {
                new Wander()
        };
        
        public virtual List<BaseAttackState> InitializeAttackStates() => new List<BaseAttackState> {
                new GroundDashAttack()
        };

        public virtual List<BaseMovementState> InitializeMovementStates() => new List<BaseMovementState> {
                new MeleeWalk()
        };
    }
}