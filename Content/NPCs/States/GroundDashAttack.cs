using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GroundDashAttack : BaseAttackState
    {
        public override int Weight => 3;

        /// <summary>
        /// Prevent exiting until attack is done.
        /// </summary>
        public override bool CanExit => IsFinished;
        public GroundDashAttack(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            // Check if target is close enough to melee attack
            if (OvermorrowNPC.TargetingModule.Target is Entity target)
            {
                return Vector2.Distance(NPC.Center, target.Center) <= 10 * 16;
            }

            return false;
        }

        Vector2 toTarget;
        public override void Enter()
        {
            toTarget = NPC.Center.DirectionTo(OvermorrowNPC.TargetingModule.Target.Center);

            OvermorrowNPC.AICounter = 0;
            NPC.velocity.X = 0;

            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.velocity.X = 0;
            NPC.RemoveStealth();
        }


        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            if (OvermorrowNPC.AICounter == 30)
            {
                //NPC.velocity.X = Main.rand.Next(14, 17) * NPC.direction;

                float speed = Main.rand.Next(14, 17);
                if (MathHelper.ToDegrees(NPC.AngleTo(OvermorrowNPC.TargetingModule.Target.Center)) < 0)
                    speed = 12;

                NPC.velocity = toTarget * speed;
            }
            else if (OvermorrowNPC.AICounter >= 30)
            {
                if (NPC.collideX)
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                if (NPC.velocity.X != 0 && OvermorrowNPC.AICounter >= 40)
                {
                    NPC.velocity.X *= 0.9f;
                }

                NPC.velocity.Y += 0.09f;

                if (NPC.collideX)
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                if (OvermorrowNPC.AICounter++ >= 102 && NPC.collideY)
                //if(Math.Abs(NPC.velocity.X) <= 2)
                {
                    IsFinished = true;
                }
            }
        }
    }
}