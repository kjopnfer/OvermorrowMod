using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using Terraria;
using static log4net.Appender.RollingFileAppender;

namespace OvermorrowMod.Content.NPCs
{
    public class Hop : BaseMovementState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;
        public Hop(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            //return true;
            return OvermorrowNPC.IdleCounter <= 0;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;

            int jumpDirection = Main.rand.Next(2, 6) * NPC.direction;
            int jumpHeight = Main.rand.Next(-10, -4);
            NPC.velocity = new Vector2(jumpDirection, jumpHeight);

            IsFinished = false;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;

        }

        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            if (((NPC.collideY && NPC.velocity.Y == 0) || (NPC.collideX && NPC.velocity.X == 0)) && OvermorrowNPC.AICounter > 24)
            {
                NPC.velocity.X = 0;
                OvermorrowNPC.IdleCounter = Main.rand.Next(3, 4) * 10;

                IsFinished = true;
            }
        }
    }
}