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
        public GainStealth(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute(OvermorrowNPC npc)
        {
            if (!npc.NPC.IsStealthOnCooldown() && !npc.NPC.IsStealthed())
            {
                return true;
            }

            return false;
        }

        public override void Enter()
        {
            Main.NewText("Stealth gain begin");
            OvermorrowNPC.AICounter = 0;
            NPC.velocity.X = 0;

            var stealthDelay = 300;
            NPC.SetStealth(stealthTime: 18000, stealthDelay);

            IsFinished = false;
        }

        public override void Exit()
        {
            Main.NewText("Stealth gain ends.");
            OvermorrowNPC.AICounter = 0;
            NPC.velocity.X = 0;
        }

        public override void Update()
        {
            if (OvermorrowNPC.AICounter++ >= 60)
            {
                IsFinished = true;
            }
        }
    }
}