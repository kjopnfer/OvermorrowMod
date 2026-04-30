using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding
{
    /// <summary>
    /// Tells A* how expensive it is to place a given cell at a given anchor.
    /// Lower numbers are preferred; higher numbers get avoided.
    /// </summary>
    /// <remarks>
    /// Used to bias path shape: paths drift around expensive areas, prefer
    /// certain cell types, and stay out of zones marked as forbidden.
    /// </remarks>
    public delegate double EdgeCost(Point anchor, GridRoom candidate);

    /// <summary>
    /// Builders for the cost functions A* uses while planning a path.
    /// Pick one based on what kind of variation is desired.
    /// </summary>
    public static class PathfindingCost
    {
        /// <summary>
        /// Every cell costs the same. A* will pick the path with the fewest
        /// cells with no preference for one route over another. Useful as
        /// a baseline when shape variation is not wanted.
        /// </summary>
        public static EdgeCost Uniform()
        {
            return (anchor, candidate) =>
            {
                int cellCount = candidate.CellWidth * candidate.CellHeight;
                return cellCount;
            };
        }

        /// <summary>
        /// Cost is read from a pre-built per-cell map. A* routes through
        /// the cheap regions and avoids the expensive ones.
        /// </summary>
        /// <remarks>
        /// <paramref name="noise"/> is a 2D array sized to match the grid,
        /// with each entry holding the cost of that cell (typically between
        /// 1.0 and 3.0). Use <see cref="BuildSimplexNoiseField"/> to generate one.
        /// <para/>
        /// <paramref name="typeWeights"/> (optional) makes some cell types
        /// cheaper or more expensive overall (for example, stairs cost twice
        /// as much so A* uses them sparingly).
        /// </remarks>
        public static EdgeCost FromNoise(double[,] noise, IReadOnlyDictionary<Type, double> typeWeights = null)
        {
            int width = noise.GetLength(0);
            int height = noise.GetLength(1);

            return (anchor, candidate) =>
            {
                // Add up the cost for every cell the candidate covers
                // (a 2x2 stair sums 4 cells, a 1x1 bookshelf sums 1).
                double sum = 0.0;
                for (int sc = 0; sc < candidate.CellWidth; sc++)
                {
                    for (int sr = 0; sr < candidate.CellHeight; sr++)
                    {
                        int x = anchor.X + sc;
                        int y = anchor.Y + sr;
                        if (x < 0 || x >= width || y < 0 || y >= height)
                            return double.PositiveInfinity; // off the grid; never pick this
                        sum += noise[x, y];
                    }
                }

                if (typeWeights != null && typeWeights.TryGetValue(candidate.GetType(), out double w))
                    sum *= w;

                return sum;
            };
        }

        /// <summary>
        /// Generates a "cost map" for the grid using OpenSimplex noise: a
        /// smooth random pattern of cheap and expensive regions that look
        /// natural rather than chaotic. A* flows through the cheap regions
        /// and curves around the expensive ones.
        /// </summary>
        /// <remarks>
        /// <paramref name="frequency"/> controls how big each cheap/expensive
        /// region is. Smaller numbers = bigger blobs. 0.08 gives features
        /// roughly 10-15 cells wide; 0.2 gives smaller, busier variation.
        /// <para/>
        /// <paramref name="minCost"/> / <paramref name="maxCost"/> set how
        /// strong the variation is. Cells end up between those two values;
        /// a wider range makes A* avoid expensive zones more aggressively.
        /// </remarks>
        public static double[,] BuildSimplexNoiseField(int cols, int rows, int seed,
                                                       float frequency = 0.08f,
                                                       double minCost = 1.0,
                                                       double maxCost = 3.0)
        {
            var fn = new FastNoiseLite(seed);
            fn.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            fn.SetFrequency(frequency);

            double[,] field = new double[cols, rows];
            double range = maxCost - minCost;
            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    // FastNoise gives -1..1; remap to the cost range.
                    float n = fn.GetNoise(c, r);
                    double t = (n + 1.0) * 0.5;     // 0..1
                    field[c, r] = minCost + t * range;
                }
            }
            return field;
        }
    }
}
