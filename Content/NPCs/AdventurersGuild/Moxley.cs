using OvermorrowMod.Common;
using OvermorrowMod.Core.MetaUpgrades;
using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.AdventurersGuild
{
    public class Moxley : ModNPC
    {
        public override string Texture => AssetDirectory.GuildNPCs + Name;

        private bool healed;

        private static string Line(string key) => Language.GetTextValue(LocalizationPath.Dialogue + "Moxley." + key);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 36;
            NPC.height = 50;
            NPC.friendly = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.lifeMax = 250;
            NPC.aiStyle = -1;
            NPC.npcSlots = 0f;
        }

        public override void AI()
        {
            NPC.velocity.X = 0f;
        }

        public override bool CanChat() => true;

        public override string GetChat() => Line(healed ? "AlreadyHealed" : "Greeting");

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Line("HealButton");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (!firstButton) return;

            if (healed)
            {
                Main.npcChatText = Line("AlreadyHealed");
                return;
            }

            Heal(Main.LocalPlayer);
            healed = true;
            Main.npcChatText = Line("Healed");
        }

        private static void Heal(Player player)
        {
            int missing = player.statLifeMax2 - player.statLife;
            if (missing > 0) player.Heal(missing);
            player.statMana = player.statManaMax2;

            for (int i = Player.MaxBuffs - 1; i >= 0; i--)
            {
                int type = player.buffType[i];
                if (player.buffTime[i] > 0 && Main.debuff[type] && !BuffID.Sets.NurseCannotRemoveDebuff[type])
                    player.DelBuff(i);
            }

            if (player.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.HeartyMeal) > 0)
                player.AddBuff(BuffID.WellFed, SubworldTimer.Duration);

            SoundEngine.PlaySound(SoundID.Item4, player.Center);
        }
    }
}
