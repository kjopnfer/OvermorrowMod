using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class State
    {
        /// <summary>
        /// Determines if the state can currently be exited. Default is true.
        /// Subclasses can override this to prevent premature exits.
        /// </summary>
        public virtual bool CanExit => true;
        protected OvermorrowNPC OvermorrowNPC { get; }
        protected NPC NPC => OvermorrowNPC.NPC;

        protected State(OvermorrowNPC npc)
        {
            OvermorrowNPC = npc;
        }

        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
        public virtual void OnInterrupt() { }  // Handles forced transitions
    }
}