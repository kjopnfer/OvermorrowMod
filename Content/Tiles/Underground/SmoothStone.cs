using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Underground
{
    public class SmoothStone : ModTile
    {
        public override void SetDefaults()
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

            Main.tileMerge[Type][ModContent.TileType<CrunchyStone>()] = true;
            Main.tileMerge[ModContent.TileType<CrunchyStone>()][Type] = true;

            Main.tileLighted[Type] = true;

            minPick = 55;
            mineResist = 2f;
            drop = ModContent.ItemType<Items.Placeable.Tiles.SmoothStone>();

            AddMapEntry(new Color(100, 83, 83));
        }
    }
}