using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Core.NPCs;
using System;
using Terraria;

namespace OvermorrowMod.Content.NPCs
{
    public class Wander : BaseIdleState
    {
        public override int Weight => 1;
        public override bool CanExit => true;

        /// <summary>
        /// Minimum range of tiles that the NPC must choose to wander from its spawn point.
        /// </summary>
        public int MinWanderRange { get; private set; }

        /// <summary>
        /// Maximum range of tiles that the NPC can wander from its spawn point.
        /// </summary>
        public int MaxWanderRange { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="npc"></param>
        /// <param name="minRange">The minimum range in tiles that the NPC must choose to wander from.</param>
        /// <param name="maxRange">The maxmimum range in tiles that the NPC can choose to wander to.</param>
        public Wander(OvermorrowNPC npc, int minRange = 4, int maxRange = 10) : base(npc)
        {
            MinWanderRange = minRange;
            MaxWanderRange = maxRange;
        }

        public override void Enter()
        {
            IsFinished = false;
            NPC.velocity.X = 0;
            if (OvermorrowNPC.SpawnPoint != null)
            {
                Vector2 spawnPosition = OvermorrowNPC.SpawnPoint.Position.ToWorldCoordinates();

                // Start from max range and work down to min range
                for (int range = MaxWanderRange; range >= MinWanderRange; range--)
                {
                    if (TryFindValidPosition(spawnPosition, range, out Vector2 validPosition))
                    {
                        OvermorrowNPC.TargetingModule.MiscTargetPosition = validPosition;
                        return;
                    }
                }

                // If min range still didn't work, try fractional partitions of min range
                float[] fractions = { 0.75f, 0.5f, 0.25f, 0.1f };
                foreach (float fraction in fractions)
                {
                    int fractionalRange = Math.Max(1, (int)(MinWanderRange * fraction));
                    if (TryFindValidPosition(spawnPosition, fractionalRange, out Vector2 validPosition))
                    {
                        OvermorrowNPC.TargetingModule.MiscTargetPosition = validPosition;
                        return;
                    }
                }

                // No valid position found at any range
                IsFinished = true;
                OvermorrowNPC.IdleCounter = Main.rand.Next(30, 60);
            }
            else
            {
                if (OvermorrowNPC.SpawnerID.HasValue)
                {
                }
                else
                {
                }
            }
        }

        private bool TryFindValidPosition(Vector2 spawnPos, int range, out Vector2 position)
        {
            for (int attempts = 0; attempts < 8; attempts++)
            {
                float offsetX = ModUtils.TilesToPixels(Main.rand.Next(-range, range));
                float offsetY = ModUtils.TilesToPixels(Main.rand.Next(-range, range));

                position = spawnPos + new Vector2(offsetX, offsetY);
                position = TileUtils.FindNearestGround(position);

                // For the emergency ranges (below MinWanderRange), skip the distance check
                bool meetsDistanceRequirement = range < MinWanderRange ||
                                               Vector2.Distance(NPC.Center, position) >= ModUtils.TilesToPixels(MinWanderRange);

                if (meetsDistanceRequirement && IsGroundPathClear(NPC.Center, position))
                {
                    return true;
                }
            }

            position = Vector2.Zero;
            return false;
        }

        /// <summary>
        /// Checks if there's a walkable path along the ground between two positions
        /// </summary>
        /// <param name="from">Starting position</param>
        /// <param name="to">Target position</param>
        /// <returns>True if the path is walkable</returns>
        private bool IsGroundPathClear(Vector2 from, Vector2 to)
        {
            int steps = (int)(Math.Abs(to.X - from.X) / 16f); // Check every tile
            if (steps == 0) return true;

            Vector2 fromGround = TileUtils.FindNearestGround(from);
            Vector2 toGround = TileUtils.FindNearestGround(to);

            for (int i = 0; i <= steps; i++)
            {
                float progress = (float)i / steps;
                Vector2 checkPos = Vector2.Lerp(from, to, progress);
                Vector2 groundPos = TileUtils.FindNearestGround(checkPos);

                // Instead of checking against the lerped position, check against reasonable movement limits
                Vector2 expectedPos = Vector2.Lerp(fromGround, toGround, progress);
                float heightDiff = Math.Abs(groundPos.Y - expectedPos.Y);

                // Allow more tolerance for drops (NPCs can fall) vs climbs
                float maxClimb = ModUtils.TilesToPixels(3);  // Can climb up 3 tiles
                float maxDrop = ModUtils.TilesToPixels(10);  // Can drop down 10 tiles

                float verticalChange = groundPos.Y - expectedPos.Y;
                if (verticalChange < -maxClimb || verticalChange > maxDrop)
                    return false;
            }

            return true;
        }

        public override void Exit()
        {
            OvermorrowNPC.AICounter = 0;
            OvermorrowNPC.TargetingModule.MiscTargetPosition = null;
        }

        public override void Update()
        {
            NPC.noGravity = false;

            // TODO: Change this to an NPC properties module.
            float maxSpeed = 1.8f;
            if (OvermorrowNPC.SpawnPoint != null)
            {
                if (OvermorrowNPC.TargetingModule.MiscTargetPosition.HasValue)
                {
                    Vector2 targetPosition = OvermorrowNPC.TargetingModule.MiscTargetPosition.Value;
                    NPC.direction = NPC.GetDirection(targetPosition);
                    Vector2 distance = NPC.Move(targetPosition, 0.2f, maxSpeed, 8f);

                    if (distance.X <= ModUtils.TilesToPixels(1))
                    {
                        if (!IsFinished) // Prevent setting multiple times
                        {
                            OvermorrowNPC.IdleCounter = Main.rand.Next(12, 15) * 10;
                            IsFinished = true;
                            NPC.velocity.X = 0;
                        }
                    }
                }
            }
        }
    }
}