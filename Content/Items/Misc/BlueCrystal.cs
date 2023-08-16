using OvermorrowMod.Content.Tiles.Ambient;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Misc
{
    public class BlueCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Resonance Crystal of Eternity");
            // Tooltip.SetDefault("Used to enhance your physical form");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<BlueCrystal1>();
        }
    }
}