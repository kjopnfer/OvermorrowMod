using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Defines a custom aggro range shape with individual radius values for each direction.
    /// Supports directional awareness and can represent circles, ovals, or any asymmetric shape.
    /// </summary>
    public class AggroRadius
    {
        /// <summary>
        /// Radius extending to the right of the NPC's center (in pixels).
        /// </summary>
        public float Right { get; set; }

        /// <summary>
        /// Radius extending to the left of the NPC's center (in pixels).
        /// </summary>
        public float Left { get; set; }

        /// <summary>
        /// Radius extending upward from the NPC's center (in pixels).
        /// </summary>
        public float Up { get; set; }

        /// <summary>
        /// Radius extending downward from the NPC's center (in pixels).
        /// </summary>
        public float Down { get; set; }

        /// <summary>
        /// Whether this radius should flip left/right values based on NPC facing direction.
        /// When true: NPC facing right uses Right for front, Left for back.
        /// When NPC facing left: uses Left for front, Right for back.
        /// </summary>
        public bool FlipWithDirection { get; set; }

        /// <summary>
        /// Creates an AggroRadius with individual values for each direction.
        /// </summary>
        /// <param name="right">Radius extending to the right</param>
        /// <param name="left">Radius extending to the left</param>
        /// <param name="up">Radius extending upward</param>
        /// <param name="down">Radius extending downward</param>
        /// <param name="flipWithDirection">Whether to flip left/right with NPC direction</param>
        public AggroRadius(float right, float left, float up, float down, bool flipWithDirection = false)
        {
            Right = right;
            Left = left;
            Up = up;
            Down = down;
            FlipWithDirection = flipWithDirection;
        }

        /// <summary>
        /// Creates a circular aggro radius.
        /// </summary>
        /// <param name="radius">The radius in all directions</param>
        /// <param name="flipWithDirection">Whether to flip left/right with NPC direction</param>
        public static AggroRadius Circle(float radius, bool flipWithDirection = false)
        {
            return new AggroRadius(radius, radius, radius, radius, flipWithDirection);
        }

        /// <summary>
        /// Creates an oval aggro radius.
        /// </summary>
        /// <param name="horizontal">Radius extending left and right</param>
        /// <param name="vertical">Radius extending up and down</param>
        /// <param name="flipWithDirection">Whether to flip left/right with NPC direction</param>
        public static AggroRadius Oval(float horizontal, float vertical, bool flipWithDirection = false)
        {
            return new AggroRadius(horizontal, horizontal, vertical, vertical, flipWithDirection);
        }

        /// <summary>
        /// Creates a line-of-sight style radius that's longer in front than behind.
        /// </summary>
        /// <param name="front">Radius in the direction the NPC is facing</param>
        /// <param name="back">Radius behind the NPC</param>
        /// <param name="vertical">Radius up and down</param>
        public static AggroRadius LineOfSight(float front, float back, float vertical)
        {
            return new AggroRadius(front, back, vertical, vertical, true);
        }

        /// <summary>
        /// Checks if a point is within this aggro radius relative to the NPC's center.
        /// </summary>
        /// <param name="npcCenter">The center position of the NPC</param>
        /// <param name="targetPoint">The point to check</param>
        /// <param name="npcDirection">The NPC's facing direction (1 for right, -1 for left)</param>
        /// <returns>True if the point is within the aggro radius</returns>
        public bool IsPointInRange(Vector2 npcCenter, Vector2 targetPoint, int npcDirection = 1)
        {
            Vector2 offset = targetPoint - npcCenter;

            // Get the effective radii based on direction
            float effectiveRight, effectiveLeft;
            if (FlipWithDirection && npcDirection == -1)
            {
                // NPC facing left, so flip the left/right radii
                effectiveRight = Left;
                effectiveLeft = Right;
            }
            else
            {
                // NPC facing right or no flipping
                effectiveRight = Right;
                effectiveLeft = Left;
            }

            // Calculate the normalized position within the ellipse
            float normalizedX, normalizedY;

            if (offset.X >= 0)
            {
                // Point is to the right
                normalizedX = Math.Abs(offset.X) / effectiveRight;
            }
            else
            {
                // Point is to the left
                normalizedX = Math.Abs(offset.X) / effectiveLeft;
            }

            if (offset.Y >= 0)
            {
                // Point is below
                normalizedY = Math.Abs(offset.Y) / Down;
            }
            else
            {
                // Point is above
                normalizedY = Math.Abs(offset.Y) / Up;
            }

            // Check if the point is within the ellipse (normalized distance <= 1)
            return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1.0f;
        }

        /// <summary>
        /// Gets the maximum radius in any direction for this aggro shape.
        /// Useful for broad-phase collision detection.
        /// </summary>
        /// <returns>The largest radius value</returns>
        public float GetMaxRadius()
        {
            return Math.Max(Math.Max(Right, Left), Math.Max(Up, Down));
        }

        /// <summary>
        /// Gets the effective radius in a specific direction from the NPC center.
        /// </summary>
        /// <param name="direction">Normalized direction vector</param>
        /// <param name="npcDirection">The NPC's facing direction (1 for right, -1 for left)</param>
        /// <returns>The radius in that direction</returns>
        public float GetRadiusInDirection(Vector2 direction, int npcDirection = 1)
        {
            if (direction == Vector2.Zero)
                return 0f;

            direction.Normalize();

            // Get effective radii
            float effectiveRight, effectiveLeft;
            if (FlipWithDirection && npcDirection == -1)
            {
                effectiveRight = Left;
                effectiveLeft = Right;
            }
            else
            {
                effectiveRight = Right;
                effectiveLeft = Left;
            }

            // Calculate angle and determine which quadrant
            float angle = (float)Math.Atan2(direction.Y, direction.X);

            // Normalize angle to 0-2*PI
            if (angle < 0)
                angle += MathHelper.TwoPi;

            // Determine the radius based on angle
            float horizontalRadius = direction.X >= 0 ? effectiveRight : effectiveLeft;
            float verticalRadius = direction.Y >= 0 ? Down : Up;

            // Calculate the radius at this angle using ellipse formula
            float cosAngle = Math.Abs((float)Math.Cos(angle));
            float sinAngle = Math.Abs((float)Math.Sin(angle));

            // Ellipse radius formula: r = ab / sqrt((b*cos(angle))^2 + (a*sin(angle))^2)
            float denominator = (float)Math.Sqrt(
                (verticalRadius * cosAngle) * (verticalRadius * cosAngle) +
                (horizontalRadius * sinAngle) * (horizontalRadius * sinAngle)
            );

            return denominator > 0 ? (horizontalRadius * verticalRadius) / denominator : 0f;
        }

        /// <summary>
        /// Creates a copy of this AggroRadius.
        /// </summary>
        /// <returns>A new AggroRadius with the same values</returns>
        public AggroRadius Clone()
        {
            return new AggroRadius(Right, Left, Up, Down, FlipWithDirection);
        }
    }
}