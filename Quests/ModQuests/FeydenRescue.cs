using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Content.WorldGeneration;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Requirements;
using OvermorrowMod.Quests.Rewards;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests.ModQuests
{
    public class FeydenRescue : BaseQuest
    {
        public override string QuestName => "A Call for Help";
        public override QuestRepeatability Repeatability => QuestRepeatability.OncePerWorld;
        public override QuestType Type => QuestType.Kill;
        public override void SetDefaults()
        {
            Requirements = new[]
            {
                new ChainRequirement(
                    new IQuestRequirement[]
                    {
                        new TravelRequirement("feyden_cave", () => ModUtils.FindNearestGround(GuideCamp.FeydenCavePosition) * 16, "???", "Investigate the Call for Help"),
                        new MiscRequirement("feyden_fight", "Help the Stranger Fight Off Slimes"),
                    }, "chain")
            };
            Rewards = new[]
            {
                new ItemReward(ItemID.DirtBlock, 1)
            };
        }

    }
}