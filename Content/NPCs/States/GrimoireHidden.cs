using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireHidden : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => false;

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("entered hidden state");
            npc.AICounter = 0;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.NPC.noGravity = true;
            npc.AICounter = 0;

            Main.NewText("exited hidden state");
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.TargetingModule.HasTarget())
            {
                npc.NPC.noGravity = false;

                if (npc.AICounter++ >= 36)
                {
                    //IsFinished = true;
                }
            }
        }
    }
}