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
        /// Number of seconds in ticks that it takes before the NPC gets respawned.
        /// </summary>
        public int CooldownTime { get; private set; } = ModUtils.SecondsToTicks(80);

        /// <summary>
        /// Distance in pixels at which NPCs will spawn, i.e., (1 tile = 16 pixels)
        /// </summary>
        private float SpawnDistance = ModUtils.TilesToPixels(120);

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
                //Main.NewText("NPC already spawned: " + ChildNPC.FullName);
                return;
            }

            ChildNPC = NPC.NewNPCDirect(null, Position.ToWorldCoordinates(10, 28), NPCType);

            OvermorrowNPC modNPC = ChildNPC.ModNPC as OvermorrowNPC;
            modNPC.SpawnerID = ID;

            //Main.NewText("spawn " + modNPC.Name + " with id: " + ID);
            HasBeenKilled = false;
        }

        public void SetSpawnerCleared()
        {
            ChildNPC = null;
            HasBeenKilled = true;
            HasBeenCleared = true;

            SpawnerCooldown = CooldownTime;
        }

        /// <summary>
        /// Checks if any player is within spawn distance of this spawn point
        /// </summary>
        /// <returns>True if a player is within 200 tiles (3200 pixels)</returns>
        private bool IsPlayerNearby()
        {
            foreach (var player in Main.player)
            {
                if (player.active)
                {
                    float distance = Vector2.Distance(Position.ToWorldCoordinates(), player.Center);
                    if (distance <= SpawnDistance)
                    {
                        //Main.NewText("position: " + Position.ToWorldCoordinates() + ", vs " + SpawnDistance + " distance: " + distance);
                        return true;
                    }
                }
            }   

            return false;
        }

        public override void Update()
        {
            bool playerNearby = IsPlayerNearby();

            // If the NPC is active but no players are nearby, despawn it
            if (ChildNPC != null && ChildNPC.active)
            {
                if (!playerNearby)
                {
                    ChildNPC.active = false; // Despawn NPC
                    ChildNPC = null;
                    HasBeenKilled = false;   // Don't treat this as a natural death
                                             // No cooldown
                }
            }

            // If the NPC was removed (not killed), clear the reference
            if (ChildNPC != null && !HasBeenKilled && !ChildNPC.active)
            {
                ChildNPC = null;
            }

            // If player is nearby and NPC is not currently active, spawn a new one
            if (playerNearby)
            {
                if (ChildNPC == null && SpawnerCooldown <= 0)
                {
                    SpawnNPC();
                }
            }
            else
            {
                // If no player nearby, decrement cooldown only
                if (SpawnerCooldown > 0)
                    SpawnerCooldown--;
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            return true;
        }
    }
}