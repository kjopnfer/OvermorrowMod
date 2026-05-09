using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.RoomManager;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.NPCs.Archives;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Misc
{
    /// <summary>
    /// Drives a single combat-room encounter from setup to reward.
    /// Spawned by a CombatDoor_TE on first interaction (singleton-per-room
    /// enforced on the door side). Information flow is one-way: the
    /// orchestrator writes IsLocked / IsDisabled onto the two door TEs;
    /// the doors never read from or hold a reference to the orchestrator.
    /// <para/>
    /// State machine:
    ///   Setup  : invisible, hovering. Waits for both doors to finish
    ///            their entry animation, then locks them and spawns rats.
    ///   Combat : invisible, hovering. Polls the spawn list; when all
    ///            enemies are inactive, transition to End.
    ///   End    : becomes a visible reward chest, drops under gravity,
    ///            spawns landing particles on impact, and waits for the
    ///            player to right-click to open.
    /// </summary>
    public class CombatOrchestrator : ModNPC
    {
        public enum Phase { Setup, Combat, End }

        public int LeftDoorTEID = -1;
        public int RightDoorTEID = -1;

        private Phase phase = Phase.Setup;
        private readonly List<int> spawnedNPCs = new();

        private bool endTriggered = false;
        private bool dedupeChecked = false;

        private bool capturedAnchor = false;
        private Vector2 anchorPosition;

        // EnterEnd flips isRevealed; ground contact flips hasImpacted;
        // right-click flips isOpen.
        public bool isRevealed = false;
        public bool hasImpacted = false;
        public bool isOpen = false;

        // Right-click → countdown → ramping wobble → isOpen.
        public int openingBuildupTimer = 0;
        private const int OpeningBuildupDuration = 90; // 1.5 s

        private const int MinTilesFromDoor = 4;

        public override string Texture => AssetDirectory.Misc + "RewardChest";

        private CombatDoor_TE LeftDoor => LookupDoor(LeftDoorTEID);
        private CombatDoor_TE RightDoor => LookupDoor(RightDoorTEID);

        private static CombatDoor_TE LookupDoor(int id)
        {
            if (id < 0) return null;
            return TileEntity.ByID.TryGetValue(id, out var te) ? te as CombatDoor_TE : null;
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.aiStyle = -1;
            NPC.lifeMax = 1;
            NPC.life = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.friendly = true;
            NPC.dontCountMe = true;
            NPC.HitSound = null;
            NPC.DeathSound = null;
        }

        public override bool CheckActive() => false;

        public override void AI()
        {
            NPC.ShowNameOnHover = false;

            if (!capturedAnchor)
            {
                anchorPosition = NPC.position;
                capturedAnchor = true;
            }

            // Pin until End; after that, gravity drops the chest.
            if (!isRevealed)
            {
                NPC.position = anchorPosition;
                NPC.velocity = Vector2.Zero;
            }

            // Singleton-per-room dedupe, runs once.
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
                case Phase.Setup: UpdateSetup(left, right); break;
                case Phase.Combat: UpdateCombat(); break;
                case Phase.End: UpdateEnd(); break;
            }

            if (isRevealed && !hasImpacted && NPC.collideY)
            {
                SpawnImpactParticles();
                hasImpacted = true;
            }

            EmitRays();

            // Gold glow, brighter as the buildup ramps.
            if (isRevealed)
            {
                float intensity = 0.6f;
                if (openingBuildupTimer > 0)
                {
                    float progress = (OpeningBuildupDuration - openingBuildupTimer)
                                     / (float)OpeningBuildupDuration;
                    intensity = MathHelper.Lerp(0.6f, 1.2f, progress);
                }
                Lighting.AddLight(NPC.Center, Color.Gold.ToVector3() * intensity);
            }
        }

        private void UpdateSetup(CombatDoor_TE left, CombatDoor_TE right)
        {
            bool bothClosed =
                left.State == CombatDoor_TE.DoorState.Closed &&
                right.State == CombatDoor_TE.DoorState.Closed;
            if (!bothClosed) return;

            left.IsLocked = true;
            right.IsLocked = true;

            SpawnFirstWave(left, right);
            phase = Phase.Combat;
        }

        private void UpdateCombat()
        {
            bool anyAlive = false;
            for (int i = 0; i < spawnedNPCs.Count; i++)
            {
                int idx = spawnedNPCs[i];
                if (idx >= 0 && idx < Main.npc.Length && Main.npc[idx].active)
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive)
            {
                EnterEnd();
                phase = Phase.End;
            }
        }

        private void UpdateEnd()
        {
            if (!hasImpacted) return;

            const float InteractRangeTiles = 7f;
            float interactRange = InteractRangeTiles * 16f;
            bool playerInRange = Main.LocalPlayer.active
                && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) <= interactRange;

            if (!playerInRange)
            {
                isOpen = false;
                openingBuildupTimer = 0;
                return;
            }

            // Buildup commits once started: ticks regardless of mouse position.
            if (openingBuildupTimer > 0)
            {
                openingBuildupTimer--;
                if (openingBuildupTimer == 0) isOpen = true;
                return;
            }

            if (!NPC.Hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y)) return;

            Main.LocalPlayer.cursorItemIconEnabled = true;
            Main.LocalPlayer.cursorItemIconID = ItemID.GoldChest;
            Main.LocalPlayer.noThrow = 2;

            if (Main.mouseRight && Main.mouseRightRelease && !isOpen)
                openingBuildupTimer = OpeningBuildupDuration;
        }

        private void SpawnFirstWave(CombatDoor_TE left, CombatDoor_TE right)
        {
            int leftDoorX = System.Math.Min(left.Position.X, right.Position.X);
            int rightDoorX = System.Math.Max(left.Position.X, right.Position.X);
            int floorRow = left.Position.Y;
            int midX = (leftDoorX + rightDoorX) / 2;

            // Half-of-room ranges, clamped so a narrow room doesn't invert.
            int leftMinX = leftDoorX + MinTilesFromDoor;
            int leftMaxX = System.Math.Max(leftMinX, midX - 1);
            int rightMaxX = rightDoorX - MinTilesFromDoor;
            int rightMinX = System.Math.Min(rightMaxX, midX);

            int ratAX = Main.rand.Next(leftMinX, leftMaxX + 1);
            int ratBX = Main.rand.Next(rightMinX, rightMaxX + 1);

            int worldYBottom = (floorRow + 1) * 16;
            int ratType = ModContent.NPCType<ArchiveRat>();

            int a = NPC.NewNPC(NPC.GetSource_FromAI(), ratAX * 16 + 8, worldYBottom, ratType);
            int b = NPC.NewNPC(NPC.GetSource_FromAI(), ratBX * 16 + 8, worldYBottom, ratType);

            if (a >= 0 && a < Main.npc.Length)
            {
                spawnedNPCs.Add(a);
                SpawnRedBurst(Main.npc[a].Center);
            }
            if (b >= 0 && b < Main.npc.Length)
            {
                spawnedNPCs.Add(b);
                SpawnRedBurst(Main.npc[b].Center);
            }
        }

        /// <summary>Red pop at an enemy spawn point: pulse + radiating sparks.</summary>
        private static void SpawnRedBurst(Vector2 position)
        {
            Texture2D circleTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "circle_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D sparkTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            var pulse = new Circle(circleTex, ModUtils.SecondsToTicks(0.4f),
                                   canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(pulse, position, Vector2.Zero,
                Color.Red, 1f, 0.6f, 0f, useAdditiveBlending: true);

            for (int i = 0; i < 14; i++)
            {
                Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi)
                              * Main.rand.NextFloat(3f, 6f);
                float scale = Main.rand.NextFloat(2f, 5f);
                var spark = new Spark(sparkTex, 0f, true, 0f) { endColor = Color.DarkRed };
                ParticleManager.CreateParticleDirect(spark, position, vel,
                    Color.Red, 1f, scale, 0f, useAdditiveBlending: true);
            }
        }

        /// <summary>Run-once End entry: text, gold burst, disable doors, reveal chest.</summary>
        private void EnterEnd()
        {
            if (endTriggered) return;
            endTriggered = true;

            Main.NewText("Room cleared!", Color.Gold);

            Texture2D circleTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "circle_01", AssetRequestMode.ImmediateLoad).Value;
            Texture2D pulseTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "pulse", AssetRequestMode.ImmediateLoad).Value;
            Texture2D sparkTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            var bigPulse = new Circle(pulseTex, ModUtils.SecondsToTicks(0.9f),
                                      canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(bigPulse, NPC.Center, Vector2.Zero,
                Color.Gold, 1f, 1.2f, 0f, useAdditiveBlending: true);

            var innerPulse = new Circle(circleTex, ModUtils.SecondsToTicks(0.6f),
                                        canGrow: true, useSineFade: true)
            {
                fadeIn = false,
                floatUp = false,
                doWaveMotion = false,
                rotationAmount = 0.05f,
            };
            ParticleManager.CreateParticleDirect(innerPulse, NPC.Center, Vector2.Zero,
                Color.LightGoldenrodYellow, 1f, 0.7f, 0f, useAdditiveBlending: true);

            for (int i = 0; i < 22; i++)
            {
                Vector2 vel = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi)
                              * Main.rand.NextFloat(4f, 8f);
                float scale = Main.rand.NextFloat(2f, 6f);
                var spark = new Spark(sparkTex, 0f, true, 0f) { endColor = Color.Goldenrod };
                ParticleManager.CreateParticleDirect(spark, NPC.Center, vel,
                    Color.Gold, 1f, scale, 0f, useAdditiveBlending: true);
            }

            var left = LeftDoor;
            var right = RightDoor;
            if (left != null) { left.IsDisabled = true; left.IsLocked = false; }
            if (right != null) { right.IsDisabled = true; right.IsLocked = false; }

            // Reveal the chest and hand it to physics. AI() spawns
            // impact particles when collideY flips true.
            isRevealed = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        private void SpawnImpactParticles()
        {
            Texture2D sparkTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;

            int count = Main.rand.Next(12, 19);
            for (int i = 0; i < count; i++)
            {
                float scale = Main.rand.NextFloat(2f, 7f);
                float angle = Main.rand.NextFloat(MathHelper.ToRadians(-15f), MathHelper.ToRadians(15f));
                if (Main.rand.NextBool()) angle += MathHelper.Pi;
                Vector2 vel = Vector2.UnitX.RotatedBy(angle) * Main.rand.Next(2, 12);

                var spark = new Spark(sparkTex, maxTime: Main.rand.Next(15, 30), true, 0f)
                {
                    endColor = Color.Goldenrod,
                };
                ParticleManager.CreateParticleDirect(spark, NPC.Bottom, vel,
                    Color.Gold, 1f, scale, 0f,
                    ParticleDrawLayer.BehindProjectiles, useAdditiveBlending: true);
            }
        }

        int frame = 0;
        int frameCount = 0;

        // Idle wobble: damped jolt then rest, new params each episode.
        public int wobbleTimer = 0;
        public int wobbleActiveTicks = 0;
        public int wobbleRestTicks = 0;
        public float wobbleStrength = 0f;
        public int wobbleDirection = 1;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!isRevealed) return false;

            // Open snaps fast (3 ticks/step), close eases slowly (8 ticks/step).
            int targetFrame = isOpen ? 2 : 0;
            int frameInterval = (frame < targetFrame) ? 3 : 8;
            frameCount++;
            if (frameCount >= frameInterval)
            {
                frameCount = 0;
                if (frame < targetFrame) frame++;
                else if (frame > targetFrame) frame--;
            }

            // Buildup wobble: amplitude and frequency both ramp up.
            if (openingBuildupTimer > 0)
            {
                int elapsed = OpeningBuildupDuration - openingBuildupTimer;
                float progress = elapsed / (float)OpeningBuildupDuration;
                float strength = MathHelper.Lerp(0.02f, 0.18f, progress);
                float freq = MathHelper.Lerp(0.3f, 1.0f, progress);
                NPC.rotation = (float)System.Math.Sin(elapsed * freq) * strength;
                wobbleTimer = 0;
            }
            // Idle wobble while landed and fully closed.
            else if (hasImpacted && !isOpen && frame == 0)
            {
                if (wobbleTimer == 0)
                {
                    wobbleActiveTicks = Main.rand.Next(18, 30);
                    wobbleRestTicks = Main.rand.Next(30, 120);
                    wobbleStrength = Main.rand.NextFloat(0.08f, 0.15f);
                    wobbleDirection = Main.rand.NextBool() ? 1 : -1;
                }

                if (wobbleTimer < wobbleActiveTicks)
                {
                    float damping = 1f - (wobbleTimer / (float)wobbleActiveTicks);
                    NPC.rotation = (float)System.Math.Sin(wobbleTimer * 0.5f)
                                   * wobbleStrength * damping * wobbleDirection;
                }
                else
                {
                    NPC.rotation = 0f;
                }

                wobbleTimer++;
                if (wobbleTimer >= wobbleActiveTicks + wobbleRestTicks)
                    wobbleTimer = 0;
            }
            else
            {
                NPC.rotation = 0f;
                wobbleTimer = 0;
            }

            Texture2D tex = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad).Value;
            var drawRectangle = new Rectangle(0, frame * 36, 32, 36);
            spriteBatch.Draw(tex, NPC.Center - screenPos, drawRectangle, drawColor, NPC.rotation, drawRectangle.Size() / 2f, 1f, SpriteEffects.None, 0f);
            return false;
        }

        /// <summary>God-ray Light particles fanning behind the chest, every 40 ticks once landed.</summary>
        private void EmitRays()
        {
            if (Main.gamePaused) return;
            if (!hasImpacted) return;

            NPC.localAI[0]++;
            if (NPC.localAI[0] % 40 != 0) return;

            Texture2D rayTex = ModContent.Request<Texture2D>(
                AssetDirectory.Textures + "ray", AssetRequestMode.ImmediateLoad).Value;

            int count = Main.rand.Next(3, 5);
            for (int i = 0; i < count; i++)
            {
                var ray = new Light(rayTex, ModUtils.SecondsToTicks(5), NPC, Vector2.Zero);
                float rotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                float scale = Main.rand.NextFloat(0.05f, 0.065f);

                ParticleManager.CreateParticleDirect(ray, NPC.Center , Vector2.Zero,
                    Color.Gold, 1f, scale, rotation,
                    ParticleDrawLayer.BehindNPCs, useAdditiveBlending: true);
            }
        }
    }
}
