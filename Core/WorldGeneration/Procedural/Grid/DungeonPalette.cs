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

        /// <summary>
        /// Object tile types this palette places in place of the keyed types, for objects a cell places by type.
        /// </summary>
        public IReadOnlyDictionary<int, int> ObjectTypes { get; }

        public DungeonPalette(IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> walls, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> tiles, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> objects, IReadOnlyDictionary<int, int> objectTypes = null)
        {
            Walls = walls;
            Tiles = tiles;
            Objects = objects;
            ObjectTypes = objectTypes ?? new Dictionary<int, int>();
        }

        public int ObjectType(int type) => ObjectTypes.TryGetValue(type, out int swapped) ? swapped : type;

        /// <summary>
        /// A copy of this palette with <paramref name="walls"/>, <paramref name="objects"/> and
        /// <paramref name="objectTypes"/> replacing the entries of the same keys.
        /// </summary>
        public DungeonPalette With(IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> walls, IReadOnlyDictionary<(byte R, byte G, byte B), TexPlaceFunction> objects, IReadOnlyDictionary<int, int> objectTypes)
        {
            return new DungeonPalette(Merge(Walls, walls), Tiles, Merge(Objects, objects), Merge(ObjectTypes, objectTypes));
        }

        private static Dictionary<TKey, TValue> Merge<TKey, TValue>(IReadOnlyDictionary<TKey, TValue> source, IReadOnlyDictionary<TKey, TValue> overrides)
        {
            var merged = new Dictionary<TKey, TValue>(source);
            foreach (var (color, place) in overrides)
                merged[color] = place;
            return merged;
        }
    }
}
