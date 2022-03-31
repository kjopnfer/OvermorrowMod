using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.DesertTemple
{
    public class SandBrick : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;

            Main.tileMergeDirt[Type] = true;
            Main.tileMerge[Type][TileID.Sand] = true;
            Main.tileMerge[TileID.Sand][Type] = true;

            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            minPick = 999;
            mineResist = 600f;
            drop = ModContent.ItemType<Items.Placeable.Tiles.SandBrick>();

            AddMapEntry(new Color(91, 57, 28));
        }
    }
}