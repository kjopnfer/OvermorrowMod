using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class AstroGray : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;

            Main.tileLighted[Type] = true;

            minPick = 55;
            mineResist = 2f;
            drop = ModContent.ItemType<Items.Placeable.Tiles.AstroGray>();

            AddMapEntry(new Color(100, 83, 83));
        }
    }
}