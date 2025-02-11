using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.RoomManager
{
    public class Room
    {
        public List<NPCSpawnPoint> SpawnPoints { get; private set; } = new();
        public Room() { }

        /// <summary>
        /// Create a new ModTileEntity that spawns a particular NPC at the position.
        /// </summary>
        /// <param name="position">The tile coordinates that the ModTileEntity is created.</param>
        /// <param name="npcType">The ID of the NPC that is spawned by the ModTileEntity</param>
        public void AddSpawnPoint(Vector2 position, int npcType)
        {
            int entityID = ModContent.GetInstance<NPCSpawnPoint>().Place((int)position.X, (int)position.Y);
            NPCSpawnPoint spawnPoint = TileEntity.ByID[entityID] as NPCSpawnPoint;
            spawnPoint.NPCType = npcType;

            SpawnPoints.Add(spawnPoint);
        }

        /// <summary>
        /// Calls all SpawnPoints and respawns their associated NPCs.
        /// </summary>
        public void RespawnNPCs()
        {
            if (HasRoomBeenCleared())
            {
                foreach (NPCSpawnPoint spawn in SpawnPoints)
                    spawn.SpawnNPC();
            }
        }

        /// <summary>
        /// Loops through all of the SpawnPoints and checks if their NPC is active.
        /// </summary>
        public bool HasRoomBeenCleared()
        {
            foreach (NPCSpawnPoint spawnPoint in SpawnPoints)
            {
                if (spawnPoint.ChildNPC.active)
                    return false;
            }

            return true;
        }
    }
}