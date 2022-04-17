using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace OvermorrowMod.Content.NPCs
{
    class FunnySlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Funny Slime"); //IDK what this thing is called, I just saw it in #code-needed
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 38;
            NPC.height = 30;
            NPC.aiStyle = NPCID.BlueSlime;
            NPC.defense = 3;
            NPC.damage = 11; //Change as you see fit 
            NPC.lifeMax = 15;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 25f;
            AnimationType = NPCID.BlueSlime;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return SpawnCondition.OverworldDaySlime.Chance * 0.3f;
        }
    }
}