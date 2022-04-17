using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class AstroPurple : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;

            Main.tileLighted[Type] = true;

            MinPick = 55;
            MineResist = 2f;
            ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.AstroPurple>();

            AddMapEntry(new Color(100, 83, 83));
        }
    }
}