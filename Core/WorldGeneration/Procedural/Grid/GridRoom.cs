using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    public enum Direction
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// Base class for all grid-based room pieces. Each GridRoom occupies one or more
    /// cells in the dungeon grid (e.g. 1x1 for a bookshelf, 2x2 for stairs).
    ///
    /// Instead of sockets, each edge declares which GridRoom types it accepts as neighbors.
    /// The grid checks mutual compatibility: A accepts B on that edge AND B accepts A.
    /// </summary>
    public abstract class GridRoom
    {
        public abstract int CellWidth { get; }
        public abstract int CellHeight { get; }

        /// <summary>
        /// Returns the set of GridRoom types accepted on the given edge of a sub-cell.
        /// For 1x1 rooms, subCol and subRow are always 0.
        /// For multi-cell rooms (e.g. 2x2 stairs), they identify which sub-cell.
        /// Return null or empty to reject all neighbors on that edge.
        /// </summary>
        public abstract HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side);

        /// <summary>
        /// Returns true if the given edge is internal to a multi-cell piece.
        /// Internal edges only match other internal edges of the same piece instance.
        /// For 1x1 rooms this always returns false.
        /// </summary>
        public virtual bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            return false;
        }

        /// <summary>
        /// Build this piece at the given world-space top-left corner.
        /// For multi-cell pieces, this is the top-left of the entire block
        /// (including internal padding between sub-cells).
        /// </summary>
        public abstract void Build(Point origin, int fillTileType, int liningTileType);
    }
}
