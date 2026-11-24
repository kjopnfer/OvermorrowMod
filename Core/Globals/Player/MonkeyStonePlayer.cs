using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Core.WorldGeneration;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    public class MonkeyStonePlayer : ModPlayer
    {
        private readonly int[] banked = new int[Enum.GetValues<MonkeyStoneTier>().Length];
        private readonly int[] gainedThisRun = new int[Enum.GetValues<MonkeyStoneTier>().Length];

        public int Get(MonkeyStoneTier tier) => banked[(int)tier];

        public int GetGainedThisRun(MonkeyStoneTier tier) => gainedThisRun[(int)tier];

        public void Add(MonkeyStoneTier tier, int amount)
        {
            banked[(int)tier] += amount;
            if (RunState.InRun) gainedThisRun[(int)tier] += amount;
        }

        public void ClearRun() => Array.Clear(gainedThisRun);

        public bool TrySpend(MonkeyStoneTier tier, int amount)
        {
            if (banked[(int)tier] < amount) return false;

            banked[(int)tier] -= amount;
            return true;
        }

        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer) return;

            foreach (Item item in Player.inventory)
                Bank(item);

            Bank(Main.mouseItem);
        }

        private void Bank(Item item)
        {
            if (item.ModItem is not MonkeyStone stone) return;

            Add(stone.Tier, item.stack);
            item.TurnToAir();
        }

        public override void SaveData(TagCompound tag)
        {
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
            {
                tag["MonkeyStones" + tier] = banked[(int)tier];
                tag["MonkeyStonesGained" + tier] = gainedThisRun[(int)tier];
            }
        }

        public override void LoadData(TagCompound tag)
        {
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
            {
                banked[(int)tier] = tag.GetInt("MonkeyStones" + tier);
                gainedThisRun[(int)tier] = tag.GetInt("MonkeyStonesGained" + tier);
            }
        }
    }
}
