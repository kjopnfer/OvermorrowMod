using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors
{
    public abstract class Stairs : IProceduralRoom
    {
        protected const int LandingWidth = 20;
        protected const int StepCount = 17;
        protected const int CorridorHeight = 26;

        public int Width => LandingWidth + StepCount + LandingWidth;
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

            void Clear(int wx, int wy)
            {
                Tile t = Main.tile[wx, wy];
                t.HasTile = false;
            }

            void Place(int wx, int wy)
            {
                Tile t = Main.tile[wx, wy];
                t.TileType = woodTile; t.HasTile = true;
            }

            for (int x = 0; x < LandingWidth; x++)
            {
                for (int y = 0; y < leftFloorY; y++)
                    Clear(origin.X + x, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    Place(origin.X + x, origin.Y + leftFloorY + d);
            }

            for (int i = 0; i < StepCount; i++)
            {
                int stepY = _descendLeftToRight ? leftFloorY + 1 + i : leftFloorY - 1 - i;
                for (int y = 0; y < stepY; y++)
                    Clear(origin.X + LandingWidth + i, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    Place(origin.X + LandingWidth + i, origin.Y + stepY + d);
            }

            for (int x = 0; x < LandingWidth; x++)
            {
                for (int y = 0; y < rightFloorY; y++)
                    Clear(origin.X + LandingWidth + StepCount + x, origin.Y + y);
                for (int d = 0; d < 4; d++)
                    Place(origin.X + LandingWidth + StepCount + x, origin.Y + rightFloorY + d);
            }

            int woodWall = ModContent.WallType<ArchiveWoodWall>();
            int blueWall = ModContent.WallType<ArchiveWoodWallBlue>();

            void DrawColoredPanel(int rx, int ry, int w, int h)
            {
                for (int lx = 0; lx < w; lx++)
                    for (int ly = 0; ly < h; ly++)
                    {
                        bool isBorder = (lx == 0 || lx == w - 1 || ly == 0 || ly == h - 1);
                        if (!isBorder)
                            WorldGen.PlaceWall(rx + lx, ry + ly, blueWall, true);
                    }
            }

            if (_descendLeftToRight)
            {
                ProceduralUtils.DrawWallPanel(origin.X, origin.Y, LandingWidth, leftFloorY, woodWall, blueWall);

                DrawColoredPanel(origin.X + LandingWidth, origin.Y, 5, leftFloorY + 1);
                DrawColoredPanel(origin.X + LandingWidth + 4, origin.Y, 5, leftFloorY + 5);
                DrawColoredPanel(origin.X + LandingWidth + 8, origin.Y, 5, leftFloorY + 9);
                DrawColoredPanel(origin.X + LandingWidth + 12, origin.Y, 5, leftFloorY + 13);
                DrawColoredPanel(origin.X + LandingWidth + 16, origin.Y, 5, leftFloorY + 17);

                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGen.PlaceWall(origin.X + LandingWidth + x - 1, origin.Y + leftFloorY + y + 1, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 3, origin.Y + leftFloorY + y + 5, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 7, origin.Y + leftFloorY + y + 9, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 11, origin.Y + leftFloorY + y + 13, woodWall);
                    }

                for (int x = 0; x < 20; x++)
                    WorldGen.PlaceWall(origin.X + LandingWidth + x + 17, origin.Y + leftFloorY + 17, woodWall);

                for (int x = 0; x < 36; x++)
                    WorldGen.PlaceWall(origin.X + LandingWidth + x + 1, origin.Y - 1, woodWall);

                ProceduralUtils.DrawWallPanel(origin.X + LandingWidth + StepCount + 4, origin.Y, LandingWidth - 4, rightFloorY, woodWall, blueWall);
            }
            else
            {
                ProceduralUtils.DrawWallPanel(origin.X, origin.Y, LandingWidth - 4, leftFloorY, woodWall, blueWall);

                DrawColoredPanel(origin.X + LandingWidth - 4, origin.Y, 5, rightFloorY + 17);
                DrawColoredPanel(origin.X + LandingWidth, origin.Y, 5, rightFloorY + 13);
                DrawColoredPanel(origin.X + LandingWidth + 4, origin.Y, 5, rightFloorY + 9);
                DrawColoredPanel(origin.X + LandingWidth + 8, origin.Y, 5, rightFloorY + 5);
                DrawColoredPanel(origin.X + LandingWidth + 12, origin.Y, 5, rightFloorY + 1);

                for (int x = 0; x <= 4; x++)
                    for (int y = 0; y < 4; y++)
                    {
                        WorldGen.PlaceWall(origin.X + LandingWidth + x - 3, origin.Y + leftFloorY + y, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 1, origin.Y + leftFloorY - 4 + y, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 5, origin.Y + leftFloorY - 8 + y, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 9, origin.Y + leftFloorY - 12 + y, woodWall);
                        WorldGen.PlaceWall(origin.X + LandingWidth + x + 13, origin.Y + leftFloorY - 16 + y, woodWall);
                    }

                for (int x = 0; x < 20; x++)
                    WorldGen.PlaceWall(origin.X + LandingWidth + x - 1, origin.Y + leftFloorY, woodWall);

                for (int x = 0; x < 36; x++)
                    WorldGen.PlaceWall(origin.X + LandingWidth + x - 3, origin.Y - 1, woodWall);

                ProceduralUtils.DrawWallPanel(origin.X + LandingWidth + StepCount, origin.Y, LandingWidth, rightFloorY, woodWall, blueWall);
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
