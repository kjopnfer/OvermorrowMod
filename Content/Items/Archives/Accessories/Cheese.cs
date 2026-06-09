using OvermorrowMod.Common;
using OvermorrowMod.Common.Items;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class Cheese : OvermorrowAccessory
    {
        public override string Texture => AssetDirectory.ArchiveItems + Name;
        protected override void SafeSetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(0, gold: 1, silver: 50, copper: 0);
        }

        protected override void UpdateAccessoryEffects(Player player)
        {
            player.aggro -= ModUtils.TilesToPixels(10);
            player.GetModPlayer<GlobalPlayer>().AlertBonus = ModUtils.TilesToPixels(10);
            player.GetModPlayer<GlobalPlayer>().AggroLossBonus = 0.5f;
        }

        protected override void SetAccessoryEffects(AccessoryDefinition definition)
        {
        }
    }
}