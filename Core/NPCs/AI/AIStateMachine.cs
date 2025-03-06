using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public class AIStateMachine
    {
        private IAIState currentState;
        public AIStateMachine(IAIState state)
        {
            currentState = state;
        }

        public void SetState(IAIState state, NPC npc)
        {
            currentState.Exit(npc);
            currentState = state;
            currentState.Enter(npc);
        }

        public void Update(NPC npc)
        {
            currentState.Execute(npc);
        }
    }
}