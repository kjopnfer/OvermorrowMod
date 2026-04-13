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
                    for (int y = 0; y < leftFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + d, woodTile);
                }

                // Steps
                for (int i = 0; i < StepCount; i++)
                {
                    int stepY = leftFloorY + 1 + i;
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
                for (int y = bridgeCeiling; y < rightFloorY; y++)
                    WorldGenUtils.ClearTile(bridgeX, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(bridgeX, origin.Y + rightFloorY + d, woodTile);

                // Bottom landing (right side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int landingX = topLanding + StepCount + 1 + x;
                    int ceilingY = x < 3 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + d, woodTile);
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
                    int panelH = (p == 0) ? corridorH : corridorH + 4;
                    DrawColoredPanel(panelX, panelY, 5, panelH, 3);
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

                // Sconces on panels 2 and 7
                int sP2YOff = (2 - 1) * 4;
                int sconcePanel2X = origin.X + topLanding + 2 * 4 + 1;
                int sconcePanel2Y = origin.Y + sP2YOff + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sconcePanel2X, sconcePanel2Y, sconceType);

                int sP7YOff = (7 - 1) * 4;
                int sconcePanel7X = origin.X + topLanding + 7 * 4 + 1;
                int sconcePanel7Y = origin.Y + sP7YOff + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sconcePanel7X, sconcePanel7Y, sconceType);

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
                for (int x = 0; x < bottomLanding; x++)
                    WorldGenUtils.SetWall(origin.X + topLanding + StepCount + 1 + x, origin.Y + rightFloorY, woodWall);

                // Ceiling trim at top landing
                for (int x = 0; x < topLanding; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y - 1, woodWall);
            }
            else
            {
                // Ascending: mirror of descending
                // Bottom landing (left side)
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < leftFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + d, woodTile);
                }

                // Bridge tile (flat at bottom floor level)
                for (int d = 0; d < 4; d++)
                    WorldGenUtils.PlaceTile(origin.X + bottomLanding, origin.Y + leftFloorY + d, woodTile);

                // Steps (ascending)
                for (int i = 0; i < StepCount; i++)
                {
                    int stepY = leftFloorY - 1 - i;
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
                    for (int y = 0; y < rightFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + landingX, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + landingX, origin.Y + rightFloorY + d, woodTile);
                }

                // Wood panels behind ceiling
                for (int x = 0; x < bottomLanding; x++)
                {
                    int ceilingY = x >= bottomLanding - 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
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
                    int ceilingY = x >= bottomLanding - 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
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
                int corridorH = TopFloorY + 1;
                int[] panelYOffsets = new int[PanelCount];
                for (int p = 0; p < PanelCount; p++)
                    panelYOffsets[p] = (PanelCount - 1 - p) * 4;

                for (int p = 0; p < PanelCount; p++)
                {
                    int panelX = origin.X + bottomLanding + 1 + p * 4 - 3;
                    int panelY = origin.Y + panelYOffsets[p];
                    int panelH = (p == PanelCount - 1) ? corridorH : corridorH + 4;
                    DrawColoredPanel(panelX, panelY, 5, panelH, 3);
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

                // Sconces on panels 2 and 7
                int sP2X = origin.X + bottomLanding + 1 + 2 * 4 - 3 + 1;
                int sP2Y = origin.Y + panelYOffsets[2] + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sP2X, sP2Y, sconceType);

                int sP7X = origin.X + bottomLanding + 1 + 7 * 4 - 3 + 1;
                int sP7Y = origin.Y + panelYOffsets[7] + (corridorH + 4) - 4 - 5;
                WorldGen.PlaceObject(sP7X, sP7Y, sconceType);

                // Wood wall bands under steps (ascending = reversed offsets)
                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x, origin.Y + rightFloorY + y + 25, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 4, origin.Y + rightFloorY + y + 21, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 8, origin.Y + rightFloorY + y + 17, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 12, origin.Y + rightFloorY + y + 13, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 16, origin.Y + rightFloorY + y + 9, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 20, origin.Y + rightFloorY + y + 5, woodWall);
                        WorldGenUtils.SetWall(origin.X + bottomLanding + 1 + x + 24, origin.Y + rightFloorY + y + 1, woodWall);
                    }

                // Floor trim at bottom landing
                for (int x = 0; x < bottomLanding; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y + leftFloorY, woodWall);

                // Ceiling trim at top landing
                for (int x = 0; x < topLanding; x++)
                    WorldGenUtils.SetWall(origin.X + totalWidth - topLanding + x, origin.Y - 1, woodWall);
            }
        }
    }
}
