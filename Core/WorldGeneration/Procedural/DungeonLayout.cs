using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public class PlacedPiece
    {
        public IProceduralRoom Template { get; }
        public Point Origin { get; }

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
    }
}
