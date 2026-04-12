using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Content.WorldGeneration.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public abstract class Stairs : IProceduralRoom
    {
        protected const int LandingWidth = 8;
        protected const int TopLandingWidth = 4;
        protected const int StepCount = 17;
        protected const int CorridorHeight = 26;

        public int Width => TopLandingWidth + StepCount + LandingWidth;
        public int Height => CorridorHeight + StepCount + 4;

        public abstract EdgeSocket Left { get; }
        public abstract EdgeSocket Right { get; }
        public EdgeSocket Top => null;
        public EdgeSocket Bottom => null;

        protected readonly bool _descendLeftToRight;

        protected Stairs(bool descendLeftToRight)
        {
            _descendLeftToRight = descendLeftToRight;
        }

        public void Build(Point origin, int fillTileType, int liningTileType)
        {
            ushort woodTile = (ushort)ModContent.TileType<ArchiveWood>();

            int leftFloorY = _descendLeftToRight ? CorridorHeight : CorridorHeight + StepCount;
            int rightFloorY = _descendLeftToRight ? CorridorHeight + StepCount : CorridorHeight;

            ushort woodWall = (ushort)ModContent.WallType<ArchiveWoodWall>();
            ushort blueWall = (ushort)ModContent.WallType<ArchiveWoodWallBlue>();
            int platformType = ModContent.TileType<CastlePlatform>();
            int sconceType = ModContent.TileType<WaxSconce>();

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
                // Left landing (top, 4 tiles)
                for (int x = 0; x < TopLandingWidth; x++)
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
                        WorldGenUtils.ClearTile(origin.X + TopLandingWidth + i, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + TopLandingWidth + i, origin.Y + stepY + d, woodTile);
                }

                // Right landing (bottom, 8 tiles)
                for (int x = 0; x < LandingWidth; x++)
                {
                    int ceilingY = x < 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + TopLandingWidth + StepCount + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + TopLandingWidth + StepCount + x, origin.Y + rightFloorY + d, woodTile);
                }

                // Wood panels behind ceiling (solid area y=0 to ceilingY)
                for (int i = 0; i < StepCount; i++)
                {
                    int ci = i > 0 ? i - 1 : 0;
                    int ceilingY = (ci / 4) * 4 >= 4 ? ((ci / 4) * 4) - 4 : 0;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + i, origin.Y + y, woodWall);
                }
                for (int x = 0; x < LandingWidth; x++)
                {
                    int ceilingY = x < 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + StepCount + x, origin.Y + y, woodWall);
                }

                // Clear walls in the open space so other walls can be placed
                for (int i = 0; i < StepCount; i++)
                {
                    int ci = i > 0 ? i - 1 : 0;
                    int ceilingY = (ci / 4) * 4 >= 4 ? ((ci / 4) * 4) - 4 : 0;
                    int stepY = leftFloorY + 1 + i;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearWall(origin.X + TopLandingWidth + i, origin.Y + y);
                }
                for (int x = 0; x < LandingWidth; x++)
                {
                    int ceilingY = x < 4 ? ((StepCount - 1) / 4) * 4 - 4 : ((StepCount - 1) / 4) * 4;
                    for (int y = ceilingY; y < rightFloorY; y++)
                        WorldGenUtils.ClearWall(origin.X + TopLandingWidth + StepCount + x, origin.Y + y);
                }

                DrawColoredPanel(origin.X + TopLandingWidth, origin.Y + 0, 5, CorridorHeight + 1, 3);
                DrawColoredPanel(origin.X + TopLandingWidth + 4, origin.Y + 0, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + TopLandingWidth + 8, origin.Y + 4, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + TopLandingWidth + 12, origin.Y + 8, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + TopLandingWidth + 16, origin.Y + 12, 5, CorridorHeight + 5, 3);

                // Platforms on the gap row in each panel
                for (int lx = 0; lx < 5; lx++)
                {
                    WorldGen.PlaceTile(origin.X + TopLandingWidth + lx, origin.Y + 0 + (CorridorHeight + 1) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + TopLandingWidth + 4 + lx, origin.Y + 0 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + TopLandingWidth + 8 + lx, origin.Y + 4 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + TopLandingWidth + 12 + lx, origin.Y + 8 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + TopLandingWidth + 16 + lx, origin.Y + 12 + (CorridorHeight + 5) - 4, platformType, true, true);
                }

                // Vases on top of each platform
                GrandArchiveRoom.PlaceVaseGroup(origin.X + TopLandingWidth, origin.Y + 0 + (CorridorHeight + 1) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + TopLandingWidth + 4, origin.Y + 0 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + TopLandingWidth + 8, origin.Y + 4 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + TopLandingWidth + 12, origin.Y + 8 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + TopLandingWidth + 16, origin.Y + 12 + (CorridorHeight + 5) - 5);

                // Wax sconce on the middle (3rd) panel, 5 tiles above the platform
                WorldGen.PlaceObject(origin.X + TopLandingWidth + 8 + 1, origin.Y + 4 + (CorridorHeight + 5) - 4 - 5, sconceType);

                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + x - 1, origin.Y + leftFloorY + y + 1, woodWall);
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + x + 3, origin.Y + leftFloorY + y + 5, woodWall);
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + x + 7, origin.Y + leftFloorY + y + 9, woodWall);
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + x + 11, origin.Y + leftFloorY + y + 13, woodWall);
                    }

                // Lower Floor trim
                for (int x = 0; x < 8; x++)
                    WorldGenUtils.SetWall(origin.X + TopLandingWidth + 17 + x, origin.Y + leftFloorY + 17, woodWall);

                // Ceiling trim
                for (int x = 0; x < 16; x++)
                    WorldGenUtils.SetWall(origin.X + x + 2, origin.Y - 1, woodWall);

                // Wood Panel before Stairs
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y <= CorridorHeight; y++)
                        WorldGenUtils.SetWall(origin.X + 1 + x, origin.Y + y, woodWall);

                // Right landing wood panel (ceiling to floor, after last colored panel)
                for (int x = 0; x < 4; x++)
                    for (int y = 12; y <= rightFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + TopLandingWidth + 21 + x, origin.Y + y, woodWall);
            }
            else
            {
                // Left landing
                for (int x = 0; x < LandingWidth + 1; x++)
                {
                    int ceilingY = x < 4 ? 16 : x < 8 ? 12 : 8;
                    for (int y = ceilingY; y < leftFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + x, origin.Y + leftFloorY + d, woodTile);
                }

                // Steps (ascending)
                for (int i = 0; i < StepCount; i++)
                {
                    int stepY = leftFloorY - 1 - i;
                    int gx = LandingWidth + 1 + i;
                    int ceilingY = (gx / 4) * 4 <= 16 ? 16 - (gx / 4) * 4 : 0;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearTile(origin.X + LandingWidth + 1 + i, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + LandingWidth + 1 + i, origin.Y + stepY + d, woodTile);
                }

                // Wood panels behind ceiling (solid area y=0 to ceilingY)
                for (int x = 0; x < LandingWidth + 1; x++)
                {
                    int ceilingY = x < 4 ? 16 : x < 8 ? 12 : 8;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);
                }
                for (int i = 0; i < StepCount; i++)
                {
                    int gx = LandingWidth + 1 + i;
                    int ceilingY = (gx / 4) * 4 <= 16 ? 16 - (gx / 4) * 4 : 0;
                    for (int y = 0; y < ceilingY; y++)
                        WorldGenUtils.SetWall(origin.X + LandingWidth + 1 + i, origin.Y + y, woodWall);
                }

                // Clear walls in the open space so other walls can be placed
                for (int x = 0; x < LandingWidth + 1; x++)
                {
                    int ceilingY = x < 4 ? 16 : x < 8 ? 12 : 8;
                    for (int y = ceilingY; y < leftFloorY; y++)
                        WorldGenUtils.ClearWall(origin.X + x, origin.Y + y);
                }
                for (int i = 0; i < StepCount; i++)
                {
                    int gx = LandingWidth + 1 + i;
                    int ceilingY = (gx / 4) * 4 <= 16 ? 16 - (gx / 4) * 4 : 0;
                    int stepY = leftFloorY - 1 - i;
                    for (int y = ceilingY; y < stepY; y++)
                        WorldGenUtils.ClearWall(origin.X + LandingWidth + 1 + i, origin.Y + y);
                }

                // Wood Panel before Stairs
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y <= CorridorHeight; y++)
                        WorldGenUtils.SetWall(origin.X + Width - 4 + x, origin.Y + y, woodWall);

                // Right landing (top, 3 tiles + 1 flush step = 4 visually)
                for (int x = 0; x < TopLandingWidth - 1; x++)
                {
                    for (int y = 0; y < rightFloorY; y++)
                        WorldGenUtils.ClearTile(origin.X + LandingWidth + 1 + StepCount + x, origin.Y + y);
                    for (int d = 0; d < 4; d++)
                        WorldGenUtils.PlaceTile(origin.X + LandingWidth + 1 + StepCount + x, origin.Y + rightFloorY + d, woodTile);
                }

                DrawColoredPanel(origin.X + LandingWidth - 4, origin.Y + 12, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + LandingWidth + 0, origin.Y + 8, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + LandingWidth + 4, origin.Y + 4, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + LandingWidth + 8, origin.Y + 0, 5, CorridorHeight + 5, 3);
                DrawColoredPanel(origin.X + LandingWidth + 12, origin.Y + 0, 5, CorridorHeight + 1, 3);

                // Platforms on the gap row in each panel (5 wide to span both panel borders)
                for (int lx = 0; lx < 5; lx++)
                {
                    WorldGen.PlaceTile(origin.X + LandingWidth - 4 + lx, origin.Y + 12 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + LandingWidth + 0 + lx, origin.Y + 8 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + LandingWidth + 4 + lx, origin.Y + 4 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + LandingWidth + 8 + lx, origin.Y + 0 + (CorridorHeight + 5) - 4, platformType, true, true);
                    WorldGen.PlaceTile(origin.X + LandingWidth + 12 + lx, origin.Y + 0 + (CorridorHeight + 1) - 4, platformType, true, true);
                }

                // Vases on top of each platform
                GrandArchiveRoom.PlaceVaseGroup(origin.X + LandingWidth - 4, origin.Y + 12 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + LandingWidth + 0, origin.Y + 8 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + LandingWidth + 4, origin.Y + 4 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + LandingWidth + 8, origin.Y + 0 + (CorridorHeight + 5) - 5);
                GrandArchiveRoom.PlaceVaseGroup(origin.X + LandingWidth + 12, origin.Y + 0 + (CorridorHeight + 1) - 5);

                // Wax sconce on the middle (3rd) panel, 5 tiles above the platform
                WorldGen.PlaceObject(origin.X + LandingWidth + 4 + 1, origin.Y + 4 + (CorridorHeight + 5) - 4 - 5, sconceType);

                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGenUtils.SetWall(origin.X + LandingWidth + 1 + x, origin.Y + rightFloorY + y + 13, woodWall);
                        WorldGenUtils.SetWall(origin.X + LandingWidth + 1 + x + 4, origin.Y + rightFloorY + y + 9, woodWall);
                        WorldGenUtils.SetWall(origin.X + LandingWidth + 1 + x + 8, origin.Y + rightFloorY + y + 5, woodWall);
                        WorldGenUtils.SetWall(origin.X + LandingWidth + 1 + x + 12, origin.Y + rightFloorY + y + 1, woodWall);
                    }

                // Left landing wood panel (ceiling to floor)
                for (int x = 0; x < 4; x++)
                    for (int y = 16; y <= leftFloorY; y++)
                        WorldGenUtils.SetWall(origin.X + x, origin.Y + y, woodWall);

                // Lower Floor trim
                for (int x = 0; x < 9; x++)
                    WorldGenUtils.SetWall(origin.X + x, origin.Y + leftFloorY, woodWall);

                // Ceiling trim
                for (int x = 0; x < 16; x++)
                    WorldGenUtils.SetWall(origin.X + x + 15, origin.Y - 1, woodWall);
            }
        }
    }

    /// <summary>
    /// Steps descend left to right. Left socket is high, right socket is low.
    /// </summary>
    public class DescendingStairs : Stairs
    {
        public override EdgeSocket Left { get; }
        public override EdgeSocket Right { get; }

        public DescendingStairs()
            : base(descendLeftToRight: true)
        {
            Left = new EdgeSocket(new Point(0, CorridorHeight - 1), SocketDirection.Left);
            Right = new EdgeSocket(new Point(Width - 1, CorridorHeight + StepCount - 1), SocketDirection.Right);
        }
    }

    /// <summary>
    /// Steps ascend left to right. Left socket is low, right socket is high.
    /// </summary>
    public class AscendingStairs : Stairs
    {
        public override EdgeSocket Left { get; }
        public override EdgeSocket Right { get; }

        public AscendingStairs()
            : base(descendLeftToRight: false)
        {
            Left = new EdgeSocket(new Point(0, CorridorHeight + StepCount - 1), SocketDirection.Left);
            Right = new EdgeSocket(new Point(Width - 1, CorridorHeight - 1), SocketDirection.Right);
        }
    }
}
