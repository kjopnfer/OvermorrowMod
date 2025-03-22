using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GainStealth : BaseAttackState
    {
        public override int Weight => 4;

        /// <summary>
        /// Prevent exiting until attack is done.
        /// </summary>
        public override bool CanExit => IsFinished;

        public override bool CanExecute(OvermorrowNPC npc)
        {
            if (!npc.NPC.IsStealthOnCooldown() && !npc.NPC.IsStealthed())
            {
                return true;
            }

            return false;
        }

        public override void Enter(OvermorrowNPC npc)
        {
            NPC baseNPC = npc.NPC;

            Main.NewText("Stealth gain begin");
            npc.AICounter = 0;
            npc.NPC.velocity.X = 0;

            var stealthDelay = 300;
            baseNPC.SetStealth(stealthTime: 18000, stealthDelay);

            IsFinished = false;
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("Stealth gain ends.");
            npc.AICounter = 0;
            npc.NPC.velocity.X = 0;
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.AICounter++ >= 60)
            {
                IsFinished = true;
            }
        }
    }
}