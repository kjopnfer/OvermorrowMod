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

        public override void Enter(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;

            Main.NewText("Dash attack begin");
            npc.AICounter = 0;
            npc.NPC.velocity.X = 0;


            IsFinished = false;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("Dash attack ends.");
            npc.NPC.velocity.X = 0;
            npc.NPC.RemoveStealth();
        }

        public override void Update(OvermorrowNPC npc)
        {
            npc.AICounter++;
            NPC baseNPC = npc.NPC;

            if (npc.AICounter == 30)
            {
                baseNPC.velocity.X = Main.rand.Next(14, 17) * baseNPC.direction;
            }
            else if (npc.AICounter >= 30)
            {
                if (baseNPC.collideX)
                    Collision.StepUp(ref baseNPC.position, ref baseNPC.velocity, baseNPC.width, baseNPC.height, ref baseNPC.stepSpeed, ref baseNPC.gfxOffY);

                if (baseNPC.velocity.X != 0 && npc.AICounter >= 40)
                {
                    baseNPC.velocity.X *= 0.9f;
                }

                if (baseNPC.collideX)
                    Collision.StepUp(ref baseNPC.position, ref baseNPC.velocity, baseNPC.width, baseNPC.height, ref baseNPC.stepSpeed, ref baseNPC.gfxOffY);

                if (npc.AICounter++ >= 102)
                //if(Math.Abs(baseNPC.velocity.X) <= 2)
                {
                    IsFinished = true;
                }
            }
        }
    }
}