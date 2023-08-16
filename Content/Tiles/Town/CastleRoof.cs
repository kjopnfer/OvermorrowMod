using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.Town
{
    public class CastleRoof : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;

            Main.tileLighted[Type] = true;

            MinPick = 95;
            MineResist = 2f;

            AddMapEntry(new Color(117, 70, 46));
        }
    }
}