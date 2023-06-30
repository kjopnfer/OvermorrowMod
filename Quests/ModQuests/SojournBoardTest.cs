using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests.ModQuests
{
    public class SojournBoardTest : BaseQuest
    {
        public override string QuestName => "Get Gel";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        //public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("gel", ItemID.Gel, 2, "Obtain 2 Gel", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };
        }
    }

    public class SojournBoardTest2 : BaseQuest
    {
        public override string QuestName => "Touch Dirt";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        //public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.DirtBlock, 2, "Obtain 2 Dirt", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };
        }
    }
}