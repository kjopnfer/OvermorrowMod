using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items
{
    /// <summary>
    /// Central tooltip system for managing tooltip display and coordination
    /// </summary>
    public class TooltipSystem : ModSystem
    {
        public static Keys TooltipKey { get; set; } = Keys.LeftShift;

        /// <summary>
        /// Checks if the tooltip key is currently pressed
        /// </summary>
        public static bool IsTooltipKeyPressed => Main.keyState.IsKeyDown(TooltipKey);

        /// <summary>
        /// Gets tooltip entities from an item if it implements the interface
        /// </summary>
        public static List<TooltipEntity> GetTooltipEntities(Item item)
        {
            if (item.ModItem is ITooltipEntities tooltipProvider)
            {
                var entities = tooltipProvider.TooltipObjects();
                tooltipProvider.UpdateTooltips(entities);
                return entities;
            }
            return new List<TooltipEntity>();
        }

        /// <summary>
        /// Gets tooltip lines from an item if it implements the interface
        /// </summary>
        public static List<string> GetTooltipLines(Item item)
        {
            if (item.ModItem is ITooltipEntities tooltipProvider)
            {
                return tooltipProvider.GetTooltipLines();
            }
            return new List<string>();
        }
    }
}