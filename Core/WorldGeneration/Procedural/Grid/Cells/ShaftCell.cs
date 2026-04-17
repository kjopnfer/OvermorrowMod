using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Grid.Cells
{
    public class ShaftCell : GridRoom
    {
        public override int CellWidth => 1;
        public override int CellHeight => 1;

        private static readonly HashSet<Type> VerticalAccepted = new()
        {
            typeof(BookshelfCell),
            typeof(ShaftCell)
        };

        public override HashSet<Type> GetAcceptedNeighbors(int subCol, int subRow, Direction side)
        {
            return side switch
            {
                Direction.Top => VerticalAccepted,
                Direction.Bottom => VerticalAccepted,
                _ => null
            };
        }

        public override void Build(Point origin, int fillTileType, int liningTileType)
        {
            int width = DungeonGrid.CellTileWidth;
            int height = DungeonGrid.CellTileHeight;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);

            int woodWallId = ModContent.WallType<ArchiveWoodWall>();
            int blueWallId = ModContent.WallType<ArchiveWoodWallBlue>();
            ushort woodWall = (ushort)woodWallId;

            // Wall panels on left and right sides (1 tile wide each, leaves 16 tiles for panel)
            for (int y = 0; y < height; y++)
            {
                WorldGenUtils.SetWall(origin.X, origin.Y + y, woodWall);
                WorldGenUtils.SetWall(origin.X + width - 1, origin.Y + y, woodWall);
            }

            // Decorative wall panel filling the open shaft area between the side edge walls.
            DrawShaftWallPanel(origin.X + 1, origin.Y + 1, 16, 25, woodWall, (ushort)blueWallId);
        }

        private static void DrawShaftWallPanel(int rx, int ry, int w, int h, ushort woodWall, ushort blueWall)
        {
            int drawStartY = ry - 1;
            int drawEndY = ry + h;
            int drawHeight = drawEndY - drawStartY + 1;

            // Top row is an open gap (no outer wood border), matching the padding above.
            // Inner top-cut row separates top wood section from middle blue.
            int innerTopCutY = 5;
            int innerBottomCutY = drawHeight - 4;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < drawHeight; ly++)
                {
                    int worldX = rx + lx;
                    int worldY = drawStartY + ly;

                    // Top row is entirely empty (no walls)
                    if (ly == 0)
                        continue;

                    bool isOuterBorder = (lx == 0 || lx == w - 1 || ly == drawHeight - 1);
                    bool isGap = !isOuterBorder && (lx == 1 || lx == w - 2 || ly == drawHeight - 2);
                    bool isInner = (lx >= 2 && lx <= w - 3 && ly >= 1 && ly <= drawHeight - 3);
                    bool isCutRow = isInner && (ly == innerTopCutY || ly == innerBottomCutY);

                    if (isOuterBorder)
                        WorldGenUtils.SetWall(worldX, worldY, woodWall);
                    else if (isGap || isCutRow)
                        continue;
                    else if (isInner)
                    {
                        bool isMiddleSection = ly > innerTopCutY && ly < innerBottomCutY;
                        WorldGenUtils.SetWall(worldX, worldY, isMiddleSection ? blueWall : woodWall);
                    }
                }
            }
        }
    }
}
