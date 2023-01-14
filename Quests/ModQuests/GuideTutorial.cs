using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideTutorial : BaseQuest
    {
        public override string QuestName => "Tutorial";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorldPerPlayer;
        public override int QuestGiver => NPCID.Guide;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("workbench", ItemID.WorkBench, 1, true),
                new ItemRequirement("torch", ItemID.Torch, 20, true)
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

        protected override bool IsValidFor(Player player)
        {
            return Quests.HasCompletedQuest<GuideSlime>(player);
        }
    }
}
