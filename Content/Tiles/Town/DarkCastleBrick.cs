using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class DarkCastleBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            Main.tileMerge[Type][ModContent.TileType<CastleRoof>()] = true;
            Main.tileMerge[ModContent.TileType<CastleRoof>()][Type] = true;

            Main.tileMerge[Type][ModContent.TileType<CastleBrick>()] = true;
            Main.tileMerge[ModContent.TileType<CastleBrick>()][Type] = true;

            Main.tileMerge[Type][TileID.Grass] = true;
            Main.tileMerge[TileID.Grass][Type] = true;

            MinPick = 95;
            MineResist = 2f;

            ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.RuinBrick>();
            AddMapEntry(new Color(67, 65, 64));
        }
    }
}