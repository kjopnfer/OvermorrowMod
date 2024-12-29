using Microsoft.Xna.Framework;
using System;
using Terraria;

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
    }
}