using OvermorrowMod.Core.Items.Collectibles;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.MetaUpgrades;
using OvermorrowMod.Core.Runs;
using OvermorrowMod.Core.WorldGeneration;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    public class SubworldPlayer : ModPlayer
    {
        public const int BaseMaxLife = 100;

        public Item[] mainInventory;
        public Item[] mainArmor;
        public Item[] mainMisc;
        public Item[] mainMiscDyes;
        public Item[] subInventory;
        public Item[] subArmor;
        public Item[] subMisc;
        public Item[] subMiscDyes;
        public bool inSubworldNow;

        // Class loadout staged by the ClassSelection UI right before SubworldSystem.Enter. Consumed in OnEnterWorld.
        public List<Item> pendingClassItems;

        public override void LoadData(TagCompound tag)
        {
            mainInventory = tag.Get<Item[]>("MainInventory");
            mainArmor = tag.Get<Item[]>("MainArmor");
            mainMisc = tag.Get<Item[]>("MainMisc");
            mainMiscDyes = tag.Get<Item[]>("MainMiscDyes");
            subInventory = tag.Get<Item[]>("SubInventory");
            subArmor = tag.Get<Item[]>("SubArmor");
            subMisc = tag.Get<Item[]>("SubMisc");
            subMiscDyes = tag.Get<Item[]>("SubMiscDyes");
            inSubworldNow = tag.GetBool("InSubworldNow");
        }

        public override void SaveData(TagCompound tag)
        {
            if (inSubworldNow)
            {
                subInventory = ClonedSlots(Player.inventory);
                subArmor = ClonedSlots(Player.armor);
                subMisc = ClonedSlots(Player.miscEquips);
                subMiscDyes = ClonedSlots(Player.miscDyes);
            }
            if (mainInventory != null) tag["MainInventory"] = mainInventory;
            if (mainArmor != null) tag["MainArmor"] = mainArmor;
            if (mainMisc != null) tag["MainMisc"] = mainMisc;
            if (mainMiscDyes != null) tag["MainMiscDyes"] = mainMiscDyes;
            if (subInventory != null) tag["SubInventory"] = subInventory;
            if (subArmor != null) tag["SubArmor"] = subArmor;
            if (subMisc != null) tag["SubMisc"] = subMisc;
            if (subMiscDyes != null) tag["SubMiscDyes"] = subMiscDyes;
            tag["InSubworldNow"] = inSubworldNow;
        }

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;

            if (SubworldSystem.AnyActive() && Player.statLifeMax > BaseMaxLife)
            {
                health = new StatModifier(1f, 1f, 0f, -(Player.statLifeMax - BaseMaxLife));
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (SubworldSystem.AnyActive())
                regen = 0f;
        }

        public override void OnEnterWorld()
        {
            bool inSubworld = RunState.InRun;

            if (inSubworld != inSubworldNow)
            {
                if (inSubworld)
                {
                    mainInventory = ClonedSlots(Player.inventory);
                    mainArmor = ClonedSlots(Player.armor);
                    mainMisc = ClonedSlots(Player.miscEquips);
                    mainMiscDyes = ClonedSlots(Player.miscDyes);
                    ApplySlots(subInventory, Player.inventory);
                    ApplySlots(subArmor, Player.armor);
                    ApplySlots(subMisc, Player.miscEquips);
                    ApplySlots(subMiscDyes, Player.miscDyes);
                }
                else
                {
                    subInventory = ClonedSlots(Player.inventory);
                    subArmor = ClonedSlots(Player.armor);
                    subMisc = ClonedSlots(Player.miscEquips);
                    subMiscDyes = ClonedSlots(Player.miscDyes);
                    ApplySlots(mainInventory, Player.inventory);
                    ApplySlots(mainArmor, Player.armor);
                    ApplySlots(mainMisc, Player.miscEquips);
                    ApplySlots(mainMiscDyes, Player.miscDyes);
                }
                inSubworldNow = inSubworld;
            }

            if (inSubworld)
            {
                if (pendingClassItems != null)
                {
                    for (int i = 0; i < Terraria.Player.MaxBuffs; i++)
                    {
                        Player.buffType[i] = 0;
                        Player.buffTime[i] = 0;
                    }

                    ClearRunItems();
                    Player.GetModPlayer<CollectiblePlayer>().Reset();
                    Player.GetModPlayer<RerollPlayer>().Reset();
                    Player.statLife = Player.statLifeMax2;

                    int slot = 0;
                    int ammoSlot = Main.InventoryAmmoSlotsStart;
                    foreach (Item item in pendingClassItems)
                    {
                        if (item == null || item.IsAir) continue;

                        Item kitItem = item.Clone();
                        RunCountedItem.MarkKit(kitItem);
                        if (kitItem.ammo != AmmoID.None) Player.inventory[ammoSlot++] = kitItem;
                        else Player.inventory[slot++] = kitItem;
                    }

                    GiveStartingCoins();
                    GivePackedPotions();
                }
            }
            pendingClassItems = null;
        }

        private void GiveStartingCoins()
        {
            int[] coins = Utils.CoinsSplit(Player.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.StartingPouch));
            int[] coinTypes = [ItemID.CopperCoin, ItemID.SilverCoin, ItemID.GoldCoin, ItemID.PlatinumCoin];
            for (int i = 0; i < coins.Length; i++)
            {
                if (coins[i] > 0) Player.inventory[Main.InventoryCoinSlotsStart + i] = new Item(coinTypes[i], coins[i]);
            }
        }

        private void GivePackedPotions()
        {
            MetaUpgradePlayer upgrades = Player.GetModPlayer<MetaUpgradePlayer>();
            int count = upgrades.Value(MetaUpgrade.PackedPotion);
            if (count == 0) return;

            int type = upgrades.Rank(MetaUpgrade.PackedPotion) >= 3 ? ItemID.HealingPotion : ItemID.LesserHealingPotion;
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                if (!Player.inventory[i].IsAir || InventoryPlayer.IsLocked(Player, i)) continue;

                Item potions = new(type, count);
                RunCountedItem.MarkKit(potions);
                Player.inventory[i] = potions;
                return;
            }
        }

        public void ClearRunInventory()
        {
            subInventory = null;
            subArmor = null;
            subMisc = null;
            subMiscDyes = null;
        }

        /// <summary>
        /// Empties the run inventory and equipment for a new run.
        /// </summary>
        private void ClearRunItems()
        {
            foreach (Item[] slots in new[] { Player.inventory, Player.armor, Player.dye, Player.miscEquips, Player.miscDyes })
            {
                for (int i = 0; i < slots.Length; i++) slots[i] = new Item();
            }
        }

        private static Item[] ClonedSlots(Item[] src)
        {
            var copy = new Item[src.Length];
            for (int i = 0; i < src.Length; i++) copy[i] = src[i].Clone();
            return copy;
        }

        private static void ApplySlots(Item[] source, Item[] target)
        {
            for (int i = 0; i < target.Length; i++)
            {
                if (source != null && i < source.Length && source[i] != null) target[i] = source[i].Clone();
                else target[i] = new Item();
            }
        }
    }
}
