using OvermorrowMod.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public abstract class SuperState<T> : State where T : BaseState
    {
        protected List<T> substates;
        public IReadOnlyList<T> Substates => substates;

        public T currentSubstate { get; protected set; }
        public override bool CanExit => base.CanExit && (currentSubstate?.CanExit ?? true);

        public SuperState(List<T> substates, OvermorrowNPC npc) : base(npc)
        {
            this.substates = substates;
        }


        public bool ContainsSubstate(State substate)
        {
            string listContents = string.Join(", ", substates.Select(s => s.GetType().Name));
            Main.NewText("Checking for substates: [" + listContents + "]");

            return substates.Contains((T)substate);
        }

        /// <summary>
        /// Removes a specific substate from this superstate.
        /// </summary>
        public void RemoveSubstate<U>() where U : T
        {
            substates.RemoveAll(s => s.GetType() == typeof(U));
        }

        public void RemoveSubstate(BaseState substate)
        {
            substates.Remove((T)substate);
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
            if (!ContainsSubstate(substate))
            {
                Main.NewText("substate not found, returning");
                return;
            }

            if (currentSubstate != substate)
            {
                currentSubstate?.Exit();
                currentSubstate = (T)substate;
                currentSubstate.Enter();
            }
        }
    }
}