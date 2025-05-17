using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Common.Utilities
{
    /// <summary>
    /// General utilies that are not restricted to a particular type.
    /// </summary>
    public static class ModUtils
    {
        /// <summary>
        /// Determines the direction (-1 or 1) of the target entity relative to the source entity.
        /// Returns 1 if the target is to the right, -1 if to the left.
        /// </summary>
        /// <param name="source">The entity determining the direction.</param>
        /// <param name="target">The entity whose position is compared.</param>
        /// <returns>-1 if the target is to the left, 1 if to the right.</returns>
        public static int GetDirection(this Entity source, Entity target)
        {
            return target.Center.X > source.Center.X ? 1 : -1;
        }

        /// <summary>
        /// Determines the direction (-1 or 1) of a target position relative to the source entity.
        /// Returns 1 if the position is to the right, -1 if to the left.
        /// </summary>
        /// <param name="source">The entity determining the direction.</param>
        /// <param name="position">The world position to compare.</param>
        /// <returns>-1 if the position is to the left, 1 if to the right.</returns>
        public static int GetDirection(this Entity source, Vector2 position)
        {
            return position.X > source.Center.X ? 1 : -1;
        }

        /// <summary>
        /// Converts seconds to ticks.
        /// </summary>
        /// <param name="seconds">The number of seconds.</param>
        /// <returns>The equivalent number of ticks.</returns>
        public static int SecondsToTicks(float seconds)
        {
            return (int)(seconds * 60f);
        }
    }
}