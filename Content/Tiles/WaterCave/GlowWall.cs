using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Tiles.WaterCave
{
    public class GlowWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(23, 71, 71));
        }

        /*public override void RandomUpdate(int i, int j)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            tile.liquidType(Tile.Liquid_Water);
            tile.liquid = byte.MaxValue;
            WorldGen.SquareTileFrame(i, j, true);
        }*/
    }
}