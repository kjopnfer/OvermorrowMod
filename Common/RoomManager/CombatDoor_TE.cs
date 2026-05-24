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
    public class CombatDoor_TE : ModTileEntity
    {
        public enum DoorState { Closed, Opening, Open, Closing }

        public bool HasBeenInteracted = false;
        public int SiblingTEID = -1;
        public DoorState State = DoorState.Closed;
        public int StateTimer = 0;

        public bool IsLocked = false;

        // If true, NPC doesnt spawn on rejoin
        public bool IsDisabled = false;
        public bool DisableAfterOpen = false;

        public const int OpenAnimationTicks = 90;
        public const int OpenDurationTicks = 60 * 2;
        public const int CloseAnimationTicks = 60;
        public const int OpenOffsetPixels = 9 * 16;

        private int doorNPCIndex = -1;
        public NPC DoorNPC => doorNPCIndex >= 0 && doorNPCIndex < Main.npc.Length && Main.npc[doorNPCIndex].active
                ? Main.npc[doorNPCIndex]
                : null;

        private static readonly float SpawnDistance = ModUtils.TilesToPixels(100);

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
                    DoorState.Closed => 0f,
                    DoorState.Opening => -OpenOffsetPixels * EasingUtils.EaseInOutQuint(MathHelper.Clamp(StateTimer / (float)OpenAnimationTicks, 0f, 1f)),
                    DoorState.Open => -OpenOffsetPixels,
                    DoorState.Closing => -OpenOffsetPixels * (1f - EasingUtils.EaseInOutQuint(MathHelper.Clamp(StateTimer / (float)CloseAnimationTicks, 0f, 1f))),
                    _ => 0f,
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

        public void BeginVanish()
        {
            IsLocked = false;
            DisableAfterOpen = true;
            if (State != DoorState.Opening && State != DoorState.Open)
            {
                State = DoorState.Opening;
                StateTimer = 0;
            }
        }

        private void SpawnOrchestrator()
        {
            var sibling = Sibling;
            if (sibling == null) return;

            int leftDoorX = System.Math.Min(Position.X, sibling.Position.X);
            int rightDoorX = System.Math.Max(Position.X, sibling.Position.X);
            int leftDoorID = Position.X < sibling.Position.X ? ID : SiblingTEID;
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
                orch.LeftDoorTEID = leftDoorID;
                orch.RightDoorTEID = rightDoorID;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["Sibling"] = SiblingTEID;
            tag["Disabled"] = IsDisabled;
        }

        public override void LoadData(TagCompound tag)
        {
            SiblingTEID = tag.Get<int>("Sibling");
            IsDisabled = tag.Get<bool>("Disabled");
        }

        public override void Update()
        {
            ManageDoorNPC();
            AdvanceStateMachine();
        }

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
                        if (DisableAfterOpen) IsDisabled = true;
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

            if (DoorNPC == null && playerNearby)
                SpawnDoorNPC();
        }

        private void SpawnDoorNPC()
        {
            Vector2 spawnPos = new Vector2(Position.X * 16 + 8, Position.Y * 16 - 128);
            int npcType = ModContent.NPCType<CombatDoorCollision>();
            int idx = NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnPos.X, (int)spawnPos.Y, npcType);
            if (idx < 0 || idx >= Main.npc.Length) return;

            var npc = Main.npc[idx];
            if (npc.ModNPC is CombatDoorCollision door)
            {
                door.ParentDoorTEID = ID;
                door.ClosedY = spawnPos.Y;
            }
            npc.netUpdate = true;
            doorNPCIndex = idx;
        }
    }
}
