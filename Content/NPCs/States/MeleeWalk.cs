using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class MeleeWalk : BaseMovementState
    {
        public override int Weight => 1;
        public override bool CanExit => true;
        public MeleeWalk(OvermorrowNPC npc) : base(npc) { }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            float maxSpeed = 1.8f;
            NPC.direction = NPC.GetDirection(OvermorrowNPC.TargetingModule.Target);
            Vector2 distance = NPC.Move(OvermorrowNPC.TargetingModule.Target.Center, 0.2f, maxSpeed, 8f);
        }
    }
}