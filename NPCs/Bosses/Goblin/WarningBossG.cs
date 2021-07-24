using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;

namespace OvermorrowMod.NPCs.Bosses.Goblin
{
    [AutoloadHead]
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
            return "Click 'Fight' to battle the miniboss. During this battle you can only shoot on the X axis. It is recommended that you use climbing claws.";
        }


        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "Fight";
        }

        void StartBoss()
        {
            Player player = Main.player[npc.target];
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/NPC/BelowZeroOpening"), player.Center);
            NPC.NewNPC((int)npc.Center.X - 10 * 16, (int)npc.Center.Y, mod.NPCType("GoblinMiniBoss"));
            return;
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            damage *= 0;
            return false;
        }

        public override void AI()
        {
            npc.homeless = false;
            npc.noGravity = true;
            npc.velocity.Y = 0f;
            npc.velocity.X = 0f;
        }
    }
}