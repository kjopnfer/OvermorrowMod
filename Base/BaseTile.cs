using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Map;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Localization;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading;
using System.Collections.Generic;

namespace OvermorrowMod
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
            TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
            x -= (tile.frameX / 18) % data.Width;
            y -= (tile.frameY / 18) % data.Height;
            return new Vector2(x, y);
        }
    }
}