using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public class CrunchyStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileMerge[Type][TileID.Stone] = true;
            Main.tileMerge[TileID.Stone][Type] = true;

            Main.tileMerge[Type][TileID.Mud] = true;
            Main.tileMerge[TileID.Mud][Type] = true;

            Main.tileMerge[Type][TileID.Silt] = true;
            Main.tileMerge[TileID.Silt][Type] = true;

            Main.tileMerge[Type][TileID.Ash] = true;
            Main.tileMerge[TileID.Ash][Type] = true;

            Main.tileLighted[Type] = true;

            MinPick = 55;
            MineResist = 3f;

            AddMapEntry(new Color(79, 86, 97));
        }
    }
}