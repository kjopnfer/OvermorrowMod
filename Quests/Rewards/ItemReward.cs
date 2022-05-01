using System;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Quests.Rewards
{
    public class ItemReward : IQuestReward
    {
        private readonly int type;
        private readonly int stack;

        public ItemReward(int type, int stack)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
        }
        public string Description => $"#{stack} {Lang.GetItemNameValue(type)}";

        public void Give(Player player)
        {
            player.QuickSpawnItem(player.GetSource_GiftOrReward("overmorrowquest"), type, stack);
        }
    }
}
