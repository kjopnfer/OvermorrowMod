using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.ModQuests;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class FeydenManager : DialogueManager
    {
        public override int NPC => ModContent.NPCType<Feyden>();

        public override DialogueWindow GetDialogueWindow()
        {
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            if (questPlayer.IsDoingQuest<Quests.ModQuests.FeydenEscort>())
            {
                return new FeydenEscort();
            }

            return new FeydenFree();
        }
    }
}