using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.Loot
{
    public static class LootPoolRegistry
    {
        private static readonly Dictionary<Type, LootPool> pools = new();
        private static readonly Dictionary<Type, Func<bool>> activators = new();

        public static void Register<T>(Func<bool> isActive) where T : LootPool, new()
        {
            pools[typeof(T)] = new T();
            activators[typeof(T)] = isActive;
        }

        public static LootPool Get(Type poolType)
        {
            pools.TryGetValue(poolType, out var pool);
            return pool;
        }

        public static LootPool Get<T>() where T : LootPool
        {
            return Get(typeof(T));
        }

        public static LootPool GetActive()
        {
            foreach (var (type, isActive) in activators)
            {
                if (isActive()) return pools[type];
            }
            return null;
        }

        public static IEnumerable<Type> AllPoolTypes() => pools.Keys;

        public static void Clear()
        {
            pools.Clear();
            activators.Clear();
        }
    }
}
