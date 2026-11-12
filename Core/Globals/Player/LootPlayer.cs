using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    /// <summary>
    /// Per-player run state for the loot system: tracks recently offered items per pool
    /// for the bag-exclusion window.
    /// </summary>
    public class LootPlayer : ModPlayer
    {
        private const int BagWindow = 2;

        private readonly Dictionary<Type, Queue<int>> recentByPool = new();

        public bool WasRecentlyOffered(Type poolType, int itemType)
        {
            return recentByPool.TryGetValue(poolType, out var queue) && queue.Contains(itemType);
        }

        public void RecordOffered(Type poolType, int itemType)
        {
            if (!recentByPool.TryGetValue(poolType, out var queue))
            {
                queue = new Queue<int>(BagWindow);
                recentByPool[poolType] = queue;
            }
            queue.Enqueue(itemType);
            while (queue.Count > BagWindow) queue.Dequeue();
        }

        public override void OnEnterWorld()
        {
            recentByPool.Clear();
        }
    }
}
