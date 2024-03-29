﻿using OvermorrowMod.Core.Interfaces;
using System;
using Terraria;

namespace OvermorrowMod.Quests.Rewards
{
    public class ItemReward : IQuestReward
    {
        private readonly int type;
        private readonly int stack;

        public ItemReward(int type, int stack = 1)
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
