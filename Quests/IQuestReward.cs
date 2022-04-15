using Terraria;

namespace OvermorrowMod.Quests
{
    public interface IQuestReward
    {
        void Give(Player player);
        string Description { get; }
    }
}
