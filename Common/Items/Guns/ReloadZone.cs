using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items.Guns
{
    /// <summary>
    /// Defines a clickable zone in the reload bar used for the reloading mechanic.
    /// </summary>
    public class ReloadZone
    {
        /// <summary>
        /// The starting percentage position of the zone (0-100).
        /// </summary>
        public int StartPercentage = 0;

        /// <summary>
        /// The ending percentage position of the zone (0-100).
        /// </summary>
        public int EndPercentage = 0;

        /// <summary>
        /// Whether the player has successfully clicked in this zone during the current reload.
        /// </summary>
        public bool HasClicked { get; set; } = false;

        /// <summary>
        /// Creates a new reload zone with specified start and end positions.
        /// </summary>
        /// <param name="startPercentage">Starting point of the zone as a percentage (0-100)</param>
        /// <param name="endPercentage">Ending point of the zone as a percentage (0-100)</param>
        public ReloadZone(int startPercentage, int endPercentage)
        {
            this.StartPercentage = startPercentage;
            this.EndPercentage = endPercentage;
        }
    }
}