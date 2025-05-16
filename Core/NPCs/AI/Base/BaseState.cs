using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class BaseState : State
    {
        /// <summary>
        /// Allows for certain states to have higher priority than others.
        /// </summary>
        public abstract int Weight { get; }
        public bool IsFinished { get; protected set; } = false;
        public BaseState(OvermorrowNPC npc) : base(npc) { }

        /// <summary>
        /// By default, assume this state can execute unless overridden.
        /// </summary>
        public virtual bool CanExecute(OvermorrowNPC npc) => true;

        /// <summary>
        /// By default, states cannot be exited until finished.
        /// </summary>
        public override bool CanExit => IsFinished;
    }

    public abstract class BaseIdleState : BaseState
    {
        protected BaseIdleState(OvermorrowNPC npc) : base(npc) { }
    }

    public abstract class BaseAttackState : BaseState
    {
        protected BaseAttackState(OvermorrowNPC npc) : base(npc) { }
    }

    public abstract class BaseMovementState : BaseState
    {
        protected BaseMovementState(OvermorrowNPC npc) : base(npc) { }
    }

}
