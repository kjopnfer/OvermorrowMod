using Terraria.ObjectData;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.CodeAnalysis.Text;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using System.Reflection;

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

        /// <summary>
        /// Places a <typeparamref name="Tile"/> at the specified coordinates and associates a <typeparamref name="TEntity"/>.
        /// The tile entity is created and placed dynamically, and the method returns the placed tile entity instance.
        /// </summary>
        /// <typeparam name="Tile">The type of the tile to place, which must inherit from <see cref="ModTile"/>.</typeparam>
        /// <typeparam name="TEntity">The type of the tile entity associated with the tile, which must inherit from <see cref="TileEntity"/>.</typeparam>
        /// <param name="x">The x-coordinate where the tile will be placed.</param>
        /// <param name="y">The y-coordinate where the tile will be placed.</param>
        /// <returns>
        /// Returns an instance of <typeparamref name="TEntity"/> that was placed at the specified coordinates, or <c>null</c> if the place operation fails.
        /// </returns>
        /// <remarks>
        /// This method uses reflection to invoke the <see cref="TileEntity.Place"/> method dynamically, if it exists, to place the tile entity. 
        /// If the <see cref="Place"/> method is not found or the tile entity cannot be placed, it returns <c>null</c>.
        /// </remarks>
        public static TEntity PlaceTileWithEntity<Tile, TEntity>(int x, int y) where Tile : ModTile where TEntity : TileEntity, new()
        {
            WorldGen.PlaceObject(x, y, ModContent.TileType<Tile>());

            // Create an instance of the tile entity
            TEntity entity = new TEntity();

            // Use reflection to invoke the Place method if it exists
            MethodInfo placeMethod = typeof(TEntity).GetMethod("Place", BindingFlags.Public | BindingFlags.Instance);

            if (placeMethod != null)
            {
                // Call the Place method dynamically
                int id = (int)placeMethod.Invoke(entity, new object[] { x, y });

                // Return the placed entity
                return TileEntity.ByID[id] as TEntity;
            }

            // Return null if the Place method is not found
            return null;
        }

        /// <summary>
        /// Gets the position of a specific corner of a multi-tile object based on the provided tile coordinates and corner type.
        /// </summary>
        /// <param name="x">The X coordinate of the tile in the world.</param>
        /// <param name="y">The Y coordinate of the tile in the world.</param>
        /// <param name="corner">The corner of the multi-tile to retrieve. Should be one of the values from the <see cref="CornerType"/> enum.</param>
        /// <returns>
        /// The position of the requested corner relative to the tile's world coordinates. The corner is specified using one of the
        /// <see cref="CornerType"/> values, and the returned point represents the world position of that corner.
        /// </returns>
        /// <remarks>
        /// The method computes the starting position of the tile object by considering its frame and data, then it adjusts this position 
        /// based on the requested corner. The four possible corner types are:
        /// <list type="bullet">
        ///     <item>
        ///         <description><see cref="CornerType.TopLeft"/>: The top-left corner of the multi-tile.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="CornerType.TopRight"/>: The top-right corner of the multi-tile.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="CornerType.BottomLeft"/>: The bottom-left corner of the multi-tile.</description>
        ///     </item>
        ///     <item>
        ///         <description><see cref="CornerType.BottomRight"/>: The bottom-right corner of the multi-tile.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static Point GetCornerOfMultiTile( int x, int y, CornerType corner)
        {
            Tile tile = Framing.GetTileSafely(x, y);
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
    }
}