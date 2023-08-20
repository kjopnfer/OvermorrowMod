using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModTile;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Tiles.UVBiome.UVRubble
{
    public class UVRubbleItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultraviolet Rubble");
            Tooltip.SetDefault("visible while the ultraviolet goggles are equipped");
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = TileType<UVRubbleBlock>();
        }
    }
    public abstract class uvRubbleBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            int[] UVTiles = { TileType<UVRubbleBlock>(), TileType<UVRubbleBlockOff>() };
            if (!UVGlobalTile.UVTiles.Contains(UVTiles))
                UVGlobalTile.UVTiles.Add(UVTiles);
            Main.tileMerge[Type][UVTiles[0]] = true;
            Main.tileMerge[Type][UVTiles[1]] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;

            ItemDrop = ItemType<UVRubbleItem>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ultraviolet Rubble");
            AddMapEntry(new Color(44, 33, 57), name);
        }
    }
    public class UVRubbleBlock : uvRubbleBlock
    {
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.24f;
            g = 0.06f;
            b = 0.24f;
        }
    }
    public class UVRubbleBlockOff : uvRubbleBlock
    {

    }
}
