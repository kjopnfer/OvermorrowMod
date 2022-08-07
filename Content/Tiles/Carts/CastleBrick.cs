using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Carts
{
    public class CastleBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<CastleRoof>()] = true;
            Main.tileMerge[ModContent.TileType<CastleRoof>()][Type] = true;

            MinPick = 95;
            MineResist = 2f;
            AddMapEntry(new Color(105, 99, 94));
        }
    }
}