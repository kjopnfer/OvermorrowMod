using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class ClockworkSpiderSwap : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        int swapCooldown = 0;
        int bounceCount = 0;
        float currentBounceVelocity = 20f;
        bool gravityFlipped;

        public ClockworkSpiderSwap(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                return false;

            if (swapCooldown-- > 0)
                return false;

            var target = OvermorrowNPC.TargetingModule.Target;
            float xDistance = Math.Abs(NPC.Center.X - target.Center.X);

            return xDistance <= ModUtils.TilesToPixels(10) && NPC.collideY;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
        }

        public override void Exit()
        {
            swapCooldown = ModUtils.SecondsToTicks(2);
            OvermorrowNPC.AICounter = 0;
        }

        public override void Update()
        {
            NPC.velocity.X = 0;

            ClockworkSpider spider = NPC.ModNPC as ClockworkSpider;

            OvermorrowNPC.AICounter++;
            if (OvermorrowNPC.AICounter == 30)
            {
                if (NPC.noGravity)
                {
                    // hahahahah velocity y canot go past 10 for some reason???
                    // why did they feel the need to cap this
                    NPC.velocity.Y = 99999999;
                    NPC.noGravity = false;
                }
                else
                {
                    NPC.velocity.Y = -20;
                    NPC.noGravity = true;
                }
            }

            if (OvermorrowNPC.AICounter > 30 && NPC.collideY)
            {
                if (!NPC.noGravity)
                {
                    Main.NewText("should bounce");
                    NPC.velocity.Y = -8;
                    //NPC.velocity.Y *= -1;
                }
                else
                {
                    NPC.velocity.Y = 2;
                }


                IsFinished = true;
                NPC.frameCounter = 0;
            }
        }
    }
}