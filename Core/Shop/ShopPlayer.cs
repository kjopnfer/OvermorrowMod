using OvermorrowMod.Content.NPCs.Archives.Shop;
using OvermorrowMod.Core.Items.Collectibles;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Shop
{
    public class ShopPlayer : ModPlayer
    {
        public override bool CanBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (vendor?.ModNPC is CartShopkeeper cart && cart.WasPurchased(item.type))
                return false;

            return true;
        }

        public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item)
        {
            if (item.ModItem is CollectibleItem collectible)
            {
                collectible.Apply(Player, 1);
                collectible.ShowPickup(Player);
                RemoveBoughtCopy(item.type);
            }

            if (vendor?.ModNPC is not CartShopkeeper cart) return;

            cart.MarkPurchased(item.type);

            for (int i = 0; i < shopInventory.Length; i++)
            {
                if (shopInventory[i] != null && shopInventory[i].type == item.type)
                {
                    shopInventory[i].TurnToAir();
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the purchased copy vanilla handed over: from the cursor on a normal buy, or from the inventory on a shift-click buy.
        /// </summary>
        private void RemoveBoughtCopy(int type)
        {
            if (Main.mouseItem.type == type)
            {
                Main.mouseItem.TurnToAir();
                return;
            }

            foreach (Item slot in Player.inventory)
            {
                if (slot.type == type)
                {
                    slot.TurnToAir();
                    return;
                }
            }
        }
    }
}
