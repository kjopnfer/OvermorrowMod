using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GrimoireHidden : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;
        public GrimoireHidden(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (OvermorrowNPC is not LivingGrimoire)
                return false;

            return true;
        }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
            IsFinished = false;
        }

        public override void Exit()
        {
            NPC.noGravity = true;
            OvermorrowNPC.AICounter = 0;

            OvermorrowNPC.AIStateMachine.RemoveSubstate<GrimoireHidden>(AIStateType.Idle, new GrimoireHidden(OvermorrowNPC));
            var newIdle = new GrimoireIdle(OvermorrowNPC);
            OvermorrowNPC.AIStateMachine.AddSubstate(AIStateType.Idle, newIdle);

            if (OvermorrowNPC is LivingGrimoire grimoire)
            {
                grimoire.ReenableAlertIndicator();
            }
        }

        public override void Update()
        {
            if (OvermorrowNPC.TargetingModule.HasTarget())
            {

                NPC.noGravity = false;
                if (OvermorrowNPC.AICounter++ >= 36)
                {
                    IsFinished = true;
                }
            }
        }
    }
}