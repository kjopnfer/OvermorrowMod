﻿/*using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideTutorial2 : BaseQuest
    {
        public override string QuestName => "Tutorial 2";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorldPerPlayer;
        public override int QuestGiver => NPCID.Guide;
        public override int QuestDelay => 1200;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new OrRequirement(
                    "ore_or",
                    new ItemRequirement("copperOre", ItemID.CopperOre, 40, "Obtain 40 Copper Ore", false),
                    new ItemRequirement("tinOre", ItemID.TinOre, 40, "Obtain 40 Tin Ore", false))
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.IronPickaxe, 1),
                new ItemReward(ItemID.Torch, 40)
            };
            QuestDialogue.Add("Next you're going to need some gear. Why don't you head down into the underground and dig up some ore?");
            QuestDialogue.Add("If you manage to get some, I'll give you my old pickaxe.");

            QuestHint.Add("Dig up some copper or tin ore for me, if you would. You can find it just beneath the surface.");

            QuestEndDialogue.Add("Excellent! Here is your reward!");
        }

        protected override bool IsValidFor(Player player)
        {
            return Quests.HasCompletedQuest<GuideTutorial>(player);
        }
    }
}*/
