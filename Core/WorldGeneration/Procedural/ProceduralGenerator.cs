using System;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralGenerator
    {
        private const int MinPanelWidth = 6;
        private const int MaxPanelWidth = 10;

        public static void PlaceWallPanels(int startX, int startY, int totalWidth, int totalHeight, int woodWall, int blueWall)
        {
            var rand = new Random(Environment.TickCount + 999);
            DivideIntoRectangles(startX, startY, totalWidth, totalHeight, woodWall, blueWall, rand);
        }

        private static void DivideIntoRectangles(int startX, int startY, int airWidth, int airHeight, int woodWall, int blueWall, Random rand)
        {
            int panelCount = Math.Max(1, airWidth / MaxPanelWidth);
            int baseWidth = airWidth / panelCount;
            int remainder = airWidth % panelCount;

            int cursorX = startX;
            for (int i = 0; i < panelCount; i++)
            {
                int pw = baseWidth;
                if (i >= panelCount / 2 - remainder / 2 && i < panelCount / 2 - remainder / 2 + remainder)
                    pw++;

                DrawRectangleBorder(cursorX, startY, pw, airHeight, woodWall, blueWall);
                cursorX += pw;
            }
        }

        private static void DrawRectangleBorder(int rx, int ry, int w, int h, int woodWall, int blueWall)
        {
            int drawStartY = ry - 1;
            int drawEndY = ry + h;
            int drawHeight = drawEndY - drawStartY + 1;

            int innerTopCutY = 6;
            int innerBottomCutY = drawHeight - 4;

            for (int lx = 0; lx < w; lx++)
            {
                for (int ly = 0; ly < drawHeight; ly++)
                {
                    int worldX = rx + lx;
                    int worldY = drawStartY + ly;

                    bool isOuterBorder = (lx == 0 || lx == w - 1 || ly == 0 || ly == drawHeight - 1);
                    bool isGap = !isOuterBorder && (lx == 1 || lx == w - 2 || ly == 1 || ly == drawHeight - 2);
                    bool isInner = (lx >= 2 && lx <= w - 3 && ly >= 2 && ly <= drawHeight - 3);
                    bool isCutRow = isInner && (ly == innerTopCutY || ly == innerBottomCutY);

                    if (isOuterBorder)
                    {
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                    }
                    else if (isGap || isCutRow)
                    {
                        // Empty
                    }
                    else if (isInner)
                    {
                        bool isMiddleSection = ly > innerTopCutY && ly < innerBottomCutY;
                        WorldGen.PlaceWall(worldX, worldY, isMiddleSection ? blueWall : woodWall, true);
                    }
                }
            }
        }
    }
}
