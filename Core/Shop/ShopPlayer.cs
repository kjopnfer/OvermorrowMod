using OvermorrowMod.Content.NPCs.Archives.Shop;
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
    }
}
