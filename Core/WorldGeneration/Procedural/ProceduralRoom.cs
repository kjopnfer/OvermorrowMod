using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public class ProceduralRoom
    {
        /// <summary>
        /// Top-left corner in tile coordinates.
        /// </summary>
        public Point Position { get; }

        /// <summary>
        /// Width in tiles.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Height in tiles.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Center of the room in tile coordinates.
        /// </summary>
        public Point Center => new Point(Position.X + Width / 2, Position.Y + Height / 2);

        public ProceduralRoom(Point position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Generates a hollow rectangle of tiles.
        /// Border is solid, interior is air.
        /// </summary>
        public void Generate(int tileType)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int worldX = Position.X + x;
                    int worldY = Position.Y + y;

                    bool isBorder = x == 0 || x == Width - 1 || y == 0 || y == Height - 1;

                    if (isBorder)
                    {
                        WorldGen.PlaceTile(worldX, worldY, tileType, true, true);
                    }
                }
            }
        }

        // ArchiveDoor is 12 tiles wide, 15 tiles tall.
        // Anchor is at bottom-left (Origin = 0, 14).
        // PlaceTileWithEntity places at the anchor coordinate.
        private const int DoorWidth = 12;
        private const int DoorHeight = 15;

        /// <summary>
        /// Returns the anchor position (bottom-left) for a door placement on the wall
        /// facing the given target point.
        /// Accounts for door dimensions: ensures the door fits within the room interior
        /// and sits 1 tile above the floor.
        /// </summary>
        public Point GetDoorPosition(Point target)
        {
            Vector2 direction = new Vector2(target.X - Center.X, target.Y - Center.Y);

            // Floor Y: the door anchor sits 1 tile above the floor (bottom wall)
            int floorY = Position.Y + Height - 2;

            if (System.Math.Abs(direction.X) > System.Math.Abs(direction.Y))
            {
                // Door on left or right wall
                int doorX;
                if (direction.X > 0)
                {
                    // Right wall: place anchor so the door's right edge doesn't clip into the wall
                    // Anchor is bottom-left, so anchor X = right wall - door width
                    doorX = Position.X + Width - 1 - DoorWidth;
                }
                else
                {
                    // Left wall: place anchor just inside the left wall
                    doorX = Position.X + 1;
                }

                return new Point(doorX, floorY);
            }
            else
            {
                // Door on top or bottom wall — center horizontally
                int doorX = Center.X - DoorWidth / 2;
                return new Point(doorX, floorY);
            }
        }
    }
}
