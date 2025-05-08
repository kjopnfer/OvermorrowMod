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
        public override bool CanExit => IsFinished;

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("entered hidden state");
            npc.AICounter = 0;
            IsFinished = false;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.NPC.noGravity = true;
            npc.AICounter = 0;

            npc.AIStateMachine.RemoveSubstate<GrimoireHidden>(AIStateType.Idle, new GrimoireHidden());
            Main.NewText("wtf remove hidden and add idle");
            var newIdle = new GrimoireIdle();
            npc.AIStateMachine.AddSubstate(AIStateType.Idle, newIdle);
            //npc.AIStateMachine.SetSubstate<GrimoireIdle>(AIStateType.Idle, npc);

            Main.NewText("exited hidden state");
        }

        public override void Update(OvermorrowNPC npc)
        {
            //Main.NewText("hidden");

            if (npc.TargetingModule.HasTarget())
            {

                npc.NPC.noGravity = false;
                //Main.NewText("yt " + npc.AICounter);
                if (npc.AICounter++ >= 36)
                {
                    IsFinished = true;
                }
            }
        }
    }
}