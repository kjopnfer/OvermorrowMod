using OvermorrowMod.Core.MetaUpgrades;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Core.Shop
{
    /// <summary>
    /// Per-run shop stock and purchase record. 
    /// The stock is rolled once and reused for the whole subworld.
    /// </summary>
    public class ShopRun : ModSystem
    {
        private static List<(int type, int price)> stock;

        public static readonly HashSet<int> Purchased = new();

        public static bool Restocked { get; private set; }

        public static List<(int type, int price)> GetStock(Player player)
        {
            stock ??= ArchiveShopPool.Roll(player, player.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.ExtraShelf));
            return stock;
        }

        public static void Restock(Player player)
        {
            stock = ArchiveShopPool.Roll(player, player.GetModPlayer<MetaUpgradePlayer>().Value(MetaUpgrade.ExtraShelf));
            Purchased.Clear();
            Restocked = true;
        }

        public override void OnWorldLoad() => Reset();

        public override void SaveWorldData(TagCompound tag)
        {
            if (stock == null) return;

            tag["ShopTypes"] = stock.Select(entry => entry.type).ToList();
            tag["ShopPrices"] = stock.Select(entry => entry.price).ToList();
            tag["ShopPurchased"] = Purchased.ToList();
            tag["ShopRestocked"] = Restocked;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.ContainsKey("ShopTypes")) return;

            IList<int> types = tag.GetList<int>("ShopTypes");
            IList<int> prices = tag.GetList<int>("ShopPrices");
            stock = types.Select((type, i) => (type, prices[i])).ToList();
            Purchased.UnionWith(tag.GetList<int>("ShopPurchased"));
            Restocked = tag.GetBool("ShopRestocked");
        }

        public override void ClearWorld() => Reset();

        private static void Reset()
        {
            stock = null;
            Purchased.Clear();
            Restocked = false;
        }
    }
}
