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

        public override bool CanExecute(OvermorrowNPC npc)
        {
            // Check if target is close enough to melee attack
            if (npc.TargetingModule.Target is Entity target)
            {
                return Vector2.Distance(npc.NPC.Center, target.Center) <= 10 * 16;
            }

            return false;
        }

        public override void Enter()
        {
            Main.NewText("Dash attack begin");
            OvermorrowNPC.AICounter = 0;
            NPC.velocity.X = 0;


            IsFinished = false;
        }

        public override void Exit()
        {
            Main.NewText("Dash attack ends.");
            NPC.velocity.X = 0;
            NPC.RemoveStealth();
        }

        public override void Update()
        {
            OvermorrowNPC.AICounter++;

            if (OvermorrowNPC.AICounter == 30)
            {
                NPC.velocity.X = Main.rand.Next(14, 17) * NPC.direction;
            }
            else if (OvermorrowNPC.AICounter >= 30)
            {
                if (NPC.collideX)
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                if (NPC.velocity.X != 0 && OvermorrowNPC.AICounter >= 40)
                {
                    NPC.velocity.X *= 0.9f;
                }

                if (NPC.collideX)
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                if (OvermorrowNPC.AICounter++ >= 102)
                //if(Math.Abs(NPC.velocity.X) <= 2)
                {
                    IsFinished = true;
                }
            }
        }
    }
}