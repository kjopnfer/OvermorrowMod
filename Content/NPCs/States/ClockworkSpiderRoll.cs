using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class ClockworkSpiderRoll : BaseAttackState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        public ClockworkSpiderRoll(OvermorrowNPC npc) : base(npc) { }

        public override bool CanExecute()
        {
            if (OvermorrowNPC is not ClockworkSpider)
                return false;

           
            return NPC.collideY;
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }
    }
}