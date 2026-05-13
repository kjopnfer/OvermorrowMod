using OvermorrowMod.Common.Utilities;
using System;
using Terraria;

namespace OvermorrowMod.Common.TextureMapping
{
    public delegate void TexPlaceFunction(int x, int y);

    public static class TexPlaceAction
    {
        public static TexPlaceFunction PlaceTile(int tileId) => (x, y) => WorldGenUtils.PlaceTile(x, y, (ushort)tileId);

        public static TexPlaceFunction PlaceWall(int wallId) => (x, y) => WorldGenUtils.SetWall(x, y, (ushort)wallId);

        public static TexPlaceFunction PlaceObject(int tileId, int style = 0, int styleRange = 1, int direction = -1) => (x, y) =>
        {
            int s = styleRange > 1 ? style + Main.rand.Next(styleRange) : style;
            WorldGen.PlaceObject(x, y, tileId, true, s, 0, -1, direction);
        };

        public static TexPlaceFunction CustomPlaceObject(Action<int, int> customAction) => (x, y) =>
        {
            customAction(x, y);
        };

        /// <summary>Wipes both tile and wall at the position.</summary>
        public static readonly TexPlaceFunction Clear = (x, y) =>
        {
            WorldGenUtils.ClearTile(x, y);
            WorldGenUtils.ClearWall(x, y);
        };
    }
}
