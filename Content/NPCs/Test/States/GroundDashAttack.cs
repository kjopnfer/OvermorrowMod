using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
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

        public override bool CanExecute(OvermorrowNPC npc)
        {
            // Check if target is close enough to melee attack
            if (npc.TargetingModule.Target is Entity target)
            {
                return Vector2.Distance(npc.NPC.Center, target.Center) <= 100f;
            }

            return false;
        }

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("Melee attack begin");
            npc.AICounter = 0;
            npc.NPC.velocity.X = 0;

            IsFinished = false;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("Melee attack ends.");
        }

        public override void Update(OvermorrowNPC npc)
        {
            npc.AICounter++;

            if (npc.AICounter >= 60) // Done attacking after 60 ticks
            {
                IsFinished = true;
            }
        }
    }
}