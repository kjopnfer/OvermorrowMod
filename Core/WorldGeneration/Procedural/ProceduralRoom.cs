using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public class ProceduralRoom
    {
        public Point Position { get; }
        public int Width { get; }
        public int Height { get; }
        public Point Center => new Point(Position.X + Width / 2, Position.Y + Height / 2);
        public int FloorY => Position.Y + Height - 2;
        public Point FloorCenter => new Point(Position.X + Width / 2, FloorY);

        public ProceduralRoom(Point position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        private const int DoorWidth = 12;
        private const int DoorPadding = 3;

        public Point GetDoorPosition(Point target)
        {
            Vector2 direction = new Vector2(target.X - Center.X, target.Y - Center.Y);

            int doorX;
            if (direction.X > 0)
                doorX = Position.X + Width - 1 - DoorWidth - DoorPadding;
            else
                doorX = Position.X + 1 + DoorPadding;

            return new Point(doorX, FloorY);
        }
    }
}
