using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.ModularGeneration
{
    /// <summary>
    /// This class represents a tunnel in a 2D space. It contains two lists of vectors that define the tunnel path.
    /// </summary>
    public static class Tunnel
    {
        // The list of vectors in the first tunnel
        public static List<Vector2> Tunnel1 { get; set; }

        // The list of vectors in the second tunnel
        public static List<Vector2> Tunnel2 { get; set; }

        // Constructor that receives the two vectors and calculates the tunnel vectors
        /// <summary>
        /// Calculates the tunnel vectors given the start and end points and the offset distance.
        /// </summary>
        /// <param name="vec1">The start point of the tunnel</param>
        /// <param name="vec2">The end point of the tunnel</param>
        /// <param name="offset">The distance to offset the tunnel from the line between the start and end points</param>
        public static void Calculate(Vector2 vec1, Vector2 vec2, float offset)
        {
            // Calculate the direction vector
            var dir = vec2 - vec1;

            // Normalize the direction vector
            dir = Vector2.Normalize(dir);

            // Calculate the distance between the two vectors
            var distance = Vector2.Distance(vec1, vec2);

            // Create a list of vectors for the first tunnel
            Tunnel1 = new List<Vector2>();

            // Create a list of vectors for the second tunnel
            Tunnel2 = new List<Vector2>();

            // Calculate the tunnel vectors
            for (int i = 0; i < distance; i++)
            {
                var vec = vec1 + dir * i;

                // Add half the offset to the Y component of the vector
                vec.Y -= offset / 2;

                Tunnel1.Add(vec);

                // Add the full offset to restore original vector, and then offset it by desired degree
                vec.Y += offset;

                Tunnel2.Add(vec);
            }
        }

        /// <summary>
        /// Function that calls WorldGen.PlaceTile on every vector in the Tunnel1 and Tunnel2 lists.
        /// </summary>
        /// <param name="tileType">The type of tile to place.</param>
        public static void PlaceTiles(int tileType)
        {
            foreach (Vector2 vec in Tunnel1)
            {
                ModularRoomGenerator.PlaceTile(vec, tileType);
            }

            foreach (Vector2 vec in Tunnel2)
            {
                ModularRoomGenerator.PlaceTile(vec, tileType);
            }
        }

    }

}


