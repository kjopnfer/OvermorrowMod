using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public enum ConnectionType { Corridor, Doors }

    public struct Connection
    {
        public ConnectionType Type;
        public int RoomAIndex;
        public int RoomBIndex;

        public CorridorPlan? Corridor;

        public int DoorAID;
        public int DoorBID;
    }

    public static class ProceduralGenerator
    {
        private const int DoorIDOffset = 1000;
        private const int DoorWidth = 12;
        private const int DoorHeight = 15;
        private const int DoorPadding = 3;

        private static readonly int[] CorridorHeightChanges = { 0, 0, 0, 6, 8, 10, 20 };

        public static List<ProceduralRoom> Generate(
            Point pointA, Point pointB, int roomCount,
            int minRoomWidth, int maxRoomWidth,
            int minRoomHeight, int maxRoomHeight,
            int corridorHeight,
            int fillTileType, int liningTileType)
        {
            var rand = new Random(Environment.TickCount);

            // ==================
            // PASS 1: STRUCTURE
            // ==================
            var rooms = new List<ProceduralRoom>();
            var connections = new List<Connection>();

            int cursorX = pointA.X;
            int cursorFloorY = pointA.Y;
            int nextDoorID = DoorIDOffset;

            for (int i = 0; i < roomCount; i++)
            {
                int roomWidth = rand.Next(minRoomWidth, maxRoomWidth + 1);
                int roomHeight = rand.Next(minRoomHeight, maxRoomHeight + 1);

                Point topLeft = new Point(cursorX, cursorFloorY - roomHeight + 2);
                rooms.Add(new ProceduralRoom(topLeft, roomWidth, roomHeight));

                if (i < roomCount - 1)
                {
                    int heightChange = CorridorHeightChanges[rand.Next(CorridorHeightChanges.Length)];
                    int sign = rand.Next(2) == 0 ? 1 : -1;
                    heightChange *= sign;

                    int targetDY = pointB.Y - cursorFloorY;
                    if (Math.Abs(targetDY) > 5 && Math.Sign(heightChange) != Math.Sign(targetDY) && rand.Next(3) > 0)
                        heightChange = Math.Abs(heightChange) * Math.Sign(targetDY);

                    // After a door connection, force the next one to be a corridor
                    bool lastWasDoor = connections.Count > 0
                        && connections[connections.Count - 1].Type == ConnectionType.Doors;
                    bool useCorridor = lastWasDoor || rand.Next(2) == 0;

                    if (useCorridor)
                    {
                        int corridorStartX = cursorX + roomWidth;
                        var plan = ProceduralCorridor.Plan(corridorStartX, cursorFloorY, heightChange, rand);

                        connections.Add(new Connection
                        {
                            Type = ConnectionType.Corridor,
                            RoomAIndex = i,
                            RoomBIndex = i + 1,
                            Corridor = plan
                        });

                        cursorX = plan.EndX;
                        cursorFloorY = plan.EndFloorY;
                    }
                    else
                    {
                        int doorAID = nextDoorID++;
                        int doorBID = nextDoorID++;

                        connections.Add(new Connection
                        {
                            Type = ConnectionType.Doors,
                            RoomAIndex = i,
                            RoomBIndex = i + 1,
                            DoorAID = doorAID,
                            DoorBID = doorBID
                        });

                        cursorX += roomWidth + rand.Next(20, 40);
                        int doorHeightChange = CorridorHeightChanges[rand.Next(CorridorHeightChanges.Length)];
                        cursorFloorY += doorHeightChange * (rand.Next(2) == 0 ? 1 : -1);
                    }
                }
            }

            // ==================
            // PASS 2: FILL
            // ==================
            FillRegion(rooms, fillTileType, padding: 10);

            // ==================
            // PASS 3: CLEAR
            // ==================
            foreach (var room in rooms)
            {
                ClearRoom(room);
            }

            foreach (var conn in connections)
            {
                if (conn.Type == ConnectionType.Corridor)
                {
                    var plan = conn.Corridor.Value;
                    ProceduralCorridor.Clear(plan, corridorHeight, fillTileType);

                    // Determine the actual ceiling height for this corridor type
                    int clearHeight = plan.Type == CorridorType.Bridge ? 25 : corridorHeight;

                    // Clear room A's right wall where the corridor starts
                    var roomA = rooms[conn.RoomAIndex];
                    int wallX = roomA.Position.X + roomA.Width - 1;
                    for (int y = plan.StartFloorY - clearHeight; y <= plan.StartFloorY; y++)
                        WorldGen.KillTile(wallX, y, false, false, true);

                    // Clear room B's left wall where the corridor ends
                    var roomB = rooms[conn.RoomBIndex];
                    int wallBX = roomB.Position.X;
                    for (int y = plan.EndFloorY - clearHeight; y <= plan.EndFloorY; y++)
                        WorldGen.KillTile(wallBX, y, false, false, true);
                }
            }

            // ==================
            // PASS 4: BORDERS
            // ==================
            PlaceWoodBorders(rooms, liningTileType, fillTileType, padding: 10);

            // ==================
            // PASS 5: FURNITURE
            // ==================
            foreach (var conn in connections)
            {
                if (conn.Type == ConnectionType.Corridor)
                {
                    ProceduralCorridor.PlaceObjects(conn.Corridor.Value, corridorHeight);
                }
                else
                {
                    var roomA = rooms[conn.RoomAIndex];
                    var roomB = rooms[conn.RoomBIndex];

                    Point doorAPos = roomA.GetDoorPosition(roomB.Center);
                    Point doorBPos = roomB.GetDoorPosition(roomA.Center);

                    PlaceDoor(doorAPos.X, doorAPos.Y, conn.DoorAID, conn.DoorBID);
                    PlaceDoor(doorBPos.X, doorBPos.Y, conn.DoorBID, conn.DoorAID);
                }
            }

            return rooms;
        }

        private static void ClearRoom(ProceduralRoom room)
        {
            for (int x = 1; x < room.Width - 1; x++)
            {
                for (int y = 1; y < room.Height - 1; y++)
                {
                    WorldGen.KillTile(room.Position.X + x, room.Position.Y + y, false, false, true);
                }
            }
        }

        private const int BorderThickness = 4;

        private static void PlaceWoodBorders(List<ProceduralRoom> rooms, int liningTileType, int fillTileType, int padding)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var room in rooms)
            {
                if (room.Position.X < minX) minX = room.Position.X;
                if (room.Position.Y < minY) minY = room.Position.Y;
                if (room.Position.X + room.Width > maxX) maxX = room.Position.X + room.Width;
                if (room.Position.Y + room.Height > maxY) maxY = room.Position.Y + room.Height;
            }

            minX -= padding;
            minY -= padding;
            maxX += padding;
            maxY += padding;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);
                    if (!tile.HasTile) continue;
                    if (tile.TileType != fillTileType) continue;

                    Tile above = Framing.GetTileSafely(x, y - 1);
                    Tile below = Framing.GetTileSafely(x, y + 1);

                    // Floor edge: this tile is stone and the tile below is air
                    if (!below.HasTile)
                    {
                        for (int t = 0; t < BorderThickness; t++)
                        {
                            int wy = y - t;
                            Tile target = Framing.GetTileSafely(x, wy);
                            if (target.HasTile && target.TileType == fillTileType)
                            {
                                WorldGen.KillTile(x, wy, false, false, true);
                                WorldGen.PlaceTile(x, wy, liningTileType, true, true);
                            }
                        }
                    }

                    // Ceiling edge: this tile is stone and the tile above is air
                    if (!above.HasTile)
                    {
                        for (int t = 0; t < BorderThickness; t++)
                        {
                            int wy = y + t;
                            Tile target = Framing.GetTileSafely(x, wy);
                            if (target.HasTile && target.TileType == fillTileType)
                            {
                                WorldGen.KillTile(x, wy, false, false, true);
                                WorldGen.PlaceTile(x, wy, liningTileType, true, true);
                            }
                        }
                    }
                }
            }
        }

        private static void FillRegion(List<ProceduralRoom> rooms, int tileType, int padding)
        {
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var room in rooms)
            {
                if (room.Position.X < minX) minX = room.Position.X;
                if (room.Position.Y < minY) minY = room.Position.Y;
                if (room.Position.X + room.Width > maxX) maxX = room.Position.X + room.Width;
                if (room.Position.Y + room.Height > maxY) maxY = room.Position.Y + room.Height;
            }

            minX -= padding;
            minY -= padding;
            maxX += padding;
            maxY += padding;

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    WorldGen.PlaceTile(x, y, tileType, true, true);
                }
            }
        }

        private static void PlaceDoor(int x, int y, int doorID, int pairedDoorID)
        {
            for (int dx = 0; dx < DoorWidth; dx++)
            {
                for (int dy = 0; dy < DoorHeight; dy++)
                {
                    WorldGen.KillTile(x + dx, y - dy, false, false, true);
                }
            }

            var doorEntity = TileUtils.PlaceTileWithEntity<ArchiveDoor, ArchiveDoor_TE>(x, y);
            if (doorEntity != null)
            {
                doorEntity.DoorID = doorID;
                doorEntity.PairedDoor = pairedDoorID;
                doorEntity.IsLocked = false;
            }
        }
    }
}
