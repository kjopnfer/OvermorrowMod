using OvermorrowMod.Content.Items.Archives;
using System;
using Terraria;
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

        /// <summary>
        /// Banks any stone that reached the inventory or cursor without being picked up from the world.
        /// </summary>
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
                tag["MonkeyStones" + tier] = banked[(int)tier];
        }

        public override void LoadData(TagCompound tag)
        {
            foreach (MonkeyStoneTier tier in Enum.GetValues<MonkeyStoneTier>())
                banked[(int)tier] = tag.GetInt("MonkeyStones" + tier);
        }
    }
}
