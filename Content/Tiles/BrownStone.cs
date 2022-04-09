using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class BrownStone : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            drop = ModContent.ItemType<Items.Placeable.Tiles.BrownStone>();
            mineResist = 2f;
            AddMapEntry(new Color(79, 86, 97));
        }
    }
}