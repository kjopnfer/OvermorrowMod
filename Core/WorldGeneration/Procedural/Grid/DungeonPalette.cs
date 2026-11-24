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

        /// <summary>
        /// A copy of this palette with <paramref name="walls"/> and <paramref name="objects"/> replacing the placements of
        /// the same colors.
        /// </summary>
        public DungeonPalette With(IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> walls, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> objects)
        {
            return new DungeonPalette(Merge(Walls, walls), Tiles, Merge(Objects, objects));
        }

        private static Dictionary<(byte R, byte G, byte B), TexPlaceFunction> Merge(IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> source, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> overrides)
        {
            var merged = new Dictionary<(byte R, byte G, byte B), TexPlaceFunction>(source);
            foreach (var (color, place) in overrides)
                merged[color] = place;
            return merged;
        }
    }
}
