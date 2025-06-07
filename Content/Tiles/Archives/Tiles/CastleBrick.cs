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

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // IMPORTANT:
            // There is a strange bug with DrawBlack which somehow makes the tiles invisible when there is little light
            // This emits a minuscule amount of light to prevent these tiles from disappearing
            r = 0.1f;
            base.ModifyLight(i, j, ref r, ref g, ref b);
        }
    }
}