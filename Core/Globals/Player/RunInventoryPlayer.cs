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
    /// Coin and ammo slots are never locked.
    /// </summary>
    public class RunInventoryPlayer : ModPlayer
    {
        public const int OpenHotbarSlots = 3;
        public const int OpenInventorySlots = 5;
        private const int HotbarSlots = 10;

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

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!RunState.InRun) return;

            for (int i = OpenHotbarSlots; i < HotbarSlots; i++)
                triggersSet.KeyStatus["Hotbar" + (i + 1)] = false;
        }

        public override bool CanSellItem(NPC vendor, Item[] shopInventory, Item item) => item.ModItem is not LockedSlot;
    }
}
