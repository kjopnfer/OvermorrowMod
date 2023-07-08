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

    public class SojournBoardTest6 : JobBoardQuest
    {
        public override string QuestName => "Birdemic";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.GoldCoin, 2, "Kill 2 Strykebeaks", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.BabyBirdStaff, 1),
            };
        }
    }

    public class SojournBoardTest7 : JobBoardQuest
    {
        public override string QuestName => "I'm at Soup";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.Mushroom, 2, "Obtain 9 Mushrooms", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.BowlofSoup, 10),
            };
        }
    }

    public class SojournBoardTest8 : JobBoardQuest
    {
        public override string QuestName => "Heavenly Cruelty";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.Mushroom, 2, "Kill 30 Bunnies", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.Starfury, 1),
            };
        }
    }

    public class SojournBoardTest9 : JobBoardQuest
    {
        public override string QuestName => "Whose Name";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.Meteorite, 12, "Obtain 12 Meteorite", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.SpaceGun, 1),
                new ItemReward(ItemID.BluePhasesaber, 1),
            };
        }
    }

    public class SojournBoardTest10 : JobBoardQuest
    {
        public override string QuestName => "Rats of the Sky";
        public override QuestRepeatability Repeatability => QuestRepeatability.Repeatable;
        public override QuestType Type => QuestType.Fetch;
        public override int QuestGiver => (int)JobBoardID.Sojourn;
        public override int BoardID => (int)JobBoardID.Sojourn;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ItemRequirement("grass", ItemID.Meteorite, 5, "Kill 5 Harpies", true)
            };

            Rewards = new[]
            {
                new ItemReward(ItemID.RoastedBird, 20),
            };
        }
    }

}