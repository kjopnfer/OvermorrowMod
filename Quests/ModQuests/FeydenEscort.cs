using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests.ModQuests
{
    public class FeydenEscort : BaseQuest
    {
        public override string QuestName => "Journey to Sojourn";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorld;
        public override QuestType Type => QuestType.Travel;
        public override int QuestGiver => ModContent.NPCType<Feyden>();
        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new TravelRequirement("sojourn_travel", () => TownGeneration.SojournLocation, "Sojourn", "Escort Feyden to Sojourn"),
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };
        }

        protected override bool IsValidFor(Player player)
        {
            return OvermorrowWorld.savedFeyden;
        }
    }
}