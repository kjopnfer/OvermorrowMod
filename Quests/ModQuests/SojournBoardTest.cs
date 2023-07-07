using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Content.Items.Weapons.Ranged;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests.ModQuests
{
    public class SojournBoardTest : JobBoardQuest
    {
        public override string QuestName => "Get Gel";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

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

    public class SojournBoardTest2 : JobBoardQuest
    {
        public override string QuestName => "Touch Dirt";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

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

    public class SojournBoardTest3 : JobBoardQuest
    {
        public override string QuestName => "Where is Grass";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.DirtBlock, 8, "Obtain 8 Grass Seeds", true)
            };

            Rewards = new[]
            {
                new ItemReward(ModContent.ItemType<WildEye>(), 1),
                new ItemReward(ModContent.ItemType<GraniteLauncher>(), 1)
            };
        }
    }

    public class SojournBoardTest4 : JobBoardQuest
    {
        public override string QuestName => "Free Money";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.Torch, 2, "Obtain 1 Torch", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.GoldBar, 2),
                new ItemReward(ItemID.GoldCoin, 2),
            };
        }
    }

    public class SojournBoardTest5 : JobBoardQuest
    {
        public override string QuestName => "Cowboy Time";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.GoldCoin, 2, "Obtain 2 Gold Coins", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.CowboyHat, 1),
                new ItemReward(ItemID.CowboyJacket, 1),
                new ItemReward(ItemID.CowboyPants, 1),
            };
        }
    }

}