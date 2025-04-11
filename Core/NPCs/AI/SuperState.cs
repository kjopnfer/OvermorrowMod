using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class SuperState<T> : State where T : BaseState
    {
        protected List<T> substates;
        public IReadOnlyList<T> Substates => substates;

        public T currentSubstate { get; protected set; }
        public override bool CanExit => base.CanExit && (currentSubstate?.CanExit ?? true);

        public SuperState(List<T> substates)
        {
            this.substates = substates;
        }

        public bool ContainsSubstate(State substate)
        {
            return substates.Contains((T)substate);
        }

        /// <summary>
        /// Removes a specific substate from this superstate.
        /// </summary>
        public void RemoveSubstate<U>() where U : T
        {
            substates.RemoveAll(s => s.GetType() == typeof(U));
        }

        /// <summary>
        /// Adds a new substate to this superstate if it matches the type.
        /// </summary>
        public void AddSubstate(T substate)
        {
            if (substate == null || substates.Any(s => s.GetType() == substate.GetType()))
                return; // Avoid adding duplicates

            substates.Add(substate);
        }

        public void SetSubstate(State substate, OvermorrowNPC npc)
        {
            if (ContainsSubstate(substate))
                return;

            if (currentSubstate != substate)
            {
                currentSubstate?.Exit(npc);
                currentSubstate = (T)substate;
                currentSubstate.Enter(npc);
            }
        }
    }
}