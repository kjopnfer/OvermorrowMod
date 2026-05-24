using OvermorrowMod.Content.Items;
using OvermorrowMod.Core.WorldGeneration.TestSubworld;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Globals
{
    public class SubworldPlayer : ModPlayer
    {
        public Item[] mainInventory;
        public Item[] mainArmor;
        public Item[] mainMisc;
        public Item[] mainMiscDyes;
        public Item[] subInventory;
        public Item[] subArmor;
        public Item[] subMisc;
        public Item[] subMiscDyes;
        public bool inSubworldNow;

        // Loadout staged by the LoadoutSelection UI right before SubworldSystem.Enter. Consumed in OnEnterWorld.
        public Item pendingLoadout;
        public Item pendingMisc;

        // Persistent loadout shown in the LoadoutSelection UI slots.
        public Item loadoutWeapon;
        public Item loadoutMisc;

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

            if (tag.ContainsKey("LoadoutWeapon")) loadoutWeapon = ItemIO.Load(tag.Get<TagCompound>("LoadoutWeapon"));
            if (tag.ContainsKey("LoadoutMisc")) loadoutMisc = ItemIO.Load(tag.Get<TagCompound>("LoadoutMisc"));
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

            if (loadoutWeapon != null && !loadoutWeapon.IsAir) tag["LoadoutWeapon"] = ItemIO.Save(loadoutWeapon);
            if (loadoutMisc != null && !loadoutMisc.IsAir) tag["LoadoutMisc"] = ItemIO.Save(loadoutMisc);
        }

        public override void OnEnterWorld()
        {
            bool wantSubworld = SubworldSystem.IsActive<TestSubworld>();

            if (wantSubworld != inSubworldNow)
            {
                if (wantSubworld)
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
                inSubworldNow = wantSubworld;
            }

            if (wantSubworld)
            {
                if (pendingLoadout != null && !pendingLoadout.IsAir)
                {
                    Player.inventory[0] = pendingLoadout.Clone();
                }
                if (pendingMisc != null && !pendingMisc.IsAir)
                {
                    Player.inventory[1] = pendingMisc.Clone();
                }
            }
            pendingLoadout = null;
            pendingMisc = null;

            GrantTestKey();
        }

        private void GrantTestKey()
        {
            int keyType = ModContent.ItemType<TestKey>();
            if (Player.inventory[9] == null || Player.inventory[9].IsAir || Player.inventory[9].type == keyType)
            {
                var key = new Item();
                key.SetDefaults(keyType);
                Player.inventory[9] = key;
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
