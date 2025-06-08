using Microsoft.Xna.Framework;
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

        int numBounces = 0;
        public override void Update()
        {
            NPC.velocity.X = 0;

            ClockworkSpider spider = NPC.ModNPC as ClockworkSpider;

            if (NPC.noGravity)
            {
                NPC.velocity.Y -= 0.08f;
            }

            OvermorrowNPC.AICounter++;

            if (OvermorrowNPC.AICounter == 30)
            {
                Main.NewText("do first bounce");
                if (NPC.noGravity)
                {
                    NPC.velocity.Y = 3;
                }
                else
                {
                    NPC.velocity.Y = -6;
                }
            }

            if (OvermorrowNPC.AICounter > 30 && NPC.collideY && numBounces == 0)
            {
                Main.NewText("I DID THE FIRS BOUNCEdo second bounce");
                numBounces++;

                if (NPC.noGravity)
                {
                    // hahahahah velocity y canot go past 10 for some reason???
                    // why did they feel the need to cap this
                    //NPC.velocity.Y = 99999999;
                    NPC.velocity.Y = 8; // its always 10 its always 10?? huh??
                    Main.NewText("HOLY SHIT FREDDIE MERCURY MAMA OOOOOOO", Color.Red);
                    NPC.noGravity = false;
                }
                else
                {
                    Main.NewText("actiave tno at " + numBounces);
                    NPC.velocity.Y = -20;
                    NPC.noGravity = true;
                }
            }

            if (!NPC.noGravity) Main.NewText(NPC.velocity.Y);

            var DUMBASSDELAY = !NPC.noGravity ? 110 : 40;
            if (OvermorrowNPC.AICounter > DUMBASSDELAY && NPC.collideY && numBounces == 1)
            {
                if (NPC.noGravity)
                {
                    Main.NewText("???");
                    NPC.velocity.Y = -8;
                    numBounces++;

                    Main.NewText(numBounces);
                }
                else
                {
                    Main.NewText("should bounce " + OvermorrowNPC.AICounter);
                    NPC.velocity.Y = -8;
                    //NPC.velocity.Y *= -1;
                    numBounces++;
                }
            }

            if (numBounces == 2 && NPC.collideY && OvermorrowNPC.AICounter > 70)
            {
                
                Main.NewText("do third again " + OvermorrowNPC.AICounter + " no gravity is: " + NPC.noGravity);
                if (NPC.noGravity)
                {
                    NPC.velocity.Y = 2;
                    numBounces++;
                } else
                {
                    NPC.velocity.Y = -3;
                    numBounces++;
                }
            }

            if (numBounces >= 3 && NPC.collideY)
            {
                Main.NewText("end bounces at " + numBounces);
                numBounces = 0;
                IsFinished = true;
                NPC.frameCounter = 0;
            }
        }
    }
}