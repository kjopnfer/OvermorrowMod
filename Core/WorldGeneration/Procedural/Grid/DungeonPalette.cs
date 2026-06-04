using OvermorrowMod.Common.TextureMapping;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Resolves the authored aseprite colors a cell paints with into the tiles,
    /// walls, and objects of one dungeon. Cells supply layout through their art;
    /// the palette supplies the materials and themeable decor that art resolves
    /// to, so the same cell renders differently under a different dungeon.
    /// </summary>
    public class DungeonPalette
    {
        /// <summary>
        /// Color to wall-layer placement.
        /// </summary>
        public IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> Walls { get; }

        /// <summary>
        /// Color to tile-layer placement.
        /// </summary>
        public IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> Tiles { get; }

        /// <summary>
        /// Color to object-layer placement for decor shared across the dungeon's cells.
        /// </summary>
        public IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> Objects { get; }

        public DungeonPalette(IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> walls, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> tiles, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> objects)
        {
            Walls = walls;
            Tiles = tiles;
            Objects = objects;
        }
    }
}
