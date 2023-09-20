using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.State;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Quests.Rewards
{
    public class ChooseReward : IQuestReward
    {
        public string Description => throw new System.NotImplementedException();
        protected IQuestReward[] Choices { get; }
        public string ID { get; }

        public ChooseReward(string id, IQuestReward[] choices)
        {
            ID = id;
            Choices = choices;
        }

        public void Give(Player player)
        {
            foreach (var reward in Choices)
            {
                reward.Give(player);
            }
        }
    }
}