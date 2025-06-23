using Microsoft.Xna.Framework;
using OvermorrowMod.Content.Buffs;
using OvermorrowMod.Core.Globals;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Utilities
{
    public static class NPCUtils
    {
        /// <summary>
        /// Moves the NPC toward a target position, applying horizontal and vertical movement, and calculates the absolute horizontal and vertical distances to the target.
        /// </summary>
        /// <param name="npc">The NPC to move.</param>
        /// <param name="targetPosition">The target position the NPC should move toward.</param>
        /// <param name="moveSpeed">The acceleration of the NPC when moving horizontally.</param>
        /// <param name="maxSpeed">The maximum horizontal speed of the NPC.</param>
        /// <param name="jumpSpeed">The vertical speed applied when the NPC needs to jump over obstacles or gaps.</param>
        /// <returns>A <see cref="Vector2"/> representing the absolute horizontal (X) and vertical (Y) distances between the NPC and the target position.</returns>
        public static Vector2 Move(this NPC npc, Vector2 targetPosition, float moveSpeed, float maxSpeed, float jumpSpeed)
        {
            if (npc.Center.X < targetPosition.X)
            {
                npc.velocity.X += moveSpeed;

                if (npc.velocity.X > maxSpeed) npc.velocity.X = maxSpeed;
            }
            else if (npc.Center.X > targetPosition.X)
            {
                npc.velocity.X -= moveSpeed;

                if (npc.velocity.X < -maxSpeed) npc.velocity.X = -maxSpeed;
            }

            if (npc.collideY && npc.velocity.Y == 0)
            {
                Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY, 1, false, 0);

                #region Jump Handling
                if (npc.collideX || npc.CheckGap())
                {
                    npc.velocity.Y -= jumpSpeed;
                }
                #endregion
            }

            // Calculate the absolute horizontal and vertical distances between the NPC and the target position
            float xDistance = Math.Abs(targetPosition.X - npc.Center.X);
            float yDistance = Math.Abs(targetPosition.Y - npc.Center.Y);

            return new Vector2(xDistance, yDistance);
        }

        public static Vector2 MoveCeiling(this NPC npc, Vector2 targetPosition, float moveSpeed, float maxSpeed, float dropSpeed)
        {
            // Horizontal movement (same as normal)
            if (npc.Center.X < targetPosition.X)
            {
                npc.velocity.X += moveSpeed;
                if (npc.velocity.X > maxSpeed)
                    npc.velocity.X = maxSpeed;
            }
            else if (npc.Center.X > targetPosition.X)
            {
                npc.velocity.X -= moveSpeed;
                if (npc.velocity.X < -maxSpeed)
                    npc.velocity.X = -maxSpeed;
            }

            // Ceiling movement (reverse vertical logic)
            if (npc.collideY && npc.velocity.Y < 0)
            {
                //Main.NewText(npc.collideX);


                // You might want to replace this with a ceiling-specific version of StepUp if needed

                // Instead of jumping up, we "drop down" to avoid obstacles or gaps in the ceiling
                if (npc.collideX || npc.CheckCeilingGap())
                {
                    npc.velocity.Y += dropSpeed; // Drop off the ceiling briefly
                }
            }

            // Return horizontal and vertical distance to the target
            float xDistance = Math.Abs(targetPosition.X - npc.Center.X);
            float yDistance = Math.Abs(targetPosition.Y - npc.Center.Y);

            return new Vector2(xDistance, yDistance);
        }

        public static bool CheckCeilingGap(this NPC npc)
        {
            Point topLeft = (npc.TopLeft - new Vector2(0, 8)).ToTileCoordinates();
            Point topRight = (npc.TopRight - new Vector2(0, 8)).ToTileCoordinates();

            // Ensure we check both left and right 2 columns above the NPC
            int tileXStart = Math.Min(topLeft.X, topRight.X);
            int tileXEnd = Math.Max(topLeft.X, topRight.X);

            // Check a 2-tile-high area above the NPC
            for (int x = tileXStart; x <= tileXEnd; x++)
            {
                // Require both tiles above to be non-solid for it to count as a gap
                bool topTile = !WorldGen.SolidTile(x, topLeft.Y - 1);
                bool secondTile = !WorldGen.SolidTile(x, topLeft.Y - 2);

                if (!topTile || !secondTile)
                {
                    // One or both tiles are solid—no gap here
                    return false;
                }
            }

            return true; // All columns above NPC are clear for 2 tiles upward
        }


        public static bool CheckGap(this NPC npc)
        {
            Rectangle npcHitbox = npc.getRect();

            Vector2 checkLeft = new Vector2(npcHitbox.BottomLeft().X, npcHitbox.BottomLeft().Y);
            Vector2 checkRight = new Vector2(npcHitbox.BottomRight().X, npcHitbox.BottomRight().Y);
            Vector2 hitboxDetection = (npc.velocity.X < 0 ? checkLeft : checkRight) / 16;

            int directionOffset = npc.direction;

            Tile tile = Framing.GetTileSafely((int)hitboxDetection.X + directionOffset, (int)hitboxDetection.Y + 1);

            return !tile.HasTile;
        }

        #region Stealth
        /// <summary>
        /// Sets the stealth time and delay for an NPC.
        /// </summary>
        /// <param name="npc">The NPC to set stealth on.</param>
        /// <param name="stealthTime">The duration of the stealth in ticks.</param>
        /// <param name="stealthDelay">The delay before stealth can be applied again in ticks.</param>
        public static void SetStealth(this NPC npc, int stealthTime, int stealthDelay)
        {
            var buffNPC = npc.GetGlobalNPC<BuffNPC>();

            // Check if the stealth delay is still active
            if (buffNPC.StealthDelay > 0)
            {
                return; // Don't apply stealth if the delay is still active
            }

            buffNPC.StealthDelay = stealthDelay;
            npc.AddBuff(ModContent.BuffType<Stealth>(), stealthTime);
        }

        /// <summary>
        /// Removes the Stealth buff from the NPC, if it has it.
        /// </summary>
        /// <param name="npc">The NPC to remove the buff from.</param>
        public static void RemoveStealth(this NPC npc)
        {
            if (npc.IsStealthed())
            {
                int buffIndex = npc.FindBuffIndex(ModContent.BuffType<Stealth>());
                if (buffIndex >= 0)
                {
                    npc.DelBuff(buffIndex);
                }
            }
        }

        /// <summary>
        /// Checks if the NPC is currently stealthed.
        /// </summary>
        /// <param name="npc">The NPC to check.</param>
        /// <returns>True if the NPC is stealthed, otherwise false.</returns>
        public static bool IsStealthed(this NPC npc)
        {
            return npc.HasBuff<Stealth>();
        }

        /// <summary>
        /// Checks if the NPC is on stealth cooldown.
        /// </summary>
        /// <param name="npc">The NPC to check.</param>
        /// <returns>True if the stealth delay is active, otherwise false.</returns>
        public static bool IsStealthOnCooldown(this NPC npc)
        {
            return npc.GetGlobalNPC<BuffNPC>().StealthDelay > 0;
        }
        #endregion

        /// <summary>
        /// Adds a barrier to the NPC, providing temporary protection from damage.
        /// The barrier absorbs incoming damage up to its total Barrier Points (BP).
        /// Any damage exceeding the BP directly reduces the NPC's health.
        /// Barriers decay after a specified duration if not replenished.
        /// </summary>
        /// <param name="npc">The NPC to which the barrier is being applied.</param>
        /// <param name="amount">The total Barrier Points (BP) to assign. This determines how much damage the barrier can absorb.</param>
        /// <param name="duration">The duration of the barrier in ticks (1/60th of a second). After this duration, the barrier will decay.</param>
        public static void AddBarrier(this NPC npc, int amount, int duration)
        {
            BarrierNPC barrierNPC = npc.GetGlobalNPC<BarrierNPC>();
            if (barrierNPC.CanGainBarrier)
                barrierNPC.SetBarrier(amount, duration);
        }
    }
}