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
            NPC.width = 57;
            NPC.height = 64;
            NPC.aiStyle = -1;
            NPC.noGravity = false;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 100;
            NPC.knockBackResist = 0f;
            NPC.dontCountMe = true;
            NPC.chaseable = false;
        }
    }
}
