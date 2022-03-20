namespace OvermorrowMod.Common.NPCs
{
    public class PushBlock : PushableNPC
    {
        bool RunOnce = true;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("A");
        }

        public override void SetDefaults()
        {
            npc.width = 57;
            npc.height = 64;
            npc.aiStyle = -1;
            npc.noGravity = false;
            npc.dontTakeDamage = true;
            npc.lifeMax = 100;
            npc.knockBackResist = 0f;
            npc.dontCountMe = true;
            npc.chaseable = false;
        }
    }
}
