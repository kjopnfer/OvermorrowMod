using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.ModQuests
{
    public class GuideHouse : BaseQuest
    {
        public override string QuestName => "Homeless";
        // So that this can't be abused, make it only possible to complete once per world. If multiple players
        // "compete", then only the first one to finish gets the reward. This is an OK way to handle this kind of
        // quest. It can also be used to make quests which have global consequences, i.e. an reward that
        // it would be really bad if triggered twice.
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorld;
        public override int QuestGiver => NPCID.Guide;

        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new HousingRequirement(NPCID.Guide)
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.GoldCoin, 1),
                new ItemReward(ItemID.Torch, 60)
            };
            QuestDialogue.Add("Well if you're looking for something else to do, " +
                "you could try your hand at building. With all the wood you've gathered, " +
                "I'm sure you have more than enough to make a comfortable house.");
            QuestDialogue.Add("You'll need some wall, a table and chair, and some light. " +
                "Thankfully, you can use that workbench you made as a table");
            QuestDialogue.Add("Once you're done, I'll move in, and if you make more houses, some new faces might just show up!");

            QuestHint.Add("Keep on working on building a house. Remember a house needs walls, a table and chair, and some light.");

            QuestEndDialogue.Add("Nice job! The house looks very comfortable.");
            QuestEndDialogue.Add("Well I think you're pretty ready now. I'd suggest you start exploring the world and the underground. " +
                "There's a lot to find which will help you with your journey");
        }

        protected override bool IsValidFor(Player player)
        {
            // This is how we do quest dependencies, you could have multiple statements like this.
            if (!Quests.HasCompletedQuest<GuideTutorial>(player)) return false;
            // This would mean that in theory the quest could be available if no NPC was present,
            // so this could be used as an "Invite NPC" quest as well.
            var npc = Main.npc.FirstOrDefault(n => n.active && n.type == NPCID.Guide);
            return npc == null || npc.homeless;
        }
    }
}
