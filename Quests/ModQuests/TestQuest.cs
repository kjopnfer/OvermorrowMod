using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class TestQuest : Quest
    {
        public override List<string> QuestDialogue => new List<string> {
            "So you want to know how to get started? Well, you’ve got some basic tools on you so I'd recommend chopping down some trees. " +
            "Wood can be used for all sorts of stuff such as a workbench, torches, and more",
            "However, torches require gel which you can get from slimes if you think you’re up to the task",
            "If you get me a few torches and make a workbench, I’ll give you something I found in my travels as a reward."
        };

        public override List<string> HintDialogue => new List<string> {
            "Let me know when you get those torches and a workbench."
        };

        public override string QuestName() => "Make Crafting Bench";
        public override int QuestGiver() => NPCID.Guide;

        public override void SetDefaults()
        {
            AddRequirement(new ItemRequirement(ItemID.WorkBench, 1));
            AddRequirement(new ItemRequirement(ItemID.Torch, 20));
            AddReward(ItemID.Wood, 100);
        }
    }

    /*public class TestQuest : Quest
    {
        public override List<Requirement> QuestRequirements => new List<Requirement>() {
            new Requirement(ItemID.WorkBench, 1, false)
        };

        public override List<string> QuestDialogue => new List<string> {
            "So you want to know how to get started? Well, you’ve got some basic tools on you so I'd recommend chopping down some trees. " +
            "Wood can be used for all sorts of stuff such as a workbench, torches, and more",
            "However, torches require gel which you can get from slimes if you think you’re up to the task",
            "If you get me a few torches and make a workbench, I’ll give you something I found in my travels as a reward."
        };

        public override List<string> HintDialogue => new List<string> {
            "Let me know when you get those torches and a workbench."
        };

        public override string QuestName() => "Make Crafting Bench";
        public override int QuestGiver() => NPCID.Guide;
        public override int QuestID() => (int)ID.TutorialGuideQuest;

        /*public override List<Requirement>[] QuestRequirements()
        {
            return new[]
            {
                new Requirement(ItemID.WorkBench, 1, false)
                /*((int)ItemID.WorkBench, 1),
                (ItemID.Torch, 12)
            };
        }*/
    /*public override List<Requirement> QuestRequirements()
    {
        return new List<Requirement>() 
        {
           new Requirement(ItemID.WorkBench, 1, false)
        };
    }


    public override (int, int)[] QuestRewards()
    {
        return new[]
        {
            ((int)ItemID.Torch, 60)
        };
    }
}*/
}