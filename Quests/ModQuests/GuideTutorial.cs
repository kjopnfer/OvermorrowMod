using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideTutorial : BaseQuest
    {
        public override string QuestName => "Tutorial";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerPlayer;
        public override int QuestGiver => NPCID.Guide;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement(ItemID.WorkBench, 1),
                new ItemRequirement(ItemID.Torch, 20)
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.Wood, 60),
                new ItemReward(ItemID.Torch, 60)
            };
            QuestDialogue.Add("So you want to know how to get started? " +
                "Well, you've got some basic tools on you so I'd recommend chopping down some trees. " +
                "Wood can be used for all sorts of stuff such as a workbench, torches, and more");
            QuestDialogue.Add("However, torches require gel which you can get from slimes if you think you're up to the task");
            QuestDialogue.Add("If you get me a few torches and make a workbench, I'll give you something I found in my travels as a reward");

            QuestHint.Add("Let me know when you get those torches and a workbench.");

            QuestEndDialogue.Add("Excellent! Here is your reward!");
        }
    }
}
