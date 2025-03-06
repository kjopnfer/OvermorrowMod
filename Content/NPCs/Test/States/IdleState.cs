using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class IdleState : IAIState
    {
        public void CanExecute(NPC npc)
        {
        }

        public void Enter(NPC npc)
        {
        }

        public void Execute(NPC npc)
        {
            npc.ai[0]++;

            if (npc.ai[0] % 20 == 0)
            {
                Main.NewText("hi " + npc.ai[0]);
            }
        }

        public void Exit(NPC npc)
        {
        }
    }
}