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
        private const int MaxDepth = 2;

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

        /// <summary>
        /// Recursively plans a chain of rooms starting from cursor.
        /// Returns the first room placed, or null if nothing could be placed.
        /// </summary>
        private static PlacedPiece PlanChain(
            DungeonLayout layout,
            SocketAnchor cursor,
            int roomCount,
            int depth,
            List<IProceduralRoom> roomPool,
            Random rand,
            Point? forceFirstOrigin = null)
        {
            PlacedPiece firstRoom = null;
            PlacedPiece prevConnector = null;

            for (int i = 0; i < roomCount; i++)
            {
                var template = roomPool[rand.Next(roomPool.Count)];

                Point roomOrigin = (forceFirstOrigin.HasValue && i == 0)
                    ? forceFirstOrigin.Value
                    : EdgeSocket.AlignTo(cursor, GetMatchingSocket(template, cursor.Facing));

                if (!layout.CanPlace(roomOrigin, template.Width, template.Height))
                    break;

                var roomPiece = new PlacedPiece(template, roomOrigin);
                layout.Add(roomPiece);

                if (firstRoom == null) firstRoom = roomPiece;

                if (prevConnector != null)
                {
                    prevConnector.Right = roomPiece;
                    roomPiece.Left = prevConnector;
                    prevConnector = null;
                }

                // Branch downward at 33% chance if we haven't hit max depth
                if (depth < MaxDepth)
                {
                    var bottomSocket = template.Bottom;
                    if (bottomSocket?.Accepted?.Count > 0 && rand.Next(3) == 0)
                    {
                        var vertConnector = bottomSocket.Accepted[rand.Next(bottomSocket.Accepted.Count)];
                        var socketWorld = bottomSocket.ToWorldAnchor(roomOrigin);
                        Point vertOrigin = EdgeSocket.AlignTo(socketWorld, GetMatchingSocket(vertConnector, socketWorld.Facing));

                        if (layout.CanPlace(vertOrigin, vertConnector.Width, vertConnector.Height))
                        {
                            // Lookahead: only commit the shaft if at least one room
                            // can be placed at the exit. Prevents orphaned shafts that
                            // carve out space but lead nowhere.
                            var exitAnchor = GetSocket(vertConnector, socketWorld.Facing).ToWorldAnchor(vertOrigin);
                            bool exitClear = false;
                            foreach (var candidate in roomPool)
                            {
                                var candidateOrigin = EdgeSocket.AlignTo(exitAnchor, GetMatchingSocket(candidate, exitAnchor.Facing));
                                if (layout.CanPlace(candidateOrigin, candidate.Width, candidate.Height))
                                {
                                    exitClear = true;
                                    break;
                                }
                            }

                            if (exitClear)
                            {
                                var shaftPiece = new PlacedPiece(vertConnector, vertOrigin);
                                layout.Add(shaftPiece);

                                roomPiece.Bottom = shaftPiece;
                                shaftPiece.Top = roomPiece;

                                int branchRoomCount = rand.Next(3, 5);
                                var firstBranchRoom = PlanChain(layout, exitAnchor, branchRoomCount, depth + 1, roomPool, rand);
                                if (firstBranchRoom != null)
                                {
                                    shaftPiece.Bottom = firstBranchRoom;
                                    firstBranchRoom.Top = shaftPiece;
                                }
                            }
                        }
                    }
                }

                // Advance horizontal chain
                if (i < roomCount - 1)
                {
                    var rightSocket = template.Right;
                    if (rightSocket?.Accepted?.Count > 0)
                    {
                        var connector = rightSocket.Accepted[rand.Next(rightSocket.Accepted.Count)];
                        var socketWorld = rightSocket.ToWorldAnchor(roomOrigin);
                        Point connectorOrigin = EdgeSocket.AlignTo(socketWorld, GetMatchingSocket(connector, socketWorld.Facing));

                        if (!layout.CanPlace(connectorOrigin, connector.Width, connector.Height))
                            break;

                        var connectorPiece = new PlacedPiece(connector, connectorOrigin);
                        layout.Add(connectorPiece);

                        roomPiece.Right = connectorPiece;
                        connectorPiece.Left = roomPiece;
                        prevConnector = connectorPiece;

                        cursor = GetSocket(connector, socketWorld.Facing).ToWorldAnchor(connectorOrigin);
                    }
                }
            }

            return firstRoom;
        }

        private static DungeonLayout Plan(Point start, int roomCount, List<IProceduralRoom> roomPool, Random rand)
        {
            var layout = new DungeonLayout();
            var cursor = new SocketAnchor { Position = start, Facing = SocketDirection.Right };
            PlanChain(layout, cursor, roomCount, 0, roomPool, rand, forceFirstOrigin: start);
            return layout;
        }

        private static void BuildStructure(DungeonLayout layout, int fillTileType, int liningTileType)
        {
            foreach (var piece in layout.AllPieces)
                piece.Template.Build(piece.Origin, fillTileType, liningTileType);
        }

        private static void Decorate(DungeonLayout layout)
        {
            // Process deepest shafts first. Shafts are added depth-first so deeper
            // shafts always appear later in the list. Reversing ensures each shaft's
            // stairs are present before the shaft above tries to anchor onto them.
            for (int idx = layout.Shafts.Count - 1; idx >= 0; idx--)
            {
                var shaft = layout.Shafts[idx];
                // Use direct neighbors only — each shaft is decorated independently.
                // Walking the full chain caused multi-level shafts to share one run
                // and have the second shaft skipped by the visited set.
                var topPiece = shaft.Top;
                var bottomPiece = shaft.Bottom;

                if (topPiece == null || bottomPiece == null) continue;
                if (topPiece.Template.Bottom == null || bottomPiece.Template.Bottom == null) continue;

                int shaftCenterX = shaft.Origin.X + shaft.Template.Top.RelativePosition.X;
                int topY = topPiece.Origin.Y + topPiece.Template.Bottom.RelativePosition.Y;
                int bottomY = bottomPiece.Origin.Y + bottomPiece.Template.Bottom.RelativePosition.Y;
                int segmentCount = (bottomY - topY) / 10;

                int stairX = shaftCenterX - 7;
                int capX = shaftCenterX - 2;

                for (int s = segmentCount - 1; s >= 0; s--)
                    WorldGen.PlaceObject(stairX, topY + s * 10 + 10, ModContent.TileType<DiagonalStairs>());

                // Only cap the top of the chain — if the room above this shaft itself
                // sits below another shaft, a cap here would land on the bottom
                // DiagonalStairs segment of the shaft above, breaking its placement.
                bool isTopOfChain = !(topPiece.Top is PlacedPiece above && above.Template is VerticalStairs);
                if (isTopOfChain)
                    WorldGen.PlaceObject(capX, topY, ModContent.TileType<StairCap>());
            }
        }

        public static void Build(
            Point start, Point target, int roomCount,
            List<IProceduralRoom> roomPool,
            int fillTileType, int liningTileType, Random rand)
        {
            int minX = Math.Max(0, Math.Min(start.X, target.X) - FillPadding);
            int maxX = Math.Min(Main.maxTilesX - 1, Math.Max(start.X, target.X) + roomCount * 300 + FillPadding);
            int minY = Math.Max(0, Math.Min(start.Y, target.Y) - roomCount * 100 - FillPadding);
            int maxY = Math.Min(Main.maxTilesY - 1, Math.Max(start.Y, target.Y) + roomCount * 100 + FillPadding);

            for (int x = minX; x <= maxX; x++)
                for (int y = minY; y <= maxY; y++)
                    WorldGen.PlaceTile(x, y, fillTileType, true, true);

            var layout = Plan(start, roomCount, roomPool, rand);
            BuildStructure(layout, fillTileType, liningTileType);
            Decorate(layout);
        }
    }
}
