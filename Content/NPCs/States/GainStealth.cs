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

        public override bool CanExecute()
        {
            if (!NPC.IsStealthOnCooldown() && !NPC.IsStealthed())
            {
                return true;
            }

            return false;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            NPC.velocity.X = 0;

            var stealthDelay = ModUtils.SecondsToTicks(5);
            NPC.SetStealth(stealthTime: ModUtils.SecondsToTicks(300), stealthDelay);

            IsFinished = false;
        }

        public override void Exit()
        {
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