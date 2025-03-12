using OvermorrowMod.Common;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class State
    {
        /// <summary>
        /// Determines if the state can currently be exited. Default is true.
        /// Subclasses can override this to prevent premature exits.
        /// </summary>
        public virtual bool CanExit => true;

        public abstract void Enter(OvermorrowNPC npc);
        public abstract void Exit(OvermorrowNPC npc);
        public abstract void Update(OvermorrowNPC npc);
        public virtual void OnInterrupt() { }  // Handles forced transitions
    }
}