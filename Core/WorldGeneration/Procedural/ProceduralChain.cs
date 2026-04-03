using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates;
using OvermorrowMod.Core.WorldGeneration.Procedural.Templates.Connectors;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralChain
    {
        private const int FillPadding = 20;

        private static EdgeSocket GetMatchingSocket(IProceduralRoom piece, SocketDirection fromFacing)
        {
            return fromFacing.Opposite() switch
            {
                SocketDirection.Left => piece.Left,
                SocketDirection.Right => piece.Right,
                SocketDirection.Up => piece.Top,
                SocketDirection.Down => piece.Bottom,
                _ => null
            };
        }

        private static EdgeSocket GetSocket(IProceduralRoom piece, SocketDirection facing)
        {
            return facing switch
            {
                SocketDirection.Left => piece.Left,
                SocketDirection.Right => piece.Right,
                SocketDirection.Up => piece.Top,
                SocketDirection.Down => piece.Bottom,
                _ => null
            };
        }

        private static DungeonLayout Plan(Point start, int roomCount, List<IProceduralRoom> roomPool, Random rand)
        {
            var layout = new DungeonLayout();

            SocketAnchor cursor = new SocketAnchor { Position = start, Facing = SocketDirection.Right };
            PlacedPiece prevConnector = null;

            for (int i = 0; i < roomCount; i++)
            {
                var template = roomPool[rand.Next(roomPool.Count)];

                Point roomOrigin = i == 0 ? start : EdgeSocket.AlignTo(cursor, GetMatchingSocket(template, cursor.Facing));

                var roomPiece = new PlacedPiece(template, roomOrigin);
                layout.Add(roomPiece);

                if (prevConnector != null)
                {
                    prevConnector.Right = roomPiece;
                    roomPiece.Left = prevConnector;
                    prevConnector = null;
                }

                if (i < roomCount - 1)
                {
                    var rightSocket = template.Right;
                    if (rightSocket?.Accepted?.Count > 0)
                    {
                        var connector = rightSocket.Accepted[rand.Next(rightSocket.Accepted.Count)];
                        var socketWorld = rightSocket.ToWorldAnchor(roomOrigin);
                        Point connectorOrigin = EdgeSocket.AlignTo(socketWorld, GetMatchingSocket(connector, socketWorld.Facing));

                        var connectorPiece = new PlacedPiece(connector, connectorOrigin);
                        layout.Add(connectorPiece);

                        roomPiece.Right = connectorPiece;
                        connectorPiece.Left = roomPiece;
                        prevConnector = connectorPiece;

                        cursor = GetSocket(connector, socketWorld.Facing).ToWorldAnchor(connectorOrigin);
                    }
                }

                var bottomSocket = template.Bottom;
                if (bottomSocket?.Accepted?.Count > 0 && rand.Next(2) == 0)
                {
                    var vertConnector = bottomSocket.Accepted[rand.Next(bottomSocket.Accepted.Count)];
                    var socketWorld = bottomSocket.ToWorldAnchor(roomOrigin);
                    Point vertOrigin = EdgeSocket.AlignTo(socketWorld, GetMatchingSocket(vertConnector, socketWorld.Facing));

                    var shaftPiece = new PlacedPiece(vertConnector, vertOrigin);
                    layout.Add(shaftPiece);

                    roomPiece.Bottom = shaftPiece;
                    shaftPiece.Top = roomPiece;

                    var vertExitAnchor = GetSocket(vertConnector, socketWorld.Facing).ToWorldAnchor(vertOrigin);
                    var belowTemplate = roomPool[rand.Next(roomPool.Count)];
                    Point belowOrigin = EdgeSocket.AlignTo(vertExitAnchor, GetMatchingSocket(belowTemplate, vertExitAnchor.Facing));

                    var belowPiece = new PlacedPiece(belowTemplate, belowOrigin);
                    layout.Add(belowPiece);

                    shaftPiece.Bottom = belowPiece;
                    belowPiece.Top = shaftPiece;
                }
            }

            return layout;
        }

        private static void BuildStructure(DungeonLayout layout, int fillTileType, int liningTileType)
        {
            foreach (var piece in layout.AllPieces)
                piece.Template.Build(piece.Origin, fillTileType, liningTileType);
        }

        private static void Decorate(DungeonLayout layout)
        {
            var visited = new HashSet<PlacedPiece>();

            foreach (var shaft in layout.Shafts)
            {
                if (visited.Contains(shaft)) continue;

                // Find topmost piece in this vertical run
                var topPiece = shaft;
                while (topPiece.Top != null)
                    topPiece = topPiece.Top;

                // Walk down from top: mark shafts visited, find bottommost piece, capture shaft center X
                var bottomPiece = topPiece;
                var current = topPiece;
                int shaftCenterX = -1;
                while (current != null)
                {
                    if (current.Template is VerticalStairs)
                    {
                        visited.Add(current);
                        if (shaftCenterX == -1)
                            shaftCenterX = current.Origin.X + current.Template.Top.RelativePosition.X;
                    }
                    bottomPiece = current;
                    current = current.Bottom;
                }

                if (topPiece.Template.Bottom == null || bottomPiece.Template.Bottom == null)
                    continue;

                int topY = topPiece.Origin.Y + topPiece.Template.Bottom.RelativePosition.Y;
                int bottomY = bottomPiece.Origin.Y + bottomPiece.Template.Bottom.RelativePosition.Y;
                int segmentCount = (bottomY - topY) / 10;

                // DiagonalStairs: 14 wide, origin at (0, 9) — center in shaft
                // StairCap: 5 wide, origin at (0, 3) — extends into room above
                int stairX = shaftCenterX - 7;
                int capX = shaftCenterX - 2;

                // Shift down by 1 so the bottommost segment anchors on bottomY + 1,
                // which is outside the room's cleared interior and still has solid fill
                for (int s = segmentCount - 1; s >= 0; s--)
                    WorldGen.PlaceObject(stairX, topY + s * 10 + 10, ModContent.TileType<DiagonalStairs>());

                // Cap sits one tile above the topmost segment (top row at topY)
                WorldGen.PlaceObject(capX, topY, ModContent.TileType<StairCap>());
            }
        }

        public static void Build(
            Point start, Point target, int roomCount,
            List<IProceduralRoom> roomPool,
            int fillTileType, int liningTileType, Random rand)
        {
            int minX = Math.Min(start.X, target.X) - FillPadding;
            int maxX = Math.Max(start.X, target.X) + roomCount * 200 + FillPadding;
            int minY = Math.Min(start.Y, target.Y) - roomCount * 60 - FillPadding;
            int maxY = Math.Max(start.Y, target.Y) + roomCount * 60 + FillPadding;

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    WorldGen.PlaceTile(x, y, fillTileType, true, true);

            var layout = Plan(start, roomCount, roomPool, rand);
            BuildStructure(layout, fillTileType, liningTileType);
            Decorate(layout);
        }
    }
}
