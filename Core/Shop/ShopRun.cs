using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

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

        public static List<(int type, int price)> GetStock(Player player)
        {
            stock ??= ArchiveShopPool.Roll(player);
            return stock;
        }

        public override void OnWorldLoad() => Reset();

        public override void ClearWorld() => Reset();

        private static void Reset()
        {
            stock = null;
            Purchased.Clear();
        }
    }
}
