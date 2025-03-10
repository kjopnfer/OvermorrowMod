using OvermorrowMod.Common;
using OvermorrowMod.Core.NPCs;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class GroundDashAttack : BaseAttackState
    {
        public override int Weight => 1;

        public override void Enter(OvermorrowNPC npc)
        {
            Main.NewText("enter");
        }

        public override void Exit(OvermorrowNPC npc)
        {
            Main.NewText("exit");
        }

        public override void Update(OvermorrowNPC npc)
        {
            Main.NewText("the attack state");
        }
    }
}