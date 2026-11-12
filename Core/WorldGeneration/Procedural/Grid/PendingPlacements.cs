using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Pathfinding;
using System;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid
{
    /// <summary>
    /// Rooms a pathfinding search has placed on its current path but not yet committed to the grid.
    /// </summary>
    public sealed class PendingPlacements
    {
        private readonly int _cols;
        private readonly int _rows;
        private readonly int[] _stamp;
        private readonly GridRoom[] _cells;
        private readonly Point[] _anchors;
        private readonly List<PathStep> _steps = new();
        private int _current;

        public PendingPlacements(int cols, int rows)
        {
            _cols = cols;
            _rows = rows;
            _stamp = new int[cols * rows];
            _cells = new GridRoom[cols * rows];
            _anchors = new Point[cols * rows];
        }

        public IReadOnlyList<PathStep> Steps => _steps;

        public void StartPath()
        {
            _current++;
            _steps.Clear();
        }

        public void Add(GridRoom cell, Point anchor)
        {
            _steps.Add(new PathStep(cell, anchor));
            for (int sc = 0; sc < cell.CellWidth; sc++)
            {
                for (int sr = 0; sr < cell.CellHeight; sr++)
                {
                    int i = (anchor.X + sc) * _rows + anchor.Y + sr;
                    _stamp[i] = _current;
                    _cells[i] = cell;
                    _anchors[i] = anchor;
                }
            }
        }

        public bool Contains(int col, int row) => col >= 0 && col < _cols && row >= 0 && row < _rows && _stamp[col * _rows + row] == _current;

        public GridRoom CellAt(int col, int row) => Contains(col, row) ? _cells[col * _rows + row] : null;

        public bool TryGet(int col, int row, out GridRoom cell, out Point anchor)
        {
            if (!Contains(col, row))
            {
                cell = null;
                anchor = default;
                return false;
            }
            int i = col * _rows + row;
            cell = _cells[i];
            anchor = _anchors[i];
            return true;
        }

        /// <summary>
        /// Number of rooms of <paramref name="roomType"/> or a subclass on the current path.
        /// </summary>
        public int Count(Type roomType)
        {
            int count = 0;
            foreach (var step in _steps)
                if (roomType.IsInstanceOfType(step.Cell)) count++;
            return count;
        }
    }
}
