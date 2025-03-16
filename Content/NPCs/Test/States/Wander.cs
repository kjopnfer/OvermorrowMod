using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class Wander : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => true;

        public override void Enter(OvermorrowNPC npc)
        {
            npc.NPC.velocity.X = 0;
            npc.NPC.RemoveStealth();

            Main.NewText("enter");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("exited wander");
        }

        public override void Update(OvermorrowNPC npc)
        {
            Main.NewText("the wander state");
        }
    }
}