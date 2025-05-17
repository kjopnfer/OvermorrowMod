using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System.Diagnostics.Metrics;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class InactiveIdle : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => true;
        public InactiveIdle(OvermorrowNPC npc) : base(npc) { }

        public override void Enter()
        {
            OvermorrowNPC.AICounter = 0;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
        }

        public override void Update()
        {
            IsFinished = true;
        }
    }
}