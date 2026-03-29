using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates
{
    public interface IRoomTemplate
    {
        int Width { get; }
        int Height { get; }

        /// <summary>
        /// Generate a sealed room at the given position with sockets already defined.
        /// </summary>
        ProceduralRoom Generate(Point position, int fillTileType, int liningTileType);

        /// <summary>
        /// Compute room origin so its input socket aligns with the given anchor.
        /// Uses the template's known socket layout (no room instance needed).
        /// </summary>
        Point AlignTo(SocketAnchor anchor);
    }
}
