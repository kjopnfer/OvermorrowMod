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

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("enter walk");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("exited walk");
        }

        public override void Update(OvermorrowNPC npc)
        {
            float maxSpeed = 1.8f;
            npc.NPC.direction = npc.NPC.GetDirection(npc.TargetingModule.Target);
            Vector2 distance = npc.NPC.Move(npc.TargetingModule.Target.Center, 0.2f, maxSpeed, 8f);

            Main.NewText("the walk state");
        }
    }
}