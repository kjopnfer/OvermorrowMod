using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.ModularGeneration
{
    /// <summary>
    /// Static class that manages a list of rooms and generates tunnels between them.
    /// </summary>
    public static class ModularRoomGenerator
    {
        // The list of rooms
        private static List<Room> rooms = new List<Room>();
        private static TunnelType tunnelType = TunnelType.Linear;

        private static Boolean debugging = false;

        private static Room currentRoom;

        /// <summary>
        /// Adds a room to the list and specifies the generation function for the room.
        /// </summary>
        /// <param name="room">The room to add.</param>
        /// <param name="code">The generation function for the room.</param>
        public static void AddRoom(Room room, Room.GenerateRoom code)
        {
            room.code = code;
            rooms.Add(room);
        }

        /// <summary>
        /// Sets the type of the tunnel.
        /// </summary>
        /// <param name="Type">The type of the tunnel to set.</param>
        public static void SetTunnelType(TunnelType Type)
        {
            tunnelType = Type;
        }

        /// <summary>
        /// Enables or disables debugging mode.
        /// </summary>
        /// <param name="debug">True to enable debugging, false to disable it.</param>
        public static void SetDebugging(bool debug)
        {
            debugging = debug;
        }

        /// <summary>
        /// Runs the generation function for each room in the list and generates tunnels between them.
        /// </summary>
        public static void Run()
        {
            Vector2? exit = null;

            for (int i = 0; i < rooms.Count; i++)
            {
                Room room = rooms[i];
                room.code(room.Entrance);
                currentRoom = room;
                if (exit != null)
                {
                    CallTunnel(exit);
                    Tunnel.PlaceTiles(room.BlockType);
                }
                exit = room.Exit;
                if (debugging)
                {
                    room.RoomID = i;
                }
            }
        }


        /// <summary>
        /// Searches through all added rooms for a room with a matching ID.
        /// </summary>
        /// <param name="id">The ID of the room to retrieve.</param>
        /// <returns>The room with the specified ID.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if no room with the specified ID has been added.
        /// </exception>
        public static Room GetRoomFromID(int id)
        {
            foreach (Room room in rooms)
            {
                if (room.RoomID == id)
                {
                    return room;
                }
            }
            throw new KeyNotFoundException("ID not possessed by any added rooms");
        }


        /// <summary>
        /// Places a tile at the specified position in the world. If debugging is on, remembers where they were placed.
        /// </summary>
        /// <param name="pos">The position at which to place the tile.</param>
        /// <param name="tileType">The type of tile to place.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Thrown if the tile position is outside the bounds of the world.
        /// </exception>
        public static void PlaceTile(Vector2 pos, int tileType)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X > Main.maxTilesX || pos.Y > Main.maxTilesY)
            {
                throw new IndexOutOfRangeException("Tile Position outside World");
            }
            if (debugging)
            {
                currentRoom.Tiles.Add(pos);
                currentRoom.TileIDs.Add(Framing.GetTileSafely(pos).TileType);
            }
            WorldGen.PlaceTile((int)pos.X, (int)pos.Y, tileType);
        }


        /// <summary>
        /// Calls the appropriate tunnel calculation method based on the specified tunnel type.
        /// </summary>
        /// <param name="exit">The optional exit point for the tunnel. If not specified, the entrance of the room is used as the exit point.</param>
        /// <exception cref="NotImplementedException">Thrown if the tunnel type is not yet implemented.</exception>
        public static void CallTunnel(Vector2? exit)
        {
            switch (tunnelType)
            {
                case TunnelType.Linear:
                    {
                        Tunnel.Calculate(exit ?? Vector2.Zero,
                         currentRoom.Entrance ?? Vector2.Zero,
                         currentRoom.offset);
                        break;
                    }
                case TunnelType.Curve:
                    {
                        throw new NotImplementedException("This Tunnel Type is not yet implemented.");
                    }
                case TunnelType.PerpendicularLines:
                    {
                        throw new NotImplementedException("This Tunnel Type is not yet implemented.");
                    }
            }
        }


        /// <summary>
        /// Removes a room from the world by replacing its tiles with their original tiles.
        /// </summary>
        /// <param name="room">The room to remove.</param>
        public static void RemoveRoom(Room room)
        {
            for (int i = 0; i < room.Tiles.Count; i++)
            {
                Vector2 pos = room.Tiles[i];
                WorldGen.KillTile((int)pos.X, (int)pos.Y);
                WorldGen.PlaceTile((int)pos.X, (int)pos.Y, room.TileIDs[i]);
            }
        }


        ///  <summary>
        ///This method clears the rooms list of all its elements.
        ///</summary>
        public static void Clear()
        {
            rooms.Clear();
        }
    }


}