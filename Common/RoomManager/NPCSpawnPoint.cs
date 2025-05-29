using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.RoomManager
{
    public class NPCSpawnPoint : ModTileEntity
    {
        /// <summary>
        /// The room that this spawn point is associated with.
        /// </summary>
        public Room Room { get; set; }

        /// <summary>
        /// The NPC that this spawn point should create.
        /// </summary>
        public int NPCType { get; set; }

        /// <summary>
        /// The NPC that this spawner is keeping track of.
        /// </summary>
        public NPC ChildNPC { get; private set; } = null;

        /// <summary>
        /// This is set to true if the associated NPC has been killed.
        /// Being cleared is a one time flag, and should only be set the first time their NPC has died.
        /// It is separate from checking for if the NPC has been killed to respawn it.
        /// </summary>
        public bool HasBeenCleared { get; private set; } = false;

        /// <summary>
        /// This is set to true if the associated NPC has been killed.
        /// This should be set by the NPC's kill hook if they have been killed naturally (i.e., not despawned)
        /// </summary>
        public bool HasBeenKilled { get; set; } = false;

        /// <summary>
        /// Active counter of the Spawner after the NPC has been cleared.
        /// </summary>
        public int SpawnerCooldown { get; private set; } = 0;

        /// <summary>
        /// Number of ticks it takes before the NPC gets respawned.
        /// </summary>
        public int CooldownTime { get; private set; } = ModUtils.SecondsToTicks(120);

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
        }

        public void SpawnNPC()
        {
            if (ChildNPC != null && ChildNPC.active)
            {
                Main.NewText("NPC already spawned: " + ChildNPC.FullName);
                return;
            }

            ChildNPC = NPC.NewNPCDirect(null, Position.ToWorldCoordinates(10, 28), NPCType);

            OvermorrowNPC modNPC = ChildNPC.ModNPC as OvermorrowNPC;
            modNPC.SpawnerID = ID;

            Main.NewText("spawn " + modNPC.Name + " with id: " + ID);
            HasBeenKilled = false;
        }

        public void SetSpawnerCleared()
        {
            ChildNPC = null;
            HasBeenKilled = true;
            HasBeenCleared = true;

            SpawnerCooldown = CooldownTime;
        }

        public override void Update()
        {
            // For if the NPC has been somehow manually despawned without being killed.
            if (ChildNPC != null)
            {
                if (!HasBeenKilled && !ChildNPC.active)
                {
                    Main.NewText("not active");
                    ChildNPC = null;
                }
            }

            // Check if there are any players within a radius of 50 blocks (800 pixels)
            bool playerNearby = false;
            foreach (var player in Main.player)
            {
                if (player.active)
                {
                    float distance = Vector2.Distance(Position.ToWorldCoordinates(), player.Center);
                    if (distance <= 800f) // 50 blocks = 800 pixels
                    {
                        playerNearby = true;
                        break;
                    }
                }
            }

            if (playerNearby)
            {
                // Do something if a player is nearby (e.g., activate or spawn NPC)
            }
            else
            {
                // The NPC is not allowed to respawn unless the Player is offscreen
                if (SpawnerCooldown > 0) SpawnerCooldown--;
            }

            base.Update();
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return true;
        }
    }
}