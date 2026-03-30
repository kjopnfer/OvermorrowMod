using Microsoft.Xna.Framework;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralChain
    {
        private const int FillPadding = 20;

        /// <summary>
        /// Gets the matching socket on a piece for a given facing direction.
        /// A Right-facing anchor needs a Left socket on the target, etc.
        /// </summary>
        private static EdgeSocket GetMatchingSocket(IProceduralRoom piece, SocketDirection fromFacing)
        {
            return (fromFacing.Opposite()) switch
            {
                SocketDirection.Left => piece.Left,
                SocketDirection.Right => piece.Right,
                SocketDirection.Up => piece.Top,
                SocketDirection.Down => piece.Bottom,
                _ => null
            };
        }

        public static List<ProceduralRoom> Build(
            Point start, Point target, int roomCount,
            List<IProceduralRoom> roomPool,
            int fillTileType, int liningTileType, Random rand)
        {
            var rooms = new List<ProceduralRoom>();

            int minX = Math.Min(start.X, target.X) - FillPadding;
            int maxX = Math.Max(start.X, target.X) + 200 + FillPadding;
            int minY = Math.Min(start.Y, target.Y) - 100 - FillPadding;
            int maxY = Math.Max(start.Y, target.Y) + 100 + FillPadding;

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    WorldGen.PlaceTile(x, y, fillTileType, true, true);

            SocketAnchor cursor = new SocketAnchor
            {
                Position = start,
                Facing = SocketDirection.Right
            };

            for (int i = 0; i < roomCount; i++)
            {
                var template = roomPool[rand.Next(roomPool.Count)];

                Point roomOrigin;
                if (i == 0)
                {
                    roomOrigin = start;
                }
                else
                {
                    var matchingSocket = GetMatchingSocket(template, cursor.Facing);
                    roomOrigin = EdgeSocket.AlignTo(cursor, matchingSocket);
                }

                cursor = template.Build(roomOrigin, fillTileType, liningTileType);

                if (i < roomCount - 1)
                {
                    var rightSocket = template.Right;
                    if (rightSocket != null && rightSocket.Accepted != null && rightSocket.Accepted.Count > 0)
                    {
                        var connector = rightSocket.Accepted[rand.Next(rightSocket.Accepted.Count)];
                        var socketWorld = rightSocket.ToWorldAnchor(roomOrigin);
                        var connectorLeft = GetMatchingSocket(connector, socketWorld.Facing);
                        Point connectorOrigin = EdgeSocket.AlignTo(socketWorld, connectorLeft);
                        cursor = connector.Build(connectorOrigin, fillTileType, liningTileType);
                    }
                }

                var bottomSocket = template.Bottom;
                if (bottomSocket != null && bottomSocket.Accepted != null && bottomSocket.Accepted.Count > 0)
                {
                    var vertConnector = bottomSocket.Accepted[rand.Next(bottomSocket.Accepted.Count)];
                    var socketWorld = bottomSocket.ToWorldAnchor(roomOrigin);
                    var connectorTop = GetMatchingSocket(vertConnector, socketWorld.Facing);
                    Point vertOrigin = EdgeSocket.AlignTo(socketWorld, connectorTop);
                    var vertExit = vertConnector.Build(vertOrigin, fillTileType, liningTileType);

                    var belowTemplate = roomPool[rand.Next(roomPool.Count)];
                    var belowSocket = GetMatchingSocket(belowTemplate, vertExit.Facing);
                    Point belowOrigin = EdgeSocket.AlignTo(vertExit, belowSocket);
                    belowTemplate.Build(belowOrigin, fillTileType, liningTileType);
                }
            }

            return rooms;
        }
    }
}
