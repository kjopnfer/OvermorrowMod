using OvermorrowMod.Common;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class SuperState<T> : State where T : BaseState
    {
        protected List<T> substates;

        public SuperState(List<T> substates)
        {
            this.substates = substates;
        }

        /// <summary>
        /// Removes a specific substate from this superstate.
        /// </summary>
        public void RemoveSubstate<U>() where U : T
        {
            substates.RemoveAll(s => s.GetType() == typeof(U));
        }
    }
}