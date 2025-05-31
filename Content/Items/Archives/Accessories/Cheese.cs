using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class Cheese : ModItem
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.aggro -= ModUtils.TilesToPixels(10);
            player.GetModPlayer<AccessoryPlayer>().AlertBonus = ModUtils.TilesToPixels(10);
            player.GetModPlayer<AccessoryPlayer>().AggroLossBonus = 0.5f;
        }
    }
}