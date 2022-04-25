using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.DesertTemple
{
    public class SandBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[TileID.Sand][Type] = true;

            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MinPick = 999;
            MineResist = 600f;
            ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.SandBrick>();

            AddMapEntry(new Color(91, 57, 28));
        }
    }
}