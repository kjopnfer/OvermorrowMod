using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class TestQuest : Quest
    {
        public override List<string> QuestDialogue => new List<string> {"get me 10 wood underling", "you are building a crafting bench", "noob lol"};
        public override (int, int)[] QuestRewards => new[]
        {
            ((int)ItemID.Torch, 60),
            ((int)ItemID.Wood, 60),
        };
        public override string QuestName() => "Make Crafting Bench";
        public override int QuestGiver() => NPCID.Guide;
        public override int QuestID() => (int)ID.TutorialGuideQuest;
    }
}