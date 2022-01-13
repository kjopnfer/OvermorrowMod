using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class TestQuest : Quest
    {
        public override List<string> QuestDialogue => new List<string> {"dialogue 1", "dialogue2", "dialogue3"};
        public override string QuestName() => "Chungus";
        public override int QuestGiver() => NPCID.Guide;
    }
}