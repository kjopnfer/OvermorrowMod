using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Placeable.Furniture
{
    public abstract class Tent : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 54;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 2000;
        }
    }
    public class BlueTent : Tent
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Tent");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Content.Tiles.Town.BlueTent>();
        }
    }

    public class GreenTent : Tent
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Green Tent");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Content.Tiles.Town.GreenTent>();
        }
    }

    public class BrownTent : Tent
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brown Tent");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.createTile = ModContent.TileType<Content.Tiles.Town.BrownTent>();
        }
    }
}