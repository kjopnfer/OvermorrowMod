using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Common.Utilities
{
    public static class TileUtils
    {
        /// <summary>
        /// Returns the top-left coordinates of a tile object, adjusting for its frame offsets.
        /// If the tile is not part of a tile object, the original coordinates are returned.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <returns>A <see cref="Vector2"/> with the top-left coordinates of the tile object, or the original coordinates if no tile object is found.</returns>
        public static Vector2 FindTopLeft(int x, int y)
        {
            // Retrieve the tile at the specified coordinates
            Tile tile = Framing.GetTileSafely(x, y);
            if (tile == null) return new Vector2(x, y); // Return original if tile is invalid

            // Get the tile object data (if any) for the tile
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, 0);

            if (data == null) return new Vector2(x, y); // Return original if no tile object data

            // Adjust the coordinates based on the tile's frame and object dimensions
            x -= tile.TileFrameX / 18 % data.Width;
            y -= tile.TileFrameY / 18 % data.Height;

            return new Vector2(x, y); // Return the adjusted top-left position
        }
    }
}