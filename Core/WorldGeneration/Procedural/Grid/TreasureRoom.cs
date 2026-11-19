using Microsoft.Xna.Framework;
using OvermorrowMod.Common.TextureMapping;
using System;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// A hand-made room painted outside every dungeon, reached only through the treasure door it is paired with.
    /// Its sheet carries its own stone shell and has no spawns.
    /// </summary>
    public abstract class TreasureRoom
    {
        protected abstract string AsepritePath { get; }

        public (int Width, int Height) Size => TexGen.GetCanvasSize(AsepritePath);

        /// <summary>
        /// Paints the room with its top-left corner at <paramref name="origin"/>.
        /// </summary>
        /// <returns>
        /// The exit door, or null if the sheet has none.
        /// </returns>
        public IDungeonDoor Generate(Point origin, DungeonPalette palette, Random rand)
        {
            TexGen.PaintClearLayer(AsepritePath, origin.X, origin.Y);
            TexGen.PaintAsepriteLayer(SheetLayer.Walls, AsepritePath, origin.X, origin.Y, palette.Walls);
            TexGen.PaintAsepriteLayer(SheetLayer.Tiles, AsepritePath, origin.X, origin.Y, palette.Tiles);
            TexGen.PaintAsepriteLayer(SheetLayer.Objects, AsepritePath, origin.X, origin.Y, palette.Objects);
            return PlaceFeatures(origin, rand);
        }

        /// <summary>
        /// Places the exit door and the reward from the markers on the Objects layer.
        /// </summary>
        /// <returns>
        /// The exit door, or null if the sheet has none.
        /// </returns>
        protected abstract IDungeonDoor PlaceFeatures(Point origin, Random rand);
    }
}
