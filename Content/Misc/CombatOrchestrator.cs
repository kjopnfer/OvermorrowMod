using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Loot;
using OvermorrowMod.Core.Particles;
using OvermorrowMod.Core.WorldGeneration;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    public class CombatOrchestrator : RewardChest
    {
        public enum Phase { Setup, Combat, End }

        public int LeftDoorTEID = -1;
        public int RightDoorTEID = -1;

        private Phase phase = Phase.Setup;
        private readonly List<int> spawnedNPCs = new();

        private const int TotalWaves = 2;
        private int currentWave = 0;
        private int waveDelayTimer = 0;
        private const int WaveDelayTicks = 90;
        private const float WaveBlasterBookChance = 0.35f;

        private const int TelegraphDuration = 70;
        private int telegraphTimer = 0;
        private readonly List<PendingSpawn> pendingSpawns = new();

        private struct PendingSpawn
        {
            public int Type;
            public int TileX;
            public int FloorRow;
        }

        private bool endTriggered = false;
        private bool dedupeChecked = false;

        private bool capturedAnchor = false;
        private Vector2 anchorPosition;

        private const int MinTilesFromDoor = 4;

        private static readonly RarityModifier RewardModifier = new(rare: 5);

        private CombatDoor_TE LeftDoor => LookupDoor(LeftDoorTEID);
        private CombatDoor_TE RightDoor => LookupDoor(RightDoorTEID);

        private static CombatDoor_TE LookupDoor(int id)
        {
            if (id < 0) return null;
            return TileEntity.ByID.TryGetValue(id, out var te) ? te as CombatDoor_TE : null;
        }

        public override void AI()
        {
            NPC.ShowNameOnHover = false;

            if (UpdateCleanup()) return;

            if (!capturedAnchor)
            {
                anchorPosition = NPC.position;
                capturedAnchor = true;
            }

            // Drops chest
            if (!isRevealed)
            {
                NPC.position = anchorPosition;
                NPC.velocity = Vector2.Zero;
            }

            if (!dedupeChecked)
            {
                dedupeChecked = true;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    var other = Main.npc[i];
                    if (i == NPC.whoAmI || !other.active) continue;
                    if (other.ModNPC is CombatOrchestrator orch
                        && orch.LeftDoorTEID == LeftDoorTEID
                        && LeftDoorTEID >= 0)
                    {
                        NPC.active = false;
                        return;
                    }
                }
            }

            var left = LeftDoor;
            var right = RightDoor;
            if (left == null || right == null)
            {
                NPC.active = false;
                return;
            }

            switch (phase)
            {
                case Phase.Setup:
                    UpdateSetup(left, right);
                    break;
                case Phase.Combat:
                    UpdateCombat();
                    break;
                case Phase.End:
                    UpdateOpening();
                    break;
            }

            UpdateChest();
        }

        protected override int[] RollRewards() => RollOffers(RewardModifier, default);

        private void UpdateSetup(CombatDoor_TE left, CombatDoor_TE right)
        {
            bool bothClosed = left.State == CombatDoor_TE.DoorState.Closed &&
                right.State == CombatDoor_TE.DoorState.Closed;

            if (!bothClosed)
                return;

            left.IsLocked = true;
            right.IsLocked = true;

            SpawnWave();
            IgniteRoomLights(left, right);
            phase = Phase.Combat;
        }

        private static void IgniteRoomLights(CombatDoor_TE left, CombatDoor_TE right)
        {
            int leftX = System.Math.Min(left.Position.X, right.Position.X);
            int rightX = System.Math.Max(left.Position.X, right.Position.X);
            int top = left.Position.Y - ArchiveLights.CombatRoomHeight;

            var roomRect = new Rectangle(leftX, top, rightX - leftX, ArchiveLights.CombatRoomHeight);
            ArchiveLights.SetRegion(roomRect, 1f, ModUtils.SecondsToTicks(1.5f));
        }

        private void UpdateCombat()
        {
            if (pendingSpawns.Count > 0)
            {
                if (--telegraphTimer > 0) return;
                ResolvePendingSpawns();
                return;
            }

            int aliveCount = 0;
            Player target = null;
            for (int i = 0; i < spawnedNPCs.Count; i++)
            {
                int idx = spawnedNPCs[i];
                if (idx < 0 || idx >= Main.npc.Length) continue;

                NPC enemy = Main.npc[idx];
                if (!enemy.active) continue;
                if (enemy.ModNPC is not OvermorrowNPC modNPC || modNPC.CombatOrchestratorWhoAmI != NPC.whoAmI) continue;

                aliveCount++;

                if (!modNPC.TargetingModule.HasTarget())
                {
                    target ??= FindCombatTarget();
                    if (target != null) modNPC.TargetingModule.SetTarget(target);
                }
            }

            if (aliveCount > 0)
            {
                waveDelayTimer = 0;
                return;
            }

            if (currentWave < TotalWaves)
            {
                if (++waveDelayTimer < WaveDelayTicks) return;

                SpawnWave();
                return;
            }

            EnterEnd();
            phase = Phase.End;
        }

        private void SpawnWave()
        {
            var left = LeftDoor;
            var right = RightDoor;
            if (left == null || right == null) return;

            currentWave++;
            waveDelayTimer = 0;
            spawnedNPCs.Clear();
            pendingSpawns.Clear();

            int leftDoorX = System.Math.Min(left.Position.X, right.Position.X);
            int rightDoorX = System.Math.Max(left.Position.X, right.Position.X);
            int floorRow = left.Position.Y;
            int midX = (leftDoorX + rightDoorX) / 2;

            int leftMinX = leftDoorX + MinTilesFromDoor;
            int leftMaxX = System.Math.Max(leftMinX, midX - 1);
            int rightMaxX = rightDoorX - MinTilesFromDoor;
            int rightMinX = System.Math.Min(rightMaxX, midX);

            int ratAX = Main.rand.Next(leftMinX, leftMaxX + 1);
            int ratBX = Main.rand.Next(rightMinX, rightMaxX + 1);

            QueueSpawn(PickWaveEnemy(), ratAX, floorRow, 1);
            QueueSpawn(PickWaveEnemy(), ratBX, floorRow, -1);

            telegraphTimer = TelegraphDuration;
        }

        private static int PickWaveEnemy()
        {
            if (Main.rand.NextFloat() < WaveBlasterBookChance)
                return ModContent.NPCType<BlasterBook>();
            return ModContent.NPCType<ArchiveRat>();
        }

        private const int TelegraphCenterOffsetY = 20;
        private void QueueSpawn(int npcType, int tileX, int floorRow, int spinDirection)
        {
            pendingSpawns.Add(new PendingSpawn { Type = npcType, TileX = tileX, FloorRow = floorRow });

            int worldX = tileX * 16 + 8;
            int worldYBottom = (floorRow + 1) * 16;
            float telegraphY = worldYBottom - TelegraphCenterOffsetY;
            if (SpawnsFlying(npcType))
                telegraphY -= FlyingSpawnHeightTiles * 16;
            Vector2 telegraphCenter = new Vector2(worldX, telegraphY);

            Projectile.NewProjectile(NPC.GetSource_FromAI(), telegraphCenter, Vector2.Zero, ModContent.ProjectileType<SpawnTelegraph>(), 0, 0f, Main.myPlayer, spinDirection, TelegraphDuration);
        }

        private static bool SpawnsFlying(int npcType) => ContentSamples.NpcsByNetId[npcType].noGravity;

        private void ResolvePendingSpawns()
        {
            Player target = FindCombatTarget();
            foreach (PendingSpawn spawn in pendingSpawns)
                SpawnEnemy(spawn.Type, spawn.TileX, spawn.FloorRow, target);

            pendingSpawns.Clear();
        }

        // Tiles above the floor that gravity-less enemies are lifted to when spawned.
        private const int FlyingSpawnHeightTiles = 6;

        private void SpawnEnemy(int npcType, int tileX, int floorRow, Player target)
        {
            int worldX = tileX * 16 + 8;
            int worldYBottom = (floorRow + 1) * 16;

            int index = NPC.NewNPC(NPC.GetSource_FromAI(), worldX, worldYBottom, npcType);
            if (index < 0 || index >= Main.npc.Length) return;

            NPC enemy = Main.npc[index];
            Vector2 anchor = new Vector2(worldX, worldYBottom);

            if (enemy.noGravity)
                enemy.position.Y -= FlyingSpawnHeightTiles * 16;

            if (enemy.ModNPC is OvermorrowNPC modNPC)
            {
                modNPC.AnchorPosition = anchor;
                modNPC.CombatOrchestratorWhoAmI = NPC.whoAmI;
                if (target != null) modNPC.TargetingModule.SetTarget(target);

                if (modNPC is LivingGrimoire grimoire)
                    grimoire.RevealAndFly();
            }

            spawnedNPCs.Add(index);
            SpawnRedBurst(enemy.Center);
        }

        private Player FindCombatTarget()
        {
            Player nearest = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active || player.dead) continue;

                float distance = Vector2.DistanceSquared(player.Center, NPC.Center);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player;
                }
            }

            return nearest;
        }

        private static void SpawnRedBurst(Vector2 position)
        {
            Texture2D circleTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D sparkTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            var pulse = new Circle(circleTex, ModUtils.SecondsToTicks(0.4f), canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(pulse, position, Vector2.Zero, Color.Red, 1f, 0.6f, 0f, useAdditiveBlending: true);

            for (int i = 0; i < 14; i++)
            {
                Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 6f);
                float scale = Main.rand.NextFloat(2f, 5f);
                var spark = new Spark(sparkTex, 0f, true, 0f) { endColor = Color.DarkRed };
                ParticleManager.CreateParticleDirect(spark, position, vel, Color.Red, 1f, scale, 0f, useAdditiveBlending: true);
            }
        }

        private void EnterEnd()
        {
            if (endTriggered) return;
            endTriggered = true;

            Main.NewText("Room cleared!", Color.Gold);
            SubworldTimer.AddTime(30);

            Texture2D circleTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D pulseTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "pulse", AssetRequestMode.ImmediateLoad).Value;
            Texture2D sparkTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            var bigPulse = new Circle(pulseTex, ModUtils.SecondsToTicks(0.9f), canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(bigPulse, NPC.Center, Vector2.Zero, Color.Gold, 1f, 1.2f, 0f, useAdditiveBlending: true);

            var innerPulse = new Circle(circleTex, ModUtils.SecondsToTicks(0.6f), canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(innerPulse, NPC.Center, Vector2.Zero, Color.LightGoldenrodYellow, 1f, 0.7f, 0f, useAdditiveBlending: true);

            for (int i = 0; i < 22; i++)
            {
                Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(4f, 8f);
                float scale = Main.rand.NextFloat(2f, 6f);
                var spark = new Spark(sparkTex, 0f, true, 0f) { endColor = Color.Goldenrod };
                ParticleManager.CreateParticleDirect(spark, NPC.Center, vel, Color.Gold, 1f, scale, 0f, useAdditiveBlending: true);
            }

            LeftDoor?.BeginVanish();
            RightDoor?.BeginVanish();

            // Reveal the chest and allow for gravity
            isRevealed = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }
    }
}
