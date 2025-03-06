using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public interface IAIState
    {
        void Enter(NPC npc);
        void CanExecute(NPC npc);
        void Execute(NPC npc);
        void Exit(NPC npc);
    }
}