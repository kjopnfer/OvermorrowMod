using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    /// <summary>
    /// NPC remains inactive until a target is found.
    /// </summary>
    public class Hidden : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => IsFinished;

        public override void Enter(OvermorrowNPC npc)
        {
        }

        public override void Exit(OvermorrowNPC npc)
        {
            npc.AICounter = 0;
        }

        public override void Update(OvermorrowNPC npc)
        {
            if (npc.TargetingModule.HasTarget())
            {
                IsFinished = true;
            }
        }
    }
}