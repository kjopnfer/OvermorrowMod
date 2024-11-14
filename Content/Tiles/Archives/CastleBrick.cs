using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class CastleBrick : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            //Main.tileMerge[Type][ModContent.TileType<CastleRoof>()] = true;
            //Main.tileMerge[ModContent.TileType<CastleRoof>()][Type] = true;

            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;

            MinPick = 95;
            MineResist = 2f;

            AddMapEntry(new Color(105, 99, 94));
        }
    }
}