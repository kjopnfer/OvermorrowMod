using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    public interface IQuestReward
    {
        void Give(Player player);
        string Description { get; }
    }
}
