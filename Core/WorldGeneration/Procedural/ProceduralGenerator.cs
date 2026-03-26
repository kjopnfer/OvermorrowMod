using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Tiles.Archives;
using System.Collections.Generic;

namespace OvermorrowMod.Core.WorldGeneration.Procedural
{
    public static class ProceduralGenerator
    {
        /// <summary>
        /// Starting ID offset for procedurally generated doors.
        /// Avoids collision with the existing DoorID enum values.
        /// </summary>
        private const int DoorIDOffset = 1000;

        /// <summary>
        /// Generates a chain of rooms between two points, connected by door pairs.
        /// </summary>
        /// <param name="pointA">Start point in tile coordinates.</param>
        /// <param name="pointB">End point in tile coordinates.</param>
        /// <param name="roomCount">Number of rooms to generate.</param>
        /// <param name="roomWidth">Width of each room in tiles.</param>
        /// <param name="roomHeight">Height of each room in tiles.</param>
        /// <param name="tileType">Tile type for room walls.</param>
        /// <returns>List of generated rooms. First room is at pointA.</returns>
        public static List<ProceduralRoom> Generate(Point pointA, Point pointB, int roomCount, int roomWidth, int roomHeight, int tileType)
        {
            var rooms = new List<ProceduralRoom>();

            for (int i = 0; i < roomCount; i++)
            {
                float t = roomCount <= 1 ? 0f : (float)i / (roomCount - 1);

                // Lerp between A and B to get the center of each room
                int centerX = (int)MathHelper.Lerp(pointA.X, pointB.X, t);
                int centerY = (int)MathHelper.Lerp(pointA.Y, pointB.Y, t);

                // Convert center to top-left position
                Point topLeft = new Point(centerX - roomWidth / 2, centerY - roomHeight / 2);

                var room = new ProceduralRoom(topLeft, roomWidth, roomHeight);
                room.Generate(tileType);
                rooms.Add(room);
            }

            // Place paired doors between consecutive rooms
            PlaceDoors(rooms);

            return rooms;
        }

        private static void PlaceDoors(List<ProceduralRoom> rooms)
        {
            int nextDoorID = DoorIDOffset;

            for (int i = 0; i < rooms.Count - 1; i++)
            {
                var roomA = rooms[i];
                var roomB = rooms[i + 1];

                int doorAID = nextDoorID++;
                int doorBID = nextDoorID++;

                Point doorAPos = roomA.GetDoorPosition(roomB.Center);
                Point doorBPos = roomB.GetDoorPosition(roomA.Center);

                PlaceDoor(doorAPos.X, doorAPos.Y, doorAID, doorBID);
                PlaceDoor(doorBPos.X, doorBPos.Y, doorBID, doorAID);
            }
        }

        private static void PlaceDoor(int x, int y, int doorID, int pairedDoorID)
        {
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
