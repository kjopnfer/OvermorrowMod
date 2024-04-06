using Microsoft.Xna.Framework;
using OvermorrowMod.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public class SlimedStone : ModTile
    {
        public override string Texture => AssetDirectory.Content + "Tiles/Underground/Slime/SlimedStone";

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

            AddMapEntry(new Color(32, 90, 212));
        }
    }
}