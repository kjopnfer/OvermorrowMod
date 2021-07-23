using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.Goblin
{
    public class WarningBossG : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Right Click on me!");
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
            npc.knockBackResist = 0f;
            npc.CloneDefaults(NPCID.Guide);
            npc.friendly = true;
            npc.aiStyle = 7;
            npc.townNPC = true;  
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                StartBoss();
            }
        }



        public override string GetChat()
        {
            return "Click 'shop' to fight the mini boss";
        }



        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        void StartBoss()
        {
            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("GoblinMiniBoss"));
            return;
        }

        public override void AI()
        {
            npc.noGravity = true;
            npc.velocity.Y = 0f;
            npc.velocity.X = 0f;
        }
    }
}