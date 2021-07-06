using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs
{
    public class BloodHeal : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Clot");
            Main.npcFrameCount[npc.type] = 9;
        }

        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 32;
            npc.aiStyle = 0;
            npc.noGravity = false;
            npc.damage = 0;
            npc.defense = 400;
            npc.lifeMax = 7;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[BuffID.OnFire] = true;
        }
        
        public override void AI()
        {
            experttimer++;
            if (expert && experttimer == 1)
            {
                npc.life = 7;
                npc.lifeMax = 7;
                npc.damage = 0;
            }
            if (experttimer > 240)
            {
                npc.life = -7;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;

            if (npc.frameCounter % 6f == 5f) // Ticks per frame
            {
                npc.frame.Y += frameHeight;
            }
            if (npc.frame.Y >= frameHeight * 7) // 7 is max # of frames
            {
                npc.frame.Y = 0; // Reset back to default
            }
        }

        public override void NPCLoot()
        {
            if(experttimer < 120)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 58);
            }
            if (experttimer < 180)
            {
                Item.NewItem((int)npc.position.X + 7, (int)npc.position.Y, npc.width, npc.height, 58);
            }
            if (experttimer < 240)
            {
                Item.NewItem((int)npc.position.X - 7, (int)npc.position.Y, npc.width, npc.height, 58);
            }
        }
    }
}

