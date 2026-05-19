using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates
{
    /// <summary>
    /// A rectangular piece with dimensions, sockets, and a build method.
    /// Rooms, corridors, and shafts all implement this.
    /// </summary>
    public interface IProceduralRoom
    {
        int Width { get; }
        int Height { get; }

        EdgeSocket Left { get; }
        EdgeSocket Right { get; }
        EdgeSocket Top { get; }
        EdgeSocket Bottom { get; }

        /// <summary>
        /// Build this piece at the given origin (top-left corner).
        /// </summary>
        void Build(Point origin, int fillTileType, int liningTileType);
    }
}
