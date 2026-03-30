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

        public List<IProceduralRoom> Accepted { get; }

        /// <summary>
        /// The room this socket belongs to. Set when added to a room.
        /// </summary>
        public ProceduralRoom Owner { get; internal set; }

        public EdgeSocket(Point relativePosition, SocketDirection facing, List<IProceduralRoom> accepted = null)
        {
            RelativePosition = relativePosition;
            Facing = facing;
            Accepted = accepted;
        }

        /// <summary>
        /// Returns this socket's world position given the owner's origin.
        /// </summary>
        public SocketAnchor ToWorldAnchor(Point ownerOrigin) => new SocketAnchor
        {
            Position = new Point(ownerOrigin.X + RelativePosition.X, ownerOrigin.Y + RelativePosition.Y),
            Facing = Facing
        };

        /// <summary>
        /// Computes the origin of a piece so that its matching socket sits adjacent to the given anchor.
        /// Pieces end up side by side, not overlapping.
        /// </summary>
        public static Point AlignTo(SocketAnchor from, EdgeSocket targetSocket)
        {
            Point offset = from.Facing switch
            {
                SocketDirection.Right => new Point(1, 0),
                SocketDirection.Left => new Point(-1, 0),
                SocketDirection.Down => new Point(0, 1),
                SocketDirection.Up => new Point(0, -1),
                _ => Point.Zero
            };

            return new Point(
                from.Position.X + offset.X - targetSocket.RelativePosition.X,
                from.Position.Y + offset.Y - targetSocket.RelativePosition.Y
            );
        }
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
        public List<IProceduralRoom> Accepted { get; }

        /// <summary>
        /// The room this socket belongs to. Set when added to a room.
        /// </summary>
        public ProceduralRoom Owner { get; internal set; }

        public InteriorSocket(Rectangle bounds, List<IProceduralRoom> accepted = null)
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
