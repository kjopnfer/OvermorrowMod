using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class RedWood : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            drop = ModContent.ItemType<Items.Placeable.Tiles.RedWood>();
            mineResist = 2f;
            AddMapEntry(new Color(92, 64, 51));
        }
    }
}