using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class TestQuest : Quest
    {
        public override List<string> QuestDialogue => new List<string> {"get me 10 wood underling", "you are building a crafting bench", "noob lol"};
        public override List<string> HintDialogue => new List<string> {
            "Let me know when you get those torches and a workbench."
        };

        /*public override (int, int)[] QuestRewards => new[]
        {
            ((int)ItemID.Torch, 60),
            ((int)ItemID.Wood, 60),
        };*/
        public override string QuestName() => "Make Crafting Bench";
        public override int QuestGiver() => NPCID.Guide;
        public override int QuestID() => (int)ID.TutorialGuideQuest;

        public override void SetDefaults()
        {
            AddRequirement(new ItemRequirement(ItemID.WorkBench, 1));
            AddRequirement(new ItemRequirement(ItemID.Torch, 20));
            AddReward(ItemID.Wood, 100);
        }
    }
}