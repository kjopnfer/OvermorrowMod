using System;

namespace OvermorrowMod.Core.Loot
{
    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
    }

    public readonly struct RarityWeights
    {
        public readonly int Common;
        public readonly int Rare;
        public readonly int Epic;
        public readonly int Legendary;

        public RarityWeights(int common, int rare, int epic, int legendary = 0)
        {
            Common = common;
            Rare = rare;
            Epic = epic;
            Legendary = legendary;
        }

        public int Sum => Common + Rare + Epic + Legendary;

        public Rarity Sample(Terraria.Utilities.UnifiedRandom rng)
        {
            int sum = Sum;
            if (sum <= 0) return Rarity.Common;
            int pick = rng.Next(sum);
            if (pick < Common) return Rarity.Common;
            if (pick < Common + Rare) return Rarity.Rare;
            if (pick < Common + Rare + Epic) return Rarity.Epic;
            return Rarity.Legendary;
        }

        public RarityWeights Clamped()
        {
            return new RarityWeights(Math.Max(0, Common), Math.Max(0, Rare), Math.Max(0, Epic), Math.Max(0, Legendary));
        }
    }

    public readonly struct RarityModifier
    {
        public readonly int Common;
        public readonly int Rare;
        public readonly int Epic;
        public readonly int Legendary;

        public RarityModifier(int common = 0, int rare = 0, int epic = 0, int legendary = 0)
        {
            Common = common;
            Rare = rare;
            Epic = epic;
            Legendary = legendary;
        }

        public static RarityWeights operator +(RarityWeights weights, RarityModifier mod)
        {
            return new RarityWeights(weights.Common + mod.Common, weights.Rare + mod.Rare, weights.Epic + mod.Epic, weights.Legendary + mod.Legendary).Clamped();
        }
    }
}
