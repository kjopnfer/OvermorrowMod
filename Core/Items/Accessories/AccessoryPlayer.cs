using OvermorrowMod.Common.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Accessories
{
    public class AccessoryPlayer : ModPlayer
    {
        private HashSet<Type> _activeAccessories = new();

        /// <summary>
        /// Activates an accessory for this player using type
        /// </summary>
        public void ActivateAccessory<T>() where T : OvermorrowAccessory
        {
            _activeAccessories.Add(typeof(T));
        }

        /// <summary>
        /// Activates an accessory for this player using type
        /// </summary>
        public void ActivateAccessory(Type accessoryType)
        {
            if (accessoryType.IsSubclassOf(typeof(OvermorrowAccessory)))
            {
                _activeAccessories.Add(accessoryType);
            }
        }

        /// <summary>
        /// Deactivates an accessory for this player using type
        /// </summary>
        public void DeactivateAccessory<T>() where T : OvermorrowAccessory
        {
            _activeAccessories.Remove(typeof(T));
        }

        /// <summary>
        /// Deactivates an accessory for this player using type
        /// </summary>
        public void DeactivateAccessory(Type accessoryType)
        {
            _activeAccessories.Remove(accessoryType);
        }

        /// <summary>
        /// Checks if this player has a specific accessory active using type
        /// </summary>
        public bool HasAccessory<T>() where T : OvermorrowAccessory
        {
            return _activeAccessories.Contains(typeof(T));
        }

        /// <summary>
        /// Checks if this player has a specific accessory active using type
        /// </summary>
        public bool HasAccessory(Type accessoryType)
        {
            return _activeAccessories.Contains(accessoryType);
        }

        /// <summary>
        /// Clears all active accessories for this player
        /// </summary>
        public void ClearActiveAccessories()
        {
            _activeAccessories.Clear();
        }

        /// <summary>
        /// Gets all active accessories for this player
        /// </summary>
        public IReadOnlyCollection<Type> GetActiveAccessories()
        {
            return _activeAccessories;
        }

        public override void ResetEffects()
        {
            ClearActiveAccessories();
        }
    }
}