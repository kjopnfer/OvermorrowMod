using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModTile;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Content.Tiles.UVBiome.UVStone
{
    public class UVStoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultraviolet Stone");
            Tooltip.SetDefault("Visible when exposed to ultraviolet light");
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
            Item.createTile = TileType<UVStoneBlock>();
        }
    }
    public abstract class uvStoneBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            int[] UVTiles = { TileType<UVStoneBlock>(), TileType<UVStoneBlockOff>() };
            if (!UVGlobalTile.UVTiles.Contains(UVTiles))
                UVGlobalTile.UVTiles.Add(UVTiles);
            Main.tileMerge[Type][UVTiles[0]] = true;
            Main.tileMerge[Type][UVTiles[1]] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;

            ItemDrop = ItemType<UVStoneItem>();
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ultraviolet Stone");
            AddMapEntry(new Color(69, 36, 84), name);
        }
    }
    public class UVStoneBlock : uvStoneBlock
    {
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.24f;
            g = 0.06f;
            b = 0.24f;
        }
    }
    public class UVStoneBlockOff : uvStoneBlock
    {
        
    }
}
