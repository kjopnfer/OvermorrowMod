using OvermorrowMod.Content.Items;
using OvermorrowMod.Core.WorldGeneration;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.Globals
{
    /// <summary>
    /// Locks every hotbar and main inventory slot past the open counts with a <see cref="LockedSlot"/> placeholder while in a run.
    /// </summary>
    public class RunInventoryPlayer : ModPlayer
    {
        public const int OpenHotbarSlots = 3;
        public const int OpenInventorySlots = 5;
        private const int HotbarSlots = 10;

        private int lastSelectedItem;

        public static bool IsLocked(int slot) => (slot >= OpenHotbarSlots && slot < HotbarSlots) || (slot >= HotbarSlots + OpenInventorySlots && slot < Main.InventoryItemSlotsCount);

        public override void Load()
        {
            On_ItemSlot.LeftClick_ItemArray_int_int += BlockLockedSlotClick;
        }

        private static void BlockLockedSlotClick(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
        {
            if (inv == Main.LocalPlayer.inventory && inv[slot].ModItem is LockedSlot) return;
            orig(inv, context, slot);
        }

        public override void PreUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || !RunState.InRun) return;

            int lockedType = ModContent.ItemType<LockedSlot>();
            for (int i = 0; i < Main.InventoryItemSlotsCount; i++)
            {
                if (!IsLocked(i)) continue;

                Item item = Player.inventory[i];
                if (item.IsAir)
                {
                    item = new Item(lockedType);
                    Player.inventory[i] = item;
                }

                if (item.type == lockedType) item.favorited = true;
            }
        }

        /// <summary>
        /// Moves a selection that landed on a locked hotbar slot to the open slot on the far side, so scrolling wraps over the open slots only.
        /// </summary>
        public override void PostUpdate()
        {
            if (Player.whoAmI != Main.myPlayer || !RunState.InRun) return;

            if (Player.selectedItem < HotbarSlots && IsLocked(Player.selectedItem))
                Player.selectedItem = lastSelectedItem == 0 ? OpenHotbarSlots - 1 : 0;

            lastSelectedItem = Player.selectedItem;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!RunState.InRun) return;

            for (int i = OpenHotbarSlots; i < HotbarSlots; i++)
                triggersSet.KeyStatus["Hotbar" + (i + 1)] = false;
        }

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item) => item.ModItem is not LockedSlot;
    }
}
