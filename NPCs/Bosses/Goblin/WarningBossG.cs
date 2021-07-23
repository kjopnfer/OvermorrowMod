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
            npc.width = 36;
            npc.height = 36;
            npc.damage = 0;
            npc.defense = 0;
            npc.lifeMax = 120;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCHit4;
            npc.value = 0f;
            npc.knockBackResist = 0f;
            npc.friendly = true;
            npc.townNPC = true;
            npc.dontTakeDamage = true;
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
            return "Click 'Fight' to battle the miniboss";
        }


        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Fight";
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