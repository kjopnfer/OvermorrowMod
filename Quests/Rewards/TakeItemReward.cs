using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace OvermorrowMod.Quests.Rewards
{
    /// <summary>
    /// "Reward" for a quest which removes items from a player inventory.
    /// Would typically be used along with an ItemRequirement, to make a quest where
    /// the requested item is removed from the player inventory on completion.
    /// </summary>
    public class TakeItemReward : IQuestReward
    {
        private readonly int type;
        private readonly int stack;

        public TakeItemReward(int type, int stack)
        {
            if (stack <= 0) throw new ArgumentException($"Invalid stack size: {stack}");
            if (type <= 0) throw new ArgumentException($"Invalid type: {type}");
            this.stack = stack;
            this.type = type;
        }
        public string Description => $"Lose #{stack} {Lang.GetItemNameValue(type)}";

        public void Give(Player player)
        {
            int remaining = stack;
            foreach (var item in player.inventory)
            {
                if (item.type == type && item.stack > 0)
                {
                    int toTake = Math.Min(item.stack, remaining);
                    item.stack -= toTake;
                    if (item.stack == 0) item.TurnToAir();
                    remaining -= toTake;
                    if (remaining == 0) return;
                }
            }
        }
    }
}
