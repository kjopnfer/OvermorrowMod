using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Shades
{
    public class Shade : ModNPC
    {
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            NPC.CloneDefaults(NPCID.Wraith);
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 9;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath53;
            NPC.value = 60f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            //aiType = NPCID.Wraith;
        }

        public ref float AICounter => ref NPC.ai[0];
    }
}