using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralUtils
    {
        /// <summary>
        /// Nested rectangle wall panel: outer wood border, empty gap, inner fill with cut rows and blue middle.
        /// </summary>
        public static void DrawWallPanel(int rx, int ry, int w, int h, int woodWall, int blueWall)
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
                        WorldGen.PlaceWall(worldX, worldY, woodWall, true);
                    else if (isGap || isCutRow) { }
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
