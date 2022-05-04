using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideSlime : BaseQuest
    {
        public override string QuestName => "Slime";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorldPerPlayer;
        public override QuestType Type => QuestType.Kill;
        public override int QuestGiver => NPCID.Guide;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new OrRequirement(
                    "slime_or",
                    new KillRequirement(new List<int>(){
                        NPCID.BlueSlime,
                        NPCID.Bunny,
                        NPCID.BirdRed
                    }, 3, "slime_kill_1"),
                    new KillRequirement(new List<int>(){
                        NPCID.Grasshopper,               
                    }, 3, "slime_kill_2")
                )
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.IronPickaxe, 1),
            };
            QuestDialogue.Add("Hey you, you're finally awake.");
            QuestDialogue.Add("You were trying to cross the border, right? Walked right into that Slime ambush, same as us.");
            QuestDialogue.Add("Damn you Slimes, Terraria was fine until they came along.");
            QuestDialogue.Add("How about we get some revenge and knock out some of them for the trouble they gave us?");
            QuestDialogue.Add("Killing three of them should teach them a lesson, come back to me once you're done.");

            QuestHint.Add("Slimes are common around these parts, keep moving around and you'll be sure to find some.");

            QuestEndDialogue.Add("Wow, nice work.");
        }

        public override bool IsValidFor(Player player)
        {
            return Quests.HasCompletedQuest<GuideTravel>(player);
        }
    }
}
