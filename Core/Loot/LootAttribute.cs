using System;

namespace OvermorrowMod.Core.Loot
{
    /// <summary>
    /// Wildcard loot tag. Items with this attribute appear in every registered pool
    /// (any pool that does not have a more specific [Loot&lt;TPool&gt;] override).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class LootAttribute : Attribute
    {
        public ItemType Affinities { get; }
        public Rarity Rarity { get; }

        public LootAttribute(ItemType affinities, Rarity rarity)
        {
            Affinities = affinities;
            Rarity = rarity;
        }
    }

    /// <summary>
    /// Pool-specific loot tag. The type parameter selects the pool the item belongs to.
    /// Stack multiple instances to register the same item under different pools, with
    /// per-pool affinity and rarity values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class LootAttribute<TPool> : Attribute where TPool : LootPool
    {
        public ItemType Affinities { get; }
        public Rarity Rarity { get; }
        public Type PoolType => typeof(TPool);

        public LootAttribute(ItemType affinities, Rarity rarity)
        {
            Affinities = affinities;
            Rarity = rarity;
        }
    }
}
