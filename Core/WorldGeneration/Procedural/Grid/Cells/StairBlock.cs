using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.WorldGeneration.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class StairBlock : GridRoom
    {
        private const int StepCount = 29;
        private const int TopLandingWidth = 7;
        private const int BottomLandingWidth = 7;
        private const int TopFloorY = 25;
        private const int BottomFloorY = 55;
        private const int PanelCount = 8;

        private readonly bool _descendLeftToRight;

        public override int CellWidth => 2;
        public override int CellHeight => 2;

        /// <summary>
        /// Returns true if the given sub-cell contains the bottom landing
        /// (where the floor continues off the final step).
        /// </summary>
        public bool IsBottomLandingSubCell(int subCol, int subRow)
        {
            if (subRow != 1) return false;
            return _descendLeftToRight ? subCol == 1 : subCol == 0;
        }

        /// <summary>
        /// For the bottom landing sub-cell, returns the sub-cell-relative X range
        /// where the stair floor touches the vertical padding below.
        /// Returns (startX, width) or (-1, 0) if this sub-cell has no floor continuation.
        /// The range covers the last step + bridge + bottom landing columns.
        /// </summary>
        public (int startX, int width) GetFloorPaddingRange(int subCol, int subRow)
        {
            if (!IsBottomLandingSubCell(subCol, subRow))
                return (-1, 0);

            int bottomLanding = _descendLeftToRight ? BottomLandingWidth : TopLandingWidth;
            int baseWidth = 1 + 1 + bottomLanding; // last step + bridge + bottom landing
            int extend = 2;

            if (_descendLeftToRight)
            {
                // Sub-cell (1,1) spans block X=26-43; last step at block X=topLanding+StepCount-1=35
                // Local X = 35 - 26 = 9
                int blockX = TopLandingWidth + StepCount - 1;
                int localX = blockX - DungeonGrid.HorizontalSpacing;
                // Extend left by 3
                return (localX - extend, baseWidth + extend);
            }
            else
            {
                // Sub-cell (0,1) spans block X=0-17; bottom landing starts at X=0
                // Extend right by 3
                return (0, baseWidth + extend);
            }
        }

        private static readonly HashSet<Type> FloorAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(CorridorCell)
        };

        public StairBlock(bool descendLeftToRight)
        {
            _descendLeftToRight = descendLeftToRight;
        }

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            if (IsInternalEdge(subCol, subRow, side))
                return null;

            if (_descendLeftToRight)
            {
                // Upper floor connects on the left of (0,0)
                if (subCol == 0 && subRow == 0 && side == Direction.Left)
                    return FloorAccepted;
                // Lower floor connects on the right of (1,1)
                if (subCol == 1 && subRow == 1 && side == Direction.Right)
                    return FloorAccepted;
            }
            else
            {
                // Lower floor connects on the left of (0,1)
                if (subCol == 0 && subRow == 1 && side == Direction.Left)
                    return FloorAccepted;
                // Upper floor connects on the right of (1,0)
                if (subCol == 1 && subRow == 0 && side == Direction.Right)
                    return FloorAccepted;
            }

            return null;
        }

        public override bool IsInternalEdge(int subCol, int subRow, Direction side)
        {
            // Right edge of left column is internal
            if (subCol == 0 && side == Direction.Right) return true;
            // Left edge of right column is internal
            if (subCol == 1 && side == Direction.Left) return true;
            // Bottom edge of top row is internal
            if (subRow == 0 && side == Direction.Bottom) return true;
            // Top edge of bottom row is internal
            if (subRow == 1 && side == Direction.Top) return true;

            return false;
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();
            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();
            int platformType = ModContent.TileType<CastlePlatform>();
            int sconceType = ModContent.TileType<WaxSconce>();

            int totalWidth = DungeonGrid.CellTileWidth * 2 + DungeonGrid.HorizontalPadding;
            int totalHeight = DungeonGrid.CellTileHeight * 2 + DungeonGrid.VerticalPadding;

            int leftFloorY = _descendLeftToRight ? TopFloorY : BottomFloorY;
            int rightFloorY = _descendLeftToRight ? BottomFloorY : TopFloorY;

            int topLanding = _descendLeftToRight ? TopLandingWidth : BottomLandingWidth;
            int bottomLanding = _descendLeftToRight ? BottomLandingWidth : TopLandingWidth;

            void DrawColoredPanel(int rx, int ry, int w, int h, int skipRowFromBottom = -1)
            {
                for (int lx = 0; lx < w; lx++)
                    for (int ly = 0; ly < h; ly++)
                    {
                        bool isBorder = (lx == 0 || lx == w - 1 || ly == 0 || ly == h - 1);
                        bool isSkipped = skipRowFromBottom > 0 && ly == h - 1 - skipRowFromBottom;
                        if (!isBorder && !isSkipped)
                            WorldGenUtils.SetWall(rx + lx, ry + ly, blueWall);
                    }
            }

            if (_descendLeftToRight)
            {
                // Top landing (left side)
                for (int x = 0; x < topLanding; x++)
                {
                    for (int y = 0; y < leftFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + 1 + d, woodTile);
                }

                // Steps
                for (int i = 0; i < StepCount; i++)
                {
                    int stepY = leftFloorY + 2 + i;
                    int ci = i > 0 ? i - 1 : 0;
                    int ceilingY = (ci / 4) * 4 >= 4 ? ((ci / 4) * 4) - 4 : 0;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearTile(origin.X + topLanding + i, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + topLanding + i, origin.Y + stepY + d, woodTile);
                }

                // Bridge tile (flat at bottom floor level, ceiling matches landing transition)
                int bridgeX = origin.X + topLanding + StepCount;
                int bridgeCeiling = ((StepCount - 1) / 4) * 4 - 4;
                for (int y = bridgeCeiling; y < rightFloorY + 1; y++)
                    WorldGenUtils.ClearTile(bridgeX, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(bridgeX, origin.Y + rightFloorY + 1 + d, woodTile);

                // Bottom landing (right side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int landingX = topLanding + StepCount + 1 + x;
                    int ceilingY = x < 3 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + 1 + d, woodTile);
                }

                // Wood panels behind ceiling
                for (int i = 0; i < StepCount; i++)
                {
                    int ci = i > 0 ? i - 1 : 0;
                    int ceilingY = (ci / 4) * 4 >= 4 ? ((ci / 4) * 4) - 4 : 0;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + topLanding + i, origin.Y + y, woodWall);
                }
                for (int x = 0; x < bottomLanding; x++)
                {
                    int landingX = topLanding + StepCount + 1 + x;
                    int ceilingY = x < 3 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + landingX, origin.Y + y, woodWall);
                }

                // Clear walls in open space
                for (int i = 0; i < StepCount; i++)
                {
                    int ci = i > 0 ? i - 1 : 0;
                    int ceilingY = (ci / 4) * 4 >= 4 ? ((ci / 4) * 4) - 4 : 0;
                    int stepY = leftFloorY + 1 + i;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearWall(origin.X + topLanding + i, origin.Y + y);
                }
                for (int x = 0; x < bottomLanding; x++)
                {
                    int landingX = topLanding + StepCount + 1 + x;
                    int ceilingY = x < 3 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY; y++)
                        WorldGenUtils.ClearWall(origin.X + landingX, origin.Y + y);
                }

                // Colored panels (8 panels, every 4 steps)
                // Y pattern: 0, 0, 4, 8, 12, 16, 20, 24 (first two share Y=0)
                int corridorH = TopFloorY + 1;
                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + topLanding + p * 4;
                    int panelYOff = p == 0 ? 0 : p == 1 ? 0 : (p - 1) * 4;
                    int panelY = origin.Y + panelYOff;
                    int panelH = (p == 0) ? corridorH : (p == PanelCount - 1) ? corridorH + 5 : corridorH + 4;
                    int skipRow = (p == PanelCount - 1) ? 4 : 3;
                    DrawColoredPanel(panelX, panelY, 5, panelH, skipRow);
                }

                // Platforms on gap row of each panel
                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + topLanding + p * 4;
                    int panelYOff = p == 0 ? 0 : p == 1 ? 0 : (p - 1) * 4;
                    int panelY = origin.Y + panelYOff;
                    int panelH = (p == 0) ? corridorH : corridorH + 4;
                    for (int lx = 0; lx < 5; lx++)
                        WorldGen.PlaceTile(panelX + lx, panelY + panelH - 4, platformType, true, true);
                }

                // Vases on each platform
                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + topLanding + p * 4;
                    int panelYOff = p == 0 ? 0 : p == 1 ? 0 : (p - 1) * 4;
                    int panelY = origin.Y + panelYOff;
                    int panelH = (p == 0) ? corridorH : corridorH + 4;
                    GrandArchiveRoom.PlaceVaseGroup(panelX, panelY + panelH - 5);
                }

                // Sconces on 2nd and 7th panels (0-indexed: 1 and 6)
                int sconceAX = origin.X + topLanding + 1 * 4 + 1;
                int sconceAYOff = -1;
                int sconceAY = origin.Y + sconceAYOff + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sconceAX, sconceAY, sconceType);

                int sconceBX = origin.X + topLanding + 6 * 4 + 1;
                int sconceBYOff = (6 - 1) * 4;
                int sconceBY = origin.Y + sconceBYOff + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sconceBX, sconceBY, sconceType);

                // Wood wall bands under steps
                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGenUtils.SetWall(origin.X + topLanding + x - 1, origin.Y + leftFloorY + y + 1, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 3, origin.Y + leftFloorY + y + 5, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 7, origin.Y + leftFloorY + y + 9, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 11, origin.Y + leftFloorY + y + 13, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 15, origin.Y + leftFloorY + y + 17, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 19, origin.Y + leftFloorY + y + 21, woodWall);
                        WorldGenUtils.SetWall(origin.X + topLanding + x + 23, origin.Y + leftFloorY + y + 25, woodWall);
                    }

                // Floor trim at bottom landing
                WorldGenUtils.SetWall(origin.X + topLanding + StepCount, origin.Y + rightFloorY, woodWall);
                for (int x = 0; x < bottomLanding; x++)
                    WorldGenUtils.SetWall(origin.X + topLanding + StepCount + 1 + x, origin.Y + rightFloorY, woodWall);

                // Ceiling trim at top landing and above first two panels
                for (int x = 0; x < topLanding + 9; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y - 1, woodWall);

                // Ceiling trim above last panel
                int lastPanelX = topLanding + (PanelCount - 1) * 4 + 1;
                int lastPanelCeilingY = (PanelCount - 2) * 4;
                for (int x = 0; x < 5; x++)
                {
                    WorldGenUtils.SetWall(origin.X + lastPanelX + x, origin.Y + lastPanelCeilingY - 1, woodWall);
                    WorldGenUtils.SetWall(origin.X + lastPanelX + x, origin.Y + lastPanelCeilingY - 2, woodWall);
                    WorldGenUtils.SetWall(origin.X + lastPanelX + x, origin.Y + lastPanelCeilingY - 3, woodWall);
                }

                // Wood panel on top landing (extends into ceiling, 1 gap before first colored panel)
                int topPanelW = topLanding;
                for (int x = 0; x < topPanelW; x++)
                    for (int y = 0; y <= leftFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);

                // Wood panel on bottom landing (extends into ceiling, 1 gap after last colored panel)
                int bottomPanelStart = topLanding + StepCount + 1 + 3;
                int bottomPanelW = totalWidth - bottomPanelStart;
                int bottomCeilingY = ((StepCount - 1) / 4) * 4;
                for (int x = 0; x < bottomPanelW; x++)
                    for (int y = bottomCeilingY; y <= rightFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + bottomPanelStart + x, origin.Y + y, woodWall);
            }
            else
            {
                // Ascending: mirror of descending
                // Bottom landing (left side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 2 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < leftFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + 1 + d, woodTile);
                }

                // Bridge tile (flat at bottom floor level)
                int ascBridgeCeiling = ((StepCount - 1) / 4) * 4 - 4;
                for (int y = ascBridgeCeiling; y < leftFloorY + 1; y++)
                    WorldGenUtils.ClearTile(origin.X + bottomLanding, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(origin.X + bottomLanding, origin.Y + leftFloorY + 1 + d, woodTile);

                // Steps (ascending)
                for (int i = 0; i < StepCount; i++)
                {
                    int stepY = leftFloorY - i;
                    int j = StepCount - 1 - i;
                    int ceilingY = (j / 4) * 4 >= 4 ? ((j / 4) * 4) - 4 : 0;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearTile(origin.X + bottomLanding + 1 + i, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + bottomLanding + 1 + i, origin.Y + stepY + d, woodTile);
                }

                // Top landing (right side)
                for (int x = 0; x < topLanding; x++)
                {
                    int landingX = bottomLanding + 1 + StepCount + x;
                    for (int y = 0; y < rightFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + 1 + d, woodTile);
                }

                // Wood panels behind ceiling
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 2 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);
                }
                for (int i = 0; i < StepCount; i++)
                {
                    int j = StepCount - 1 - i;
                    int ceilingY = (j / 4) * 4 >= 4 ? ((j / 4) * 4) - 4 : 0;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + i, origin.Y + y, woodWall);
                }

                // Clear walls in open space
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 2 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < leftFloorY; y++)
                        WorldGenUtils.ClearWall(origin.X + x, origin.Y + y);
                }
                for (int i = 0; i < StepCount; i++)
                {
                    int j = StepCount - 1 - i;
                    int ceilingY = (j / 4) * 4 >= 4 ? ((j / 4) * 4) - 4 : 0;
                    int stepY = leftFloorY - 1 - i;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearWall(origin.X + bottomLanding + 1 + i, origin.Y + y);
                }

                // Colored panels (8 panels, ascending = reversed Y progression)
                // Mirror of descending offsets: 0,0,4,8,12,16,20,24 -> 24,20,16,12,8,4,0,0
                int corridorH = TopFloorY + 1;
                int[] panelYOffsets = new int[PanelCount];
                for (int p = 0; p < PanelCount; p++)
                {
                    int rp = PanelCount - 1 - p;
                    panelYOffsets[p] = rp <= 1 ? 0 : (rp - 1) * 4;
                }

                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + bottomLanding + 1 + p * 4 - 3;
                    int panelY = origin.Y + panelYOffsets[p];
                    int panelH = (p == 0) ? corridorH + 5 : (p == PanelCount - 1) ? corridorH : corridorH + 4;
                    int skipRow = (p == 0) ? 4 : 3;
                    DrawColoredPanel(panelX, panelY, 5, panelH, skipRow);
                }

                // Platforms on gap row of each panel
                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + bottomLanding + 1 + p * 4 - 3;
                    int panelY = origin.Y + panelYOffsets[p];
                    int panelH = (p == PanelCount - 1) ? corridorH : corridorH + 4;
                    for (int lx = 0; lx < 5; lx++)
                        WorldGen.PlaceTile(panelX + lx, panelY + panelH - 4, platformType, true, true);
                }

                // Vases on each platform
                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + bottomLanding + 1 + p * 4 - 3;
                    int panelY = origin.Y + panelYOffsets[p];
                    int panelH = (p == PanelCount - 1) ? corridorH : corridorH + 4;
                    GrandArchiveRoom.PlaceVaseGroup(panelX, panelY + panelH - 5);
                }

                // Sconces on 2nd and 7th panels (0-indexed: 1 and 6)
                int sP1X = origin.X + bottomLanding + 1 + 1 * 4 - 3 + 1;
                int sP1Y = origin.Y + panelYOffsets[1] + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sP1X, sP1Y, sconceType);

                int sP6X = origin.X + bottomLanding + 1 + 6 * 4 - 3 + 1;
                int sP6Y = origin.Y + panelYOffsets[6] + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sP6X, sP6Y, sconceType);

                // Wood wall bands under steps (ascending = reversed offsets)
                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x, origin.Y + rightFloorY + y + 25, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 4, origin.Y + rightFloorY + y + 21, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 8, origin.Y + rightFloorY + y + 17, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 12, origin.Y + rightFloorY + y + 13, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 16, origin.Y + rightFloorY + y + 9, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 20, origin.Y + rightFloorY + y + 5, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 3 + x + 24, origin.Y + rightFloorY + y + 1, woodWall);
                    }

                // Floor trim at bottom landing (extend right by 1)
                for (int x = 0; x < bottomLanding + 1; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y + leftFloorY, woodWall);

                // Ceiling trim at top landing (extend 7 tiles left)
                for (int x = 0; x < topLanding + 7; x++)
                    WorldGenUtils.SetWall(origin.X + totalWidth - topLanding - 7 + x, origin.Y - 1, woodWall);

                // Wood panel on bottom landing (left, extends into ceiling)
                int bottomPanelCeilingY = ((StepCount - 1) / 4) * 4;
                for (int x = 0; x < bottomLanding - 2; x++)
                    for (int y = bottomPanelCeilingY; y <= leftFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);

                // Wood panel on top landing (right, extends into ceiling)
                int topPanelStart = bottomLanding + 1 + StepCount + 1;
                int topPanelW = totalWidth - topPanelStart;
                for (int x = 0; x < topPanelW; x++)
                    for (int y = 0; y <= rightFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + topPanelStart + x, origin.Y + y, woodWall);
            }
        }
    }
}
