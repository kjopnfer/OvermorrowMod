using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideCampfire : BaseQuest
    {
        public override string QuestName => "Rekindle the Flame";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorldPerPlayer;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => NPCID.Guide;
        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ChainRequirement(
                    new IQuestRequirement[]
                    {
                        new ItemRequirement("axe", ItemID.CopperAxe, 1, "Retrieve the Axe from the Stump", false),
                        new ItemRequirement("wood", ItemID.Wood, 10, "Obtain 10 Wood", false),
                        new ItemRequirement("gel", ItemID.Gel, 2, "Obtain 2 Gel", false),
                        new ItemRequirement("torches", ItemID.Torch, 3, "Obtain 3 Torches", false),
                        new MiscRequirement("campfire", "Relight the Campfire"),
                    }, "chain")
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };

            QuestDialogue.Add("Give me 20 wood, then go on a trip, and finally come back and giving me a gold coin.");
            QuestHint.Add("You got me that wood and gold yet?");
            QuestEndDialogue.Add("Thanks, sucker");
        }

        protected override bool IsValidFor(Player player)
        {
            return player.GetModPlayer<DialoguePlayer>().unlockedGuideCampfire;
        }
    }
}