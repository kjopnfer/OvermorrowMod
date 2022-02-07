using Terraria;

namespace OvermorrowMod.Quests
{
    public interface IQuestRequirement
    {
        bool CheckCompleted(Player player);
        string GetRequirementText();
    }
}