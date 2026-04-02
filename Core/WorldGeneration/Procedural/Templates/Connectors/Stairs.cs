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

        private readonly bool _descendLeftToRight;

        protected Stairs(bool descendLeftToRight)
        {
            _descendLeftToRight = descendLeftToRight;
        }

        public void Build(Point origin, int fillTileType, int liningTileType)
        {
            int woodTile = ModContent.TileType<ArchiveWood>();

            int leftFloorY = _descendLeftToRight ? CorridorHeight : CorridorHeight + StepCount;
            int rightFloorY = _descendLeftToRight ? CorridorHeight + StepCount : CorridorHeight;

            for (int x = 0; x < LandingWidth; x++)
            {
                for (int y = 0; y < leftFloorY; y++)
                    WorldGen.KillTile(origin.X + x, origin.Y + y, false, false, true);
                for (int d = 0; d < 4; d++)
                    WorldGen.PlaceTile(origin.X + x, origin.Y + leftFloorY + d, woodTile, true, true);
            }

            for (int i = 0; i < StepCount; i++)
            {
                int stepY = _descendLeftToRight ? leftFloorY + 1 + i : leftFloorY - 1 - i;
                for (int y = 0; y < stepY; y++)
                    WorldGen.KillTile(origin.X + LandingWidth + i, origin.Y + y, false, false, true);
                for (int d = 0; d < 4; d++)
                    WorldGen.PlaceTile(origin.X + LandingWidth + i, origin.Y + stepY + d, woodTile, true, true);
            }

            for (int x = 0; x < LandingWidth; x++)
            {
                for (int y = 0; y < rightFloorY; y++)
                    WorldGen.KillTile(origin.X + LandingWidth + StepCount + x, origin.Y + y, false, false, true);
                for (int d = 0; d < 4; d++)
                    WorldGen.PlaceTile(origin.X + LandingWidth + StepCount + x, origin.Y + rightFloorY + d, woodTile, true, true);
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
