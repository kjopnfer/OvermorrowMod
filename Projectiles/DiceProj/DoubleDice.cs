using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.DiceProj
{
    public class DoubleDice : ModNPC
    {
        public override void SetDefaults()
        {
            npc.width = 30;
            npc.height = 56;
            npc.aiStyle = 8;
            aiType = NPCID.GoblinSorcerer;
            animationType = NPCID.GoblinSorcerer;
            npc.damage = 40;
            npc.defense = 0;
            npc.lifeMax = 120;
            npc.buffImmune[BuffID.Ichor] = true;
            npc.buffImmune[BuffID.CursedInferno] = true;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("2 Dice");
            Main.npcFrameCount[npc.type] = 3;
        }
        public override void NPCLoot()
        {
            if (Main.rand.Next(0, 25) == 1)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StaffDoDoDoRed"));
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = npc.lifeMax * 1;
            npc.damage = npc.damage * 1;
        }
    }
}

