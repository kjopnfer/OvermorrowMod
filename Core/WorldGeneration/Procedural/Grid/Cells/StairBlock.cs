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
        /// Returns true if the given sub-cell contains the top landing
        /// (where the stair enters from the upper floor).
        /// </summary>
        public bool IsTopLandingSubCell(int subCol, int subRow)
        {
            if (subRow != 0) return false;
            return _descendLeftToRight ? subCol == 0 : subCol == 1;
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
                // Descending bottom landing is in sub-cell (1,1). Convert the block-relative
                // X of the last step to the sub-cell-local X, then extend leftward so the
                // wood run starts slightly before the final step.
                int blockX = TopLandingWidth + StepCount - 1;
                int localX = blockX - DungeonGrid.HorizontalSpacing;
                return (localX - extend, baseWidth + extend);
            }
            else
            {
                // Ascending bottom landing is in sub-cell (0,1) starting at local X=0.
                // Extend rightward past the landing so the wood run continues under the bridge/step.
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

        /// <summary>
        /// Descending: cursor sits at the anchor (top-left of 2x2).
        /// Ascending:  cursor sits at bottom-left, so the anchor is one row above.
        /// </summary>
        public override Point AnchorOffsetFromCursor =>
            _descendLeftToRight ? Point.Zero : new Point(0, -1);

        /// <summary>
        /// Stairs are inherently directional: one exit only.
        /// Descending: cursor moves (+2, +1) to the lower-right landing's east neighbor.
        /// Ascending:  cursor moves (+2, -1) to the upper-right landing's east neighbor.
        /// </summary>
        public override CellExit[] Exits => new[]
        {
            new CellExit(
                _descendLeftToRight ? new Point(2, 1) : new Point(2, -1),
                new GridRoom[] { new BookshelfCell(), new CorridorCell() })
        };

        private const int BaselineCeilingDepth = 4;

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
                // Tracks the current drop-group cycling state. Depths cycle 9,8,7,6 within
                // each contiguous run of columns sharing the same ceilingY > 0. Flat ceiling
                // sections (ceilingY == 0) use the baseline depth instead.
                int descPrevCeilingY = -1;
                int descPosInGroup = -1;

                int DescCeilingDepth(int ceilingY)
                {
                    if (ceilingY > 0)
                    {
                        if (ceilingY != descPrevCeilingY) descPosInGroup = 0;
                        else descPosInGroup++;
                        descPrevCeilingY = ceilingY;
                        return 9 - (descPosInGroup % 4);
                    }
                    descPrevCeilingY = ceilingY;
                    descPosInGroup = -1;
                    return BaselineCeilingDepth;
                }

                // Top landing (left side)
                for (int x = 0; x < topLanding; x++)
                {
                    for (int y = 0; y < leftFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + 1 + d, woodTile);
                    int topDepth = DescCeilingDepth(0);
                    for (int d = 0; d < topDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y - 1 - d, woodTile);
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
                    int stepDepth = DescCeilingDepth(ceilingY);
                    for (int d = 0; d < stepDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + topLanding + i, origin.Y + ceilingY - 1 - d, woodTile);
                }

                // Bridge tile (flat at bottom floor level, ceiling matches landing transition)
                int bridgeX = origin.X + topLanding + StepCount;
                int bridgeCeiling = ((StepCount - 1) / 4) * 4 - 4;
                for (int y = bridgeCeiling; y < rightFloorY + 1; y++)
                    WorldGenUtils.ClearTile(bridgeX, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(bridgeX, origin.Y + rightFloorY + 1 + d, woodTile);
                int bridgeDepth = DescCeilingDepth(bridgeCeiling);
                for (int d = 0; d < bridgeDepth; d++)
                    WorldGenUtils.PlaceTile(bridgeX, origin.Y + bridgeCeiling - 1 - d, woodTile);

                // Bottom landing (right side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int landingX = topLanding + StepCount + 1 + x;
                    int ceilingY = x < 3 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + 1 + d, woodTile);
                    int landDepth = DescCeilingDepth(ceilingY);
                    for (int d = 0; d < landDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + ceilingY - 1 - d, woodTile);
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
                // Ascending: mirror of descending. Depth pattern within each drop group is
                // reversed (6,7,8,9) so that the thickest wood ceiling tile aligns with the
                // right edge of each stair step instead of the left.
                // Precompute ceilingY for every world column in the block so depths can be
                // assigned right-to-left within each drop group. This avoids off-by-one issues
                // when iterating forward across landing/bridge/step transitions.
                int totalCols = totalWidth;
                int[] ascCeilingYs = new int[totalCols];
                int ascFlatCeilingY = ((StepCount - 1) / 4) * 4;

                for (int col = 0; col < totalCols; col++)
                {
                    if (col < bottomLanding)
                    {
                        int lx = col;
                        ascCeilingYs[col] = lx >= bottomLanding - 2
                            ? ((StepCount - 1) / 4) * 4 - 4
                            : ((StepCount - 1) / 4) * 4;
                    }
                    else if (col == bottomLanding)
                    {
                        ascCeilingYs[col] = ((StepCount - 1) / 4) * 4 - 4;
                    }
                    else if (col < bottomLanding + 1 + StepCount)
                    {
                        int stepIndex = col - (bottomLanding + 1);
                        int jj = StepCount - 1 - stepIndex;
                        ascCeilingYs[col] = (jj / 4) * 4 >= 4 ? ((jj / 4) * 4) - 4 : 0;
                    }
                    else
                    {
                        ascCeilingYs[col] = 0;
                    }
                }

                // Iterate right-to-left. Within each drop-group run, depths cycle 9, 8, 7, 6
                // starting from the rightmost column. The flat-top landing run cycles 4, 3, 2, 1
                // (same right-to-left taper) so the wood ceiling eases into the corner.
                int[] ascDepths = new int[totalCols];
                int ascRunCount = 0;
                int ascRunCeilingY = -1;
                for (int col = totalCols - 1; col >= 0; col--)
                {
                    int ceilingY = ascCeilingYs[col];
                    if (ceilingY <= 0)
                    {
                        ascDepths[col] = BaselineCeilingDepth;
                        ascRunCount = 0;
                        ascRunCeilingY = -1;
                        continue;
                    }
                    if (ceilingY != ascRunCeilingY)
                    {
                        ascRunCount = 0;
                        ascRunCeilingY = ceilingY;
                    }
                    if (ceilingY == ascFlatCeilingY)
                    {
                        // Flat-top run cycles 4, 3, 2, 1 right-to-left.
                        ascDepths[col] = 4 - (ascRunCount % 4);
                    }
                    else
                    {
                        ascDepths[col] = 9 - (ascRunCount % 4);
                    }
                    ascRunCount++;
                }

                int AscCeilingDepth(int col)
                {
                    if (col < 0 || col >= totalCols)
                        return BaselineCeilingDepth;
                    return ascDepths[col];
                }

                // Bottom landing (left side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 2 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < leftFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + 1 + d, woodTile);

                    // Hardcoded: leftmost 5 columns get depths 5, 6, 7, 8, 9 above ceilingY.
                    // Everything else uses the regular AscCeilingDepth cycling.
                    int ascLandDepth;
                    if (x == 0) ascLandDepth = 5;
                    else if (x == 1) ascLandDepth = 6;
                    else if (x == 2) ascLandDepth = 7;
                    else if (x == 3) ascLandDepth = 8;
                    else if (x == 4) ascLandDepth = 9;
                    else ascLandDepth = AscCeilingDepth(x);
                    for (int d = 0; d < ascLandDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + ceilingY - 1 - d, woodTile);
                }

                // Bridge tile (flat at bottom floor level)
                int ascBridgeCeiling = ((StepCount - 1) / 4) * 4 - 4;
                for (int y = ascBridgeCeiling; y < leftFloorY + 1; y++)
                    WorldGenUtils.ClearTile(origin.X + bottomLanding, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(origin.X + bottomLanding, origin.Y + leftFloorY + 1 + d, woodTile);
                int ascBridgeDepth = AscCeilingDepth(bottomLanding);
                for (int d = 0; d < ascBridgeDepth; d++)
                    WorldGenUtils.PlaceTile(origin.X + bottomLanding, origin.Y + ascBridgeCeiling - 1 - d, woodTile);

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
                    int ascStepDepth = AscCeilingDepth(bottomLanding + 1 + i);
                    for (int d = 0; d < ascStepDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + bottomLanding + 1 + i, origin.Y + ceilingY - 1 - d, woodTile);
                }

                // Top landing (right side)
                for (int x = 0; x < topLanding; x++)
                {
                    int landingX = bottomLanding + 1 + StepCount + x;
                    for (int y = 0; y < rightFloorY + 1; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + 1 + d, woodTile);
                    int ascTopDepth = AscCeilingDepth(landingX);
                    for (int d = 0; d < ascTopDepth; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y - 1 - d, woodTile);
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
                // Ascending offsets mirror the descending pattern (0,0,4,8,12,16,20,24).
                // The two panels at the top of the stairs share the same Y, matching the
                // shared-Y pair at the bottom of a descending stair.
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

                // Floor trim spans the bottom landing plus one additional tile on the right.
                for (int x = 0; x < bottomLanding + 1; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y + leftFloorY, woodWall);

                // Ceiling trim spans the top landing plus 7 additional tiles to the left.
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
