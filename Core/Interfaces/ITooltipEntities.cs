using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using System.Collections.Generic;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Tooltip entities are defined as Projectiles or Buffs that should be described inside the item tooltip.
    /// These are nested inside tooltips and are displayed by a configurable key.
    /// </summary>
    public interface ITooltipEntities
    {
        /// <summary>
        /// Returns a list of tooltip objects to be displayed for this item.
        /// Called during SetDefaults() in GlobalItem.
        /// </summary>
        List<TooltipEntity> TooltipObjects();

        /// <summary>
        /// Optional: Returns additional tooltip lines to be added to the item's tooltip.
        /// These are processed through the text parsing system.
        /// </summary>
        virtual List<string> GetTooltipLines() => new List<string>();

        /// <summary>
        /// Optional: Allows the item to modify its tooltip behavior based on game state.
        /// Called every frame when tooltips are being displayed.
        /// </summary>
        virtual void UpdateTooltips(List<TooltipEntity> tooltips) { }
    }
}