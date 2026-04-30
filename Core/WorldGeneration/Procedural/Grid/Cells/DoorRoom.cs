using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    /// <summary>
    /// The entrance/exit cell that holds a door linking this dungeon to
    /// another area (a second procedural section, a hand-crafted room, etc.).
    /// Both sides are horizontal connections: one side sits against the
    /// grid boundary (the portal side), the other faces the dungeon. The
    /// cell is agnostic about which is which; placement at an edge column
    /// makes it implicit via the walker's out-of-bounds rejection.
    /// </summary>
    public class DoorRoom : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly HashSet<Type> HorizontalAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(CorridorCell),
            typeof(StairBlock),
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            return side switch
            {
                Direction.Left => HorizontalAccepted,
                Direction.Right => HorizontalAccepted,
                _ => null
            };
        }

        private static readonly GridRoom[] HorizontalNeighborCells =
        {
            new BookshelfCell(),
            new CorridorCell(),
            new DescendingStair(),
            new AscendingStair(),
        };

        public override CellExit[] Exits => new[]
        {
            new CellExit(new Point( 1, 0), HorizontalNeighborCells),
            new CellExit(new Point(-1, 0), HorizontalNeighborCells),
        };

        /// <summary>
        /// Doors open horizontally only. Top and bottom are walls.
        /// </summary>
        public override bool IsOpenSide(int subCol, int subRow, Direction side) =>
            side == Direction.Left || side == Direction.Right;

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            // Tiles cleared so the slot is walkable. The door object itself
            // is not yet placed by this build pass.
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
        }
    }
}
