using Microsoft.Xna.Framework;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideTravel : BaseQuest
    {
        public override string QuestName => "Travel";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorld;
        public override QuestType Type => QuestType.Travel;
        public override int QuestDelay => 2400;
        public override int QuestGiver => NPCID.Guide;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new TravelRequirement(new Vector2(Main.spawnTileX, Main.spawnTileY) * 16)
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.IronPickaxe, 1),
            };
            QuestDialogue.Add("We are going to Brazil.");
            QuestDialogue.Add("Find the marker.");

            QuestHint.Add("Look at the map if you don't know where to go.");

            QuestEndDialogue.Add("You found it.");
        }
    }
}
