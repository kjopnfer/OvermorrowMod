using OvermorrowMod.Tiles.Ambient;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Items.Misc
{
    public class BlueCrystal : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Resonance Crystal of Eternity");
            Tooltip.SetDefault("Used to enhance your physical form");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 28;
            item.rare = ItemRarityID.Green;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<BlueCrystal1>();
        }
    }
}