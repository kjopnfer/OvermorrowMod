using OvermorrowMod.Common;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    public interface IAIState
    {
        void Enter(OvermorrowNPC npc);
        bool CanExecute(OvermorrowNPC npc);
        void Execute(OvermorrowNPC npc);
        void Exit(OvermorrowNPC npc);
    }   
}