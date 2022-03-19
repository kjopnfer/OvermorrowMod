using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class SalamanderHunter : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Salamander Hunter");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 34;
            npc.height = 53;
            npc.damage = 14;
            npc.defense = 6;
            npc.lifeMax = 120;
            npc.HitSound = SoundID.NPCHit50;
            npc.DeathSound = SoundID.NPCDeath53;
            npc.value = 60f;
            npc.knockBackResist = 0.5f;
            npc.aiStyle = 3;
            aiType = NPCID.GoblinScout;
        }

        public override void AI()
        {
            if (npc.wet)
            {
                npc.velocity.Y -= .65f;
            }

            if (npc.collideX && npc.velocity.Y == 0)
            {
                npc.velocity.Y -= 6f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.spriteDirection = -npc.direction;
            npc.frameCounter++;

            if (npc.frameCounter % 6 == 5f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 4) // 4 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.GetModPlayer<OvermorrowModPlayer>().ZoneWaterCave && !spawnInfo.water ? 0.08f : 0f;
        }
    }
}