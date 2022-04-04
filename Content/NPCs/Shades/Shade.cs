using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Shades
{
    public class Shade : ModNPC
    {
        public override bool CheckActive() => false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("???");
        }

        public override void SetDefaults()
        {
            npc.CloneDefaults(NPCID.Wraith);
            npc.width = 32;
            npc.height = 32;
            npc.defense = 9;
            npc.lifeMax = 200;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;

            //aiType = NPCID.Wraith;
        }

        public ref float AICounter => ref npc.ai[0];
    }
}