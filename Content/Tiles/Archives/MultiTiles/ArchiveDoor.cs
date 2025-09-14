using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Items.Archives;
using OvermorrowMod.Content.Misc;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace OvermorrowMod.Content.Tiles.Archives
{
    public class ArchiveDoor : ModTile
    {
        public override string Texture => AssetDirectory.ArchiveTiles + Name;

        public override bool CanExplode(int i, int j) => false;
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            TileObjectData.newTile.Width = 12;
            TileObjectData.newTile.Height = 15;
            TileObjectData.newTile.CoordinateHeights = Enumerable.Repeat(16, TileObjectData.newTile.Height).ToArray();

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Origin = new Point16(0, 14);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;

            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(24, 21, 18), name);
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.player[Main.myPlayer];
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ArchiveKey>();

            base.MouseOver(i, j);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            // Remove the NPC spawning logic from here - it's now handled in the TileEntity
        }

        public override bool RightClick(int i, int j)
        {
            ArchiveDoor_TE door;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<ArchiveDoor_TE>(bottomLeft.X, bottomLeft.Y, out door);
            if (door != null)
            {
                door.Interact();
            }

            return base.RightClick(i, j);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {

            Tile tile = Framing.GetTileSafely(i, j);
            ArchiveDoor_TE door;
            Point bottomLeft = TileUtils.GetCornerOfMultiTile(i, j, TileUtils.CornerType.BottomLeft);
            TileUtils.TryFindModTileEntity<ArchiveDoor_TE>(bottomLeft.X, bottomLeft.Y, out door);

            var tileSize = 18;
            var numTilesX = 12;
            var framePixelsX = (numTilesX - 1) * tileSize;

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.ArchiveTiles + Name + "Animated").Value;
            var offset = 270 * (door.DoorFrame - 1);
            for (int xFrame = 0; xFrame <= 198; xFrame += tileSize)
            {
                // Loop through all possible frame positions for y (0 to 240) in increments of 18
                for (int yFrame = 0; yFrame <= 258; yFrame += tileSize)
                {
                    // Only draw frames that match the current TileFrameX and TileFrameY
                    if (tile.TileFrameX == xFrame && tile.TileFrameY == yFrame)
                    {
                        // Calculate the rectangle for the current tile frame
                        Rectangle drawRectangle = new Rectangle(xFrame, offset + (yFrame /* door.DoorFrame*/), 16, 16);

                        // Off-screen range for drawing optimization
                        Vector2 offScreenRange = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
                        Vector2 drawPos = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreenRange;

                        // Draw the tile using spriteBatch
                        spriteBatch.Draw(texture, drawPos, drawRectangle, Lighting.GetColor(i, j), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
                    }
                }
            }

            return false;
        }
    }

    public class ArchiveDoor_TE : ModTileEntity
    {
        public int DoorID;
        public int PairedDoor;

        public bool IsLocked = false;

        // NPC reference
        private int lockNPCIndex = -1;
        public NPC LockNPC => lockNPCIndex >= 0 && lockNPCIndex < Main.npc.Length && Main.npc[lockNPCIndex].active ? Main.npc[lockNPCIndex] : null;

        private int FrameCounter = 0;
        public int DoorFrame = 1; // Goes from frame 0 to 6
        public Vector2 DoorPosition => Position.ToWorldCoordinates(16, 16);

        public override void SaveData(TagCompound tag)
        {
            tag["DoorID"] = DoorID;
            tag["PairedDoor"] = PairedDoor;
            tag["IsLocked"] = IsLocked;
        }

        public override void LoadData(TagCompound tag)
        {
            DoorID = tag.Get<int>("DoorID");
            PairedDoor = tag.Get<int>("PairedDoor");
            IsLocked = tag.Get<bool>("IsLocked");
        }

        public void UnlockDoors()
        {
            var matchingDoor = ByID.Values
                                    .OfType<ArchiveDoor_TE>()
                                    .FirstOrDefault(door => door.PairedDoor == DoorID && PairedDoor == door.DoorID);

            IsLocked = false;
            matchingDoor.IsLocked = false;
        }

        public void Interact()
        {
            // Try to find the matching paired door by DoorID and PairedDoor
            var matchingDoor = ByID.Values
                                    .OfType<ArchiveDoor_TE>()
                                    .FirstOrDefault(door => door.PairedDoor == DoorID && PairedDoor == door.DoorID);

            // If a matching door is found, teleport the player to it
            if (matchingDoor != null)
            {
                if (IsLocked)
                {
                    // Check if player has ArchiveKey in their inventory
                    bool hasKey = false;
                    Player localPlayer = Main.LocalPlayer;

                    for (int i = 0; i < localPlayer.inventory.Length; i++)
                    {
                        if (localPlayer.inventory[i].type == ModContent.ItemType<ArchiveKey>())
                        {
                            hasKey = true;
                            break;
                        }
                    }

                    if (hasKey)
                    {
                        // Find the key item and reduce its stack by 1
                        for (int i = 0; i < localPlayer.inventory.Length; i++)
                        {
                            if (localPlayer.inventory[i].type == ModContent.ItemType<ArchiveKey>())
                            {
                                localPlayer.inventory[i].stack--;
                                if (localPlayer.inventory[i].stack <= 0)
                                {
                                    localPlayer.inventory[i].TurnToAir();
                                }
                                break;
                            }
                        }

                        // Start the death animation directly on the lock NPC
                        if (LockNPC?.ModNPC is DoorLock doorLock)
                        {
                            SoundEngine.PlaySound(SoundID.Unlock, doorLock.NPC.Center);
                            doorLock.StartDeathAnimation();
                        }
                    }
                    else
                    {
                        Main.NewText("The door is locked!", Color.Red);
                    }
                    return;
                }

                Vector2 teleportOffset = new Vector2(64, -32);
                Main.LocalPlayer.Teleport(matchingDoor.Position.ToWorldCoordinates(16, 16) + teleportOffset, -1);
                matchingDoor.DoorFrame = 6;

            }
        }

        /// <summary>
        /// Spawns the lock NPC if the door is locked and no NPC exists
        /// </summary>
        private void ManageLockNPC()
        {
            if (IsLocked)
            {
                // Handle forceful despawning - if we have an NPC reference but it's no longer active
                if (lockNPCIndex >= 0 && (lockNPCIndex >= Main.npc.Length || !Main.npc[lockNPCIndex].active))
                {
                    //Main.NewText("Lock NPC was forcefully despawned, clearing reference");
                    lockNPCIndex = -1;
                }

                // Check if we need to spawn a new NPC
                if (LockNPC == null)
                {
                    // Check if any player is within range (100 tiles)
                    bool playerNearby = false;
                    Vector2 doorCenter = DoorPosition + new Vector2(ModUtils.TilesToPixels(6), ModUtils.TilesToPixels(7.5f)); // Center of door

                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        if (Main.player[i].active)
                        {
                            float distance = Vector2.Distance(Main.player[i].Center, doorCenter);
                            if (distance <= ModUtils.TilesToPixels(100))
                            {
                                playerNearby = true;
                                break;
                            }
                        }
                    }

                    if (playerNearby)
                    {
                        SpawnLockNPC();
                    }
                }
                else
                {
                    // Ensure the existing NPC has the correct reference
                    if (LockNPC.ModNPC is DoorLock doorLock)
                    {
                        doorLock.tileEntity = this;
                    }
                }
            }
            else
            {
                // Door is not locked, remove NPC if it exists
                if (LockNPC != null)
                {
                    RemoveLockNPC();
                }
            }
        }

        /// <summary>
        /// Spawns the lock NPC at the door position
        /// </summary>
        private void SpawnLockNPC()
        {
            var npcType = ModContent.NPCType<DoorLock>();
            Vector2 spawnPosition = DoorPosition + new Vector2(ModUtils.TilesToPixels(5), -ModUtils.TilesToPixels(4.2f));

            int npcIndex = NPC.NewNPC(new EntitySource_WorldEvent(), (int)spawnPosition.X, (int)spawnPosition.Y, npcType);

            if (npcIndex >= 0 && Main.npc[npcIndex].ModNPC is DoorLock doorLock)
            {
                lockNPCIndex = npcIndex;
                doorLock.tileEntity = this;
            }
        }

        /// <summary>
        /// Removes the lock NPC
        /// </summary>
        private void RemoveLockNPC()
        {
            if (LockNPC != null)
            {
                LockNPC.active = false;
            }
            lockNPCIndex = -1;
        }

        public override void Update()
        {
            // Manage the lock NPC
            ManageLockNPC();

            // Note: The validation is now handled inside ManageLockNPC() for better organization

            if (IsLocked) return;

            Vector2 playerPosition = Main.LocalPlayer.Center;
            float distance = Vector2.Distance(playerPosition, DoorPosition + new Vector2(75, 0));

            if (distance <= ModUtils.TilesToPixels(6))
            {
                if (DoorFrame < 7)
                {
                    FrameCounter++;
                    if (FrameCounter >= 3)
                    {
                        DoorFrame++;
                        FrameCounter = 0;
                    }
                }
            }
            else
            {
                if (DoorFrame > 1)
                {
                    FrameCounter++;
                    if (FrameCounter >= 3)
                    {
                        DoorFrame--;
                        FrameCounter = 0;
                    }
                }
            }
        }

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            if (!tile.HasTile || tile.TileType != ModContent.TileType<ArchiveDoor>())
            {
                // Clean up NPC before killing the tile entity
                RemoveLockNPC();
                Kill(Position.X, Position.Y);
            }

            return tile.HasTile && tile.TileType == ModContent.TileType<ArchiveDoor>();
        }
    }
}