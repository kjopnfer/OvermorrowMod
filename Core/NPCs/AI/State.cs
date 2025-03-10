using OvermorrowMod.Common;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class State
    {
        public abstract void Enter(OvermorrowNPC npc);
        public abstract void Exit(OvermorrowNPC npc);
        public abstract void Update(OvermorrowNPC npc);
        public virtual void OnInterrupt() { }  // Handles forced transitions
    }
}