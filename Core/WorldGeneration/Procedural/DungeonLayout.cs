using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public class PlacedPiece
    {
        public IProceduralRoom Template { get; }
        public Point Origin { get; }
        public Rectangle Bounds => new Rectangle(Origin.X, Origin.Y, Template.Width, Template.Height);

        public PlacedPiece Left { get; set; }
        public PlacedPiece Right { get; set; }
        public PlacedPiece Top { get; set; }
        public PlacedPiece Bottom { get; set; }

        public PlacedPiece(IProceduralRoom template, Point origin)
        {
            Template = template;
            Origin = origin;
        }
    }

    public class DungeonLayout
    {
        public List<PlacedPiece> AllPieces { get; } = new List<PlacedPiece>();

        /// <summary>
        /// All vertical shaft connectors in the layout, used by the decoration pass.
        /// </summary>
        public List<PlacedPiece> Shafts { get; } = new List<PlacedPiece>();

        public void Add(PlacedPiece piece)
        {
            AllPieces.Add(piece);
            if (piece.Template is VerticalStairs)
                Shafts.Add(piece);
        }

        /// <summary>
        /// Returns true if the given rectangle does not overlap any already-placed piece
        /// and lies entirely within world bounds.
        /// Touching at edges is allowed — only actual overlap is rejected.
        /// </summary>
        public bool CanPlace(Point origin, int width, int height)
        {
            if (origin.X < 0 || origin.Y < 0 ||
                origin.X + width > Main.maxTilesX ||
                origin.Y + height > Main.maxTilesY)
                return false;

            var newBounds = new Rectangle(origin.X, origin.Y, width, height);
            foreach (var piece in AllPieces)
            {
                if (newBounds.Intersects(piece.Bounds))
                    return false;
            }
            return true;
        }
    }
}
