using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace OvermorrowMod.ModularGeneration
{

    ///<summary>
    ///A struct reperesenting a room to be generated.
    ///</summary>
    public struct Room
    {
        /// <summary>
        /// The type of block to generate for this room.
        /// </summary>
        public int BlockType;

        /// <summary>
        /// The type of room to generate. (NOT IMPLEMENTED)
        /// </summary>

#nullable enable
        public int? RoomType;

        /// <summary>
        /// The function used to generate the room. This should take a Vector2 as its entrance point.
        /// </summary>
        public delegate void GenerateRoom(Vector2? entrance);


        /// <summary>
        /// The function used to generate the room. This should take a Vector2 as its entrance point.
        /// </summary>
        public GenerateRoom? code;

#nullable disable

        /// <summary>
        /// The height of the tunnel leading to this room.
        /// </summary>
        public float offset;

        /// <summary>
        /// The entrance point for this room.
        /// </summary>
        public Vector2? Entrance;

        /// <summary>
        /// The exit point for this room.
        /// </summary>
        public Vector2? Exit;


        /// <summary>
        /// The ID of the room after generation, only populated if debugging is on.
        /// </summary>
        public int RoomID;


        /// <summary>
        /// The location of every tile in the room, used for removing the room, only populated if debugging is on.
        /// </summary>
        public List<Vector2> Tiles;

        /// <summary>
        /// The TileID of every tile in the room, used for removing the room, only populated if debugging is on.
        /// </summary>
        public List<int> TileIDs;
    }

}