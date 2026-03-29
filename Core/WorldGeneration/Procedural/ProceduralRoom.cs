using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

        /// <summary>
        /// One edge socket per side, null if no connection on that side.
        /// </summary>
        public EdgeSocket Left { get; set; }
        public EdgeSocket Right { get; set; }
        public EdgeSocket Top { get; set; }
        public EdgeSocket Bottom { get; set; }

        /// <summary>
        /// Interior sockets for furniture/features.
        /// </summary>
        public List<InteriorSocket> InteriorSockets { get; } = new List<InteriorSocket>();

        public ProceduralRoom(Point position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
        }

        public void SetEdgeSocket(EdgeSocket socket)
        {
            socket.Owner = this;
            switch (socket.Facing)
            {
                case SocketDirection.Left: Left = socket; break;
                case SocketDirection.Right: Right = socket; break;
                case SocketDirection.Up: Top = socket; break;
                case SocketDirection.Down: Bottom = socket; break;
            }
        }

        public void AddInteriorSocket(InteriorSocket socket)
        {
            socket.Owner = this;
            InteriorSockets.Add(socket);
        }

        /// <summary>
        /// Get edge socket by direction.
        /// </summary>
        public EdgeSocket GetEdgeSocket(SocketDirection facing) => facing switch
        {
            SocketDirection.Left => Left,
            SocketDirection.Right => Right,
            SocketDirection.Up => Top,
            SocketDirection.Down => Bottom,
            _ => null
        };

    }
}
