using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class ChairSummonAnimation : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        private int failCount = 0;
        public ChairSummonAnimation(OvermorrowNPC npc) : base(npc) { }

        public override void Enter()
        {
            int maxAttempts = 1000;
            while (!Collision.SolidTiles((int)(NPC.position.X / 16), (int)((NPC.position.X + NPC.width) / 16), (int)((NPC.position.Y + NPC.height) / 16), (int)((NPC.position.Y + NPC.height + 1) / 16)) && failCount < maxAttempts)
            {
                NPC.position.Y += 1; // Move the NPC downward
                failCount++; // Increment the fail count to avoid infinite loops
            }

            Main.NewText("entered chair summon state");
            OvermorrowNPC.AICounter = 0;

            IsFinished = false;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;

            OvermorrowNPC.AIStateMachine.RemoveSubstate<ChairSummonAnimation>(AIStateType.Idle, new ChairSummonAnimation(OvermorrowNPC));
            Main.NewText("wtf remove hidden and add idle");
            var newIdle = new GrimoireIdle(OvermorrowNPC);
            OvermorrowNPC.AIStateMachine.AddSubstate(AIStateType.Idle, newIdle);

            Main.NewText("exited hidden state");
        }

        public override void Update()
        {
            if (OvermorrowNPC.AICounter++ >= 120)
            {
                IsFinished = true;
            }
        }
    }
}