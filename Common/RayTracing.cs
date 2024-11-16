using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common
{
    public static class RayTracking
    {
        /// <summary>
        /// Casts a ray in a specified direction from a start position for a given length and checks for tile collisions along the way.
        /// Stops when a tile collision is detected.
        /// </summary>
        /// <param name="start">The starting point of the ray (origin).</param>
        /// <param name="direction">The direction in which the ray will be cast. This is normalized to a unit vector.</param>
        /// <param name="length">The maximum length of the ray to be cast.</param>
        /// <returns>A <see cref="Vector2"/> representing the final position where the ray ended, either after the given length or when a tile collision is detected.</returns>
        /// <remarks>
        /// This method iterates along the ray from the start position in the given direction, moving one unit at a time.
        /// It checks for tile collisions using <see cref="Collision.CanHitLine"/> between the current position and the next.
        /// If a tile collision is detected, the raycasting stops, and the final position is returned.
        /// This method does NOT detect entity collisions; it only checks tile collisions.
        /// </remarks>
        public static Vector2 CastTileCollision(Vector2 start, Vector2 direction, float length)
        {
            // Normalize the direction vector to ensure it has a length of 1 (unit vector)
            direction = direction.SafeNormalize(Vector2.UnitY);
            Vector2 output = start;

            // Iterate along the ray, checking for tile collisions at each step
            for (int i = 0; i < length; i++)
            {
                // Check if a tile collision occurs between the current position and the next position
                if (Collision.CanHitLine(output, 0, 0, output + direction, 0, 0))
                {
                    // If no collision, move one step in the direction of the ray
                    output += direction;
                }
                else
                {
                    // Break the loop if a tile collision is detected
                    break;
                }
            }

            // Return the final position of the ray after the cast
            return output;
        }

        /// <summary>
        /// Casts a ray in a specified direction from a start position for a given length and returns the actual distance traveled by the ray.
        /// This method accounts for both the length of the ray and the potential tile collisions along the path.
        /// </summary>
        /// <param name="start">The starting point of the ray (origin).</param>
        /// <param name="direction">The direction in which the ray will be cast. This is normalized to a unit vector.</param>
        /// <param name="length">The maximum length of the ray to be cast.</param>
        /// <returns>The actual distance traveled by the ray as a <see cref="float"/>, which may be less than the maximum length if a tile collision occurs.</returns>
        /// <remarks>
        /// This method calls <see cref="CastTileCollision"/> to find the final position of the ray and then calculates the distance 
        /// between the start and the final position. It will return the length of the ray, which may be shorter than the 
        /// maximum length if a tile collision stops the raycasting.
        /// This method checks for tile collisions only, not entity collisions.
        /// </remarks>
        public static float CastTileCollisionLength(Vector2 start, Vector2 direction, float length)
        {
            // Get the final position of the ray after casting
            Vector2 end = CastTileCollision(start, direction, length);

            // Return the distance between the start and final position
            return (end - start).Length();
        }
    }
}