using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class PurpleStone : ModTile
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
            ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.PurpleStone>();

            AddMapEntry(new Color(100, 83, 83));
        }
    }
}