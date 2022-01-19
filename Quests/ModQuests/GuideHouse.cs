using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideHouse : Quest
    {
        public override List<string> QuestDialogue => new List<string> {
            "Well if you're looking for something else to do, you could try your hand at building. With all the wood you've gathered, I'm sure you have more than enough to make a comfortable house.",
            "You'll need some wall, a table and chair, and some light. Thankfully, you can use that workbench you made as a table",
            "Once you're done, I'll move in, and if you make more houses, some new faces might just show up!"};
        public override List<string> HintDialogue => new List<string> {
            "Keep on working on building a house. Remember a house needs walls, a table and chair, and some light"
        };

        public override (int, int)[] QuestRewards => new[]
        {
            ((int)ItemID.Torch, 60),
            ((int)ItemID.GoldCoin, 60),
        };

        public override string QuestName() => "Make a House";
        public override int QuestGiver() => NPCID.Guide;
        public override int QuestID() => (int)ID.GuideHouseQuest;

        public override void SetDefaults()
        {
            AddRequirement(new ItemRequirement(ItemID.WorkBench, 1));
            AddRequirement(new ItemRequirement(ItemID.Torch, 20));
            //AddReward(ItemID.Wood, 100);
        }
    }
}