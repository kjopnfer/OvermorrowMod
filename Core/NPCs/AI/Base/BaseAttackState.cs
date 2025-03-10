using OvermorrowMod.Core.NPCs;
using System.Collections.Generic;
using System;
using Terraria;
using OvermorrowMod.Common;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class BaseAttackState : State
    {
        public abstract int Weight { get; }
    }
}