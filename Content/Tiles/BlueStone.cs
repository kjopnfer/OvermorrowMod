using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles
{
    public class BlueStone : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            ItemDrop = ModContent.ItemType<Items.Placeable.Tiles.BlueStone>();
            MineResist = 2f;
            AddMapEntry(new Color(79, 86, 97));
        }
    }
}