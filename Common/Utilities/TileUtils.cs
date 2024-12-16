using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.CodeAnalysis.Text;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;

namespace OvermorrowMod.Common.Utilities
{
    public static class TileUtils
    {
        public enum CornerType : byte
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public static Vector2 TileAdj => (Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy) ? Vector2.Zero : Vector2.One * 12;

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

        /// <summary>
        /// Finds the nearest solid or sloped ground tile beneath a given starting position.
        /// The method starts from the given position and moves downwards to find the first tile that is either solid or sloped.
        /// </summary>
        /// <param name="startPosition">The starting position to begin the search from. This position is in world coordinates.</param>
        /// <returns>
        /// A <see cref="Vector2"/> representing the position of the nearest solid or sloped ground tile. The X coordinate remains unchanged, while the Y coordinate is updated to the Y position of the found ground tile.
        /// </returns>
        public static Vector2 FindNearestGround(Vector2 startPosition)
        {
            Vector2 position = startPosition;

            // Convert the start position to tile coordinates
            Point tilePosition = position.ToTileCoordinates();

            // If inside a solid tile, move upwards until an empty tile is found
            while (WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(tilePosition.X, tilePosition.Y)))
            {
                tilePosition.Y--; // Move up one tile
            }

            // Search downwards for a solid or sloped ground tile
            while (!WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(tilePosition.X, tilePosition.Y)))
            {
                tilePosition.Y++; // Move down one tile
            }

            // If a solid or sloped tile is found, update the Y position of the input vector
            if (WorldGen.SolidOrSlopedTile(Framing.GetTileSafely(tilePosition.X, tilePosition.Y)))
            {
                position.Y = tilePosition.ToWorldCoordinates(0f, 0f).Y; // Update the Y position to the found tile's world Y coordinate
            }

            return position; // Return the updated position with the Y of the nearest ground tile
        }

        /// <summary>
        /// Tries to find an entity of the specified Type. Returns whether or not it found the
        /// entity or not.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="x"> The x coordinate of the potential entity. </param>
        /// <param name="y"> The y coordinate of the potential entity. </param>
        /// <param name="entity"> The potential entity. </param>
        public static bool TryFindModTileEntity<T>(int x, int y, out T entity) where T : ModTileEntity
        {
            TileEntity.ByPosition.TryGetValue(new Point16(x, y), out TileEntity retrievedEntity);

            if (retrievedEntity is T castEntity)
            {
                entity = castEntity;
                return true;
            }

            entity = null;
            return false;
        }

        public static Point GetCornerOfMultiTile(Tile tile, int x, int y, CornerType corner)
        {
            TileObjectData data = TileObjectData.GetTileData(tile);
            Point topLeft = new(x - tile.TileFrameX % data.CoordinateFullWidth / 18, y - tile.TileFrameY % data.CoordinateFullHeight / 18);

            return corner switch
            {
                CornerType.TopLeft => topLeft,
                CornerType.TopRight => topLeft + new Point(data.Width - 1, 0),
                CornerType.BottomLeft => topLeft + new Point(0, data.Height - 1),
                CornerType.BottomRight => topLeft + new Point(data.Width - 1, data.Height - 1),
                _ => topLeft
            };
        }

        public static T FindTE<T>(int i, int j) where T : TileEntity
        {
            Tile tile = Main.tile[i, j];
            int left = i - tile.TileFrameX / 18;
            int top = j - tile.TileFrameY / 18;

            Main.NewText("searching for TE");
            // Get the instance of the template class for T
            var instance = ModContent.GetInstance<T>();

            // Use reflection to find the 'Find' method dynamically (if it exists)
            var findMethod = typeof(T).GetMethod("Find", new Type[] { typeof(int), typeof(int) });

            if (findMethod != null)
            {
                // Invoke the 'Find' method with the coordinates dynamically
                var result = findMethod.Invoke(instance, new object[] { left, top });
                Main.NewText("reflected find method");

                if (result is T entity)
                {
                    return entity;
                }
            }

            Main.NewText("couldnt find");

            // If no Find method exists or no entity is found, return null
            return null;
        }

    }
}