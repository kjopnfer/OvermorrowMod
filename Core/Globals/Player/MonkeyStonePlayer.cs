using OvermorrowMod.Content.Items.Archives;
using System;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    /// <summary>
    /// The player's banked Monkey Stones, one count per tier. Stones never occupy inventory slots, so runs and deaths leave the counts alone.
    /// </summary>
    public class MonkeyStonePlayer : ModPlayer
    {
        private readonly int[] banked = new int[Enum.GetValues<MonkeyStoneTier>().Length];

        public int Get(MonkeyStoneTier tier) => banked[(int)tier];

        public void Add(MonkeyStoneTier tier, int amount) => banked[(int)tier] += amount;

        public bool TrySpend(MonkeyStoneTier tier, int amount)
        {
            if (banked[(int)tier] < amount) return false;

            banked[(int)tier] -= amount;
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
                tag["MonkeyStones" + tier] = banked[(int)tier];
        }

        public override void LoadData(TagCompound tag)
        {
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
                banked[(int)tier] = tag.GetInt("MonkeyStones" + tier);
        }
    }
}
