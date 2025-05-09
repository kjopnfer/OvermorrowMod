﻿using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class BaseState : State
    {
        /// <summary>
        /// Allows for certain states to have higher priority than others.
        /// </summary>
        public abstract int Weight { get; }
        public bool IsFinished { get; protected set; } = false;

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
    }

    public abstract class BaseAttackState : BaseState
    {
    }

    public abstract class BaseMovementState : BaseState
    {
    }
}
