using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModTile;
using static Terraria.ModLoader.ModContent;
using static Terraria.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Content.Tiles.UVBiome.GlimsporeVines;

namespace OvermorrowMod.Content.Tiles.UVBiome.UVSoil
{
    public class UVSoilItem : ModItem
    {

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
            Item.createTile = TileType<UVSoilOff>();
        }
    }
    public abstract class uvSoil : ModTile
    {
        public override void SetStaticDefaults()
        {
            int[] UVTiles = { TileType<UVSoil>(), TileType<UVSoilOff>() };
            if (!UVGlobalTile.UVTiles.Contains(UVTiles))
                UVGlobalTile.UVTiles.Add(UVTiles);
            Main.tileMerge[Type][UVTiles[0]] = true;
            Main.tileMerge[Type][UVTiles[1]] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;

            Main.tileMerge[Type][TileType<GlimsporeVine>()] = true;
            RegisterItemDrop(ItemType<UVSoilItem>());
            /*AdjTiles = new int[] { TileID.Dirt, TileID.Grass };
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Ultraviolet Soil");*/
            AddMapEntry(new Color(52, 37, 115));
        }
    }
    public class UVSoil : uvSoil
    {
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.24f;
            g = 0.06f;
            b = 0.24f;
        }
    }
    public class UVSoilOff : uvSoil
    {
        
    }
}
