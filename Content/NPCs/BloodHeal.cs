using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs
{
    public class BloodHeal : ModNPC
    {
        readonly bool expert = Main.expertMode;
        private int experttimer = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Clot");
            Main.npcFrameCount[NPC.type] = 9;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 32;
            NPC.aiStyle = 0;
            NPC.noGravity = false;
            NPC.damage = 0;
            NPC.defense = 400;
            NPC.lifeMax = 7;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.buffImmune[BuffID.OnFire] = true;
        }

        public override void AI()
        {
            experttimer++;
            if (expert && experttimer == 1)
            {
                NPC.life = 7;
                NPC.lifeMax = 7;
                NPC.damage = 0;
            }
            if (experttimer > 240)
            {
                NPC.life = -7;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter % 6f == 5f) // Ticks per frame
            {
                NPC.frame.Y += frameHeight;
            }
            if (NPC.frame.Y >= frameHeight * 7) // 7 is max # of frames
            {
                NPC.frame.Y = 0; // Reset back to default
            }
        }

        public override void OnKill()
        {
            if (experttimer < 120)
            {
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, 58);
            }
            if (experttimer < 180)
            {
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X + 7, (int)NPC.position.Y, NPC.width, NPC.height, 58);
            }
            if (experttimer < 240)
            {
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X - 7, (int)NPC.position.Y, NPC.width, NPC.height, 58);
            }
        }
    }
}

