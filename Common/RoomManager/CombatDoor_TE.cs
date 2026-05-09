using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Misc;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace OvermorrowMod.Common.RoomManager
{
    /// <summary>
    /// Tile entity placed at the bottom-left and bottom-right corners of a
    /// CombatRoom (one column outside the room footprint, on the floor row).
    /// Owns the CombatDoorCollision NPC: spawns it on demand when a player
    /// is nearby, despawns it when the player leaves range, and persists
    /// interaction state across saves.
    /// <para/>
    /// The two doors that bracket a CombatRoom are linked via SiblingTEID.
    /// Opening one opens the other in lockstep (Open() syncs the sibling),
    /// so the room's left and right doors always share state.
    /// </summary>
    public class CombatDoor_TE : ModTileEntity
    {
        public enum DoorState { Closed, Opening, Open, Closing }

        public bool HasBeenInteracted = false;
        public int SiblingTEID = -1;
        public DoorState State = DoorState.Closed;
        public int StateTimer = 0;

        // Set by CombatOrchestrator. Locked = Open() is a no-op (auto-close
        // still runs). Disabled = NPC despawned permanently, persisted.
        public bool IsLocked = false;
        public bool IsDisabled = false;

        public const int OpenAnimationTicks  = 90;          // 1.5s slide up
        public const int OpenDurationTicks   = 60 * 2;      // 2s open
        public const int CloseAnimationTicks = 60;          // 1s slide down
        public const int OpenOffsetPixels    = 9 * 16;      // 9 tiles up

        private int doorNPCIndex = -1;
        public NPC DoorNPC =>
            doorNPCIndex >= 0 && doorNPCIndex < Main.npc.Length && Main.npc[doorNPCIndex].active
                ? Main.npc[doorNPCIndex]
                : null;

        private static readonly float SpawnDistance = ModUtils.TilesToPixels(100);

        /// <summary>The other door bracketed by this combat room, or null.</summary>
        public CombatDoor_TE Sibling
        {
            get
            {
                if (SiblingTEID < 0) return null;
                if (!TileEntity.ByID.TryGetValue(SiblingTEID, out var te)) return null;
                return te as CombatDoor_TE;
            }
        }

        public float YOffsetPixels
        {
            get
            {
                return State switch
                {
                    DoorState.Closed   => 0f,
                    DoorState.Opening  => -OpenOffsetPixels * EasingUtils.EaseInOutQuint(MathHelper.Clamp(StateTimer / (float)OpenAnimationTicks, 0f, 1f)),
                    DoorState.Open     => -OpenOffsetPixels,
                    DoorState.Closing  => -OpenOffsetPixels * (1f - EasingUtils.EaseInOutQuint(MathHelper.Clamp(StateTimer / (float)CloseAnimationTicks, 0f, 1f))),
                    _                  => 0f,
                };
            }
        }

        /// <summary>True while the door is fully closed and collision should block the player.</summary>
        public bool IsBlocking => State == DoorState.Closed;

        /// <summary>
        /// Begins the open animation and syncs the sibling. On the first
        /// interaction in the pair, spawns the CombatOrchestrator.
        /// </summary>
        public void Open()
        {
            if (IsLocked || IsDisabled) return;
            if (State != DoorState.Closed) return;

            // Latch before mutating so simultaneous clicks on left+right
            // can only spawn the orchestrator once.
            bool firstInteractionInPair =
                !HasBeenInteracted && (Sibling == null || !Sibling.HasBeenInteracted);

            State = DoorState.Opening;
            StateTimer = 0;
            HasBeenInteracted = true;

            var sibling = Sibling;
            if (sibling != null && sibling.State == DoorState.Closed)
            {
                sibling.State = DoorState.Opening;
                sibling.StateTimer = 0;
                sibling.HasBeenInteracted = true;
            }

            if (firstInteractionInPair)
                SpawnOrchestrator();
        }

        private void SpawnOrchestrator()
        {
            var sibling = Sibling;
            if (sibling == null) return;

            int leftDoorX  = System.Math.Min(Position.X, sibling.Position.X);
            int rightDoorX = System.Math.Max(Position.X, sibling.Position.X);
            int leftDoorID  = Position.X < sibling.Position.X ? ID : SiblingTEID;
            int rightDoorID = Position.X < sibling.Position.X ? SiblingTEID : ID;

            int midTileX = (leftDoorX + rightDoorX) / 2;
            int floorRow = Position.Y;

            // 6 tiles above the floor surface so the chest has fall distance.
            const int OrchestratorTilesAboveFloor = 6;
            int worldX = midTileX * 16 + 8;
            int worldY = (floorRow + 1) * 16 - OrchestratorTilesAboveFloor * 16;

            int npcType = ModContent.NPCType<CombatOrchestrator>();
            int idx = NPC.NewNPC(new EntitySource_WorldEvent(), worldX, worldY, npcType);

            if (idx >= 0 && idx < Main.npc.Length
                && Main.npc[idx].ModNPC is CombatOrchestrator orch)
            {
                orch.LeftDoorTEID  = leftDoorID;
                orch.RightDoorTEID = rightDoorID;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Interacted"]  = HasBeenInteracted;
            tag["Sibling"]     = SiblingTEID;
            tag["State"]       = (int)State;
            tag["StateTimer"]  = StateTimer;
            tag["Locked"]      = IsLocked;
            tag["Disabled"]    = IsDisabled;
        }

        public override void LoadData(TagCompound tag)
        {
            HasBeenInteracted = tag.Get<bool>("Interacted");
            SiblingTEID       = tag.Get<int>("Sibling");
            State             = (DoorState)tag.Get<int>("State");
            StateTimer        = tag.Get<int>("StateTimer");
            IsLocked          = tag.Get<bool>("Locked");
            IsDisabled        = tag.Get<bool>("Disabled");
        }

        public override void Update()
        {
            ManageDoorNPC();
            AdvanceStateMachine();
        }

        /// <summary>The TE accepts placement anywhere; no underlying tile required.</summary>
        public override bool IsTileValidForEntity(int x, int y) => true;

        private void AdvanceStateMachine()
        {
            switch (State)
            {
                case DoorState.Opening:
                    StateTimer++;
                    if (StateTimer >= OpenAnimationTicks)
                    {
                        State = DoorState.Open;
                        StateTimer = 0;
                    }
                    break;
                case DoorState.Open:
                    StateTimer++;
                    if (StateTimer >= OpenDurationTicks)
                    {
                        State = DoorState.Closing;
                        StateTimer = 0;
                        var sibling = Sibling;
                        if (sibling != null && sibling.State == DoorState.Open)
                        {
                            sibling.State = DoorState.Closing;
                            sibling.StateTimer = 0;
                        }
                    }
                    break;
                case DoorState.Closing:
                    StateTimer++;
                    if (StateTimer >= CloseAnimationTicks)
                    {
                        State = DoorState.Closed;
                        StateTimer = 0;
                    }
                    break;
            }
        }

        private void ManageDoorNPC()
        {
            if (doorNPCIndex >= 0 && (doorNPCIndex >= Main.npc.Length || !Main.npc[doorNPCIndex].active))
                doorNPCIndex = -1;

            // Disabled = combat cleared, door is gone for good.
            if (IsDisabled)
            {
                if (DoorNPC != null) DoorNPC.active = false;
                doorNPCIndex = -1;
                return;
            }

            Vector2 doorCenter = Position.ToWorldCoordinates(8, -ModUtils.TilesToPixels(4));
            bool playerNearby = Main.LocalPlayer.active
                && Vector2.Distance(Main.LocalPlayer.Center, doorCenter) <= SpawnDistance;

            if (DoorNPC == null)
            {
                if (playerNearby) SpawnDoorNPC();
            }
            else if (DoorNPC.ModNPC is CombatDoorCollision door)
            {
                door.tileEntity = this;
            }
        }

        private void SpawnDoorNPC()
        {
            // Bottom-center of the TE's tile so the 1x9 NPC sits on the floor.
            Vector2 spawnPos = Position.ToWorldCoordinates(8, 16);
            int npcType = ModContent.NPCType<CombatDoorCollision>();
            int idx = NPC.NewNPC(new EntitySource_WorldEvent(),
                                 (int)spawnPos.X, (int)spawnPos.Y, npcType);

            if (idx >= 0 && idx < Main.npc.Length && Main.npc[idx].ModNPC is CombatDoorCollision door)
            {
                doorNPCIndex = idx;
                door.tileEntity = this;
            }
        }
    }
}
