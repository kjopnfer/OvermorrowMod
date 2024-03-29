using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.Items.Accessories;
using OvermorrowMod.Content.Items.Quest;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
                        new ItemRequirement("gel", ItemID.Gel, 1, "Obtain 1 Gel", false),
                        new ItemRequirement("torches", ItemID.Torch, 3, "Obtain 3 Torches", false),
                        new MiscRequirement("campfire", "Relight the Campfire"),
                    }, "chain")
            };
            Rewards = new[]
            {
                new ChooseReward(
                    "monster_reward",
                    new ItemReward[]
                    {
                        new ItemReward(ModContent.ItemType<OldWhetstone>()),
                        new ItemReward(ModContent.ItemType<WarmAmulet>()),
                    }
                ),
                new ChooseReward(
                    "biome_reward",
                    new ItemReward[]
                    {
                        new ItemReward(ModContent.ItemType<SimpleScabbard>()),
                        new ItemReward(ModContent.ItemType<WarmAmulet>()),
                    }
                ),
                new ChooseReward(
                    "build_reward",
                    new ItemReward[]
                    {
                        new ItemReward(ItemID.Swordfish, 1),
                        new ItemReward(ModContent.ItemType<WarmAmulet>()),
                    }
                ),
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