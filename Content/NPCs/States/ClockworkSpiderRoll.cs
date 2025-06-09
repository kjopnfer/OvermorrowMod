using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class ClockworkSpiderRoll : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        public ClockworkSpiderRoll(OvermorrowNPC npc) : base(npc) { }

        int rollCooldown = 0;
        public override bool CanExecute()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                return false;


            if (rollCooldown-- > 0)
                return false;

            return NPC.collideY && !NPC.noGravity;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            rollCooldown = ModUtils.SecondsToTicks(2);
        }

        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            if (OvermorrowNPC.AICounter >= 30 && OvermorrowNPC.AICounter <= ModUtils.SecondsToTicks(3))
            {
                // Roll for 30 frames
                NPC.velocity.X = NPC.direction * 12f; // Adjust speed as needed
                NPC.velocity.Y = 0; // Keep it horizontal

                NPC.rotation += NPC.direction * 0.6f; // Rotate while rolling
            }
            else if (OvermorrowNPC.AICounter >= ModUtils.SecondsToTicks(3))
            {
                IsFinished = true; // End the roll after 30 frames
                NPC.velocity.X = 0; // Stop movement
                NPC.noGravity = false;
            }
        }
    }
}