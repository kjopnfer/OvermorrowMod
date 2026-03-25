using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class MeleeWalk : BaseMovementState
    {
        public override int Weight => 1;
        public override bool CanExit => true;

        float maxSpeed = 1.8f;
        float moveSpeed = 0.1f;
        public MeleeWalk(OvermorrowNPC npc, float moveSpeed = 0.2f, float maxSpeed = 1.8f) : base(npc)
        {
            this.maxSpeed = maxSpeed;
            this.moveSpeed = moveSpeed;
        }

        public override void Enter()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                OvermorrowNPC.AICounter = 0;
        }

        public override void Exit()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                OvermorrowNPC.AICounter = 0;
        }

        public override void Update()
        {
            NPC.direction = NPC.GetDirectionFrom(OvermorrowNPC.TargetingModule.Target);
            Vector2 distance = NPC.Move(OvermorrowNPC.TargetingModule.Target.Center, moveSpeed, maxSpeed, 8f);

            if (OvermorrowNPC is ClockworkSpider)
            {
                OvermorrowNPC.AICounter++;
                if (OvermorrowNPC.AICounter > ModUtils.SecondsToTicks(1))
                {
                    distance = NPC.Move(OvermorrowNPC.TargetingModule.Target.Center, moveSpeed, maxSpeed, 8f);
                }
                else
                {
                    distance = NPC.Move(OvermorrowNPC.TargetingModule.Target.Center, moveSpeed, maxSpeed / 2, 8f);
                }
            }
        }
    }
}