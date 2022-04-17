using Terraria;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Common.Base
{
    public class BaseTile
    {
        //------------------------------------------------------//
        //-------------------BASE TILE CLASS--------------------//
        //------------------------------------------------------//
        // Contains methods dealing with tiles, except          //
        // generation. (for that, see BaseWorldGen/BaseGoreGen) //
        //------------------------------------------------------//
        //  Author(s): Grox the Great                           //
        //------------------------------------------------------//

        public static Vector2 FindTopLeft(int x, int y)
        {
            Tile tile = Main.tile[x, y]; if (tile == null) return new Vector2(x, y);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);
            x -= (tile.TileFrameX / 18) % data.Width;
            y -= (tile.TileFrameY / 18) % data.Height;
            return new Vector2(x, y);
        }
    }
}