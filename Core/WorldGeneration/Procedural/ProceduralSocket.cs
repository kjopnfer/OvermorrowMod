using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public enum SocketDirection { Left, Right, Up, Down }

    public static class SocketDirectionExtensions
    {
        public static SocketDirection Opposite(this SocketDirection dir) => dir switch
        {
            SocketDirection.Left => SocketDirection.Right,
            SocketDirection.Right => SocketDirection.Left,
            SocketDirection.Up => SocketDirection.Down,
            SocketDirection.Down => SocketDirection.Up,
            _ => dir
        };
    }

    public struct SocketAnchor
    {
        /// <summary>
        /// Single alignment point: floor-level at the edge, in world coordinates.
        /// </summary>
        public Point Position;

        /// <summary>
        /// Direction the opening faces.
        /// </summary>
        public SocketDirection Facing;
    }

    /// <summary>
    /// Alignment pin on room walls for room-to-corridor connections.
    /// No width/height — templates handle their own clearing.
    /// </summary>
    public class EdgeSocket
    {
        /// <summary>
        /// Room-relative floor point at the edge.
        /// </summary>
        public Point RelativePosition { get; }

        /// <summary>
        /// Direction this socket faces.
        /// </summary>
        public SocketDirection Facing { get; }

        /// <summary>
        /// What connectors can plug into this socket. Null/empty for input-only sockets.
        /// </summary>
        public List<IProcedural> Accepted { get; }

        /// <summary>
        /// The room this socket belongs to. Set when added to a room.
        /// </summary>
        public ProceduralRoom Owner { get; internal set; }

        public EdgeSocket(Point relativePosition, SocketDirection facing, List<IProcedural> accepted = null)
        {
            RelativePosition = relativePosition;
            Facing = facing;
            Accepted = accepted;
        }

        /// <summary>
        /// Returns the top-left origin of where the next connected piece starts.
        /// This is one tile past the socket in the facing direction.
        /// </summary>
        public SocketAnchor ToAnchor()
        {
            int worldX = Owner.Position.X + RelativePosition.X;
            int worldY = Owner.Position.Y + RelativePosition.Y;

            // Offset by 1 in the facing direction to get the origin of the next space
            Point origin = Facing switch
            {
                SocketDirection.Right => new Point(worldX + 1, worldY),
                SocketDirection.Left => new Point(worldX - 1, worldY),
                SocketDirection.Down => new Point(worldX, worldY + 1),
                SocketDirection.Up => new Point(worldX, worldY - 1),
                _ => new Point(worldX, worldY)
            };

            return new SocketAnchor { Position = origin, Facing = Facing };
        }

        /// <summary>
        /// Compute room origin so this socket aligns with the given anchor.
        /// Universal formula — works for all rooms, all directions.
        /// </summary>
        public Point AlignRoom(SocketAnchor anchor) => new Point(
            anchor.Position.X - RelativePosition.X,
            anchor.Position.Y - RelativePosition.Y
        );
    }

    /// <summary>
    /// Bounded region inside a room for furniture/features.
    /// Has position + width + height defining the area where something gets placed.
    /// </summary>
    public class InteriorSocket
    {
        /// <summary>
        /// Room-relative bounded area.
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        /// What can be placed in this socket.
        /// </summary>
        public List<IProcedural> Accepted { get; }

        /// <summary>
        /// The room this socket belongs to. Set when added to a room.
        /// </summary>
        public ProceduralRoom Owner { get; internal set; }

        public InteriorSocket(Rectangle bounds, List<IProcedural> accepted = null)
        {
            Bounds = bounds;
            Accepted = accepted;
        }

        /// <summary>
        /// Returns the socket bounds in absolute world coordinates.
        /// </summary>
        public Rectangle GetWorldBounds() => new Rectangle(
            Owner.Position.X + Bounds.X, Owner.Position.Y + Bounds.Y,
            Bounds.Width, Bounds.Height
        );
    }
}
