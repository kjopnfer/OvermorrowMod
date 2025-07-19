using OvermorrowMod.Common.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Accessories
{
    public class AccessoryPlayer : ModPlayer
    {
        private HashSet<Type> _activeAccessories = new();
        private HashSet<Type> _lastFrameAccessories = new();

        

        public override void ResetEffects()
        {
            // Store what was active last frame before clearing
            _lastFrameAccessories = new HashSet<Type>(_activeAccessories);
            ClearActiveAccessories();
        }

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

        public override void PostUpdateEquips()
        {
            var unequippedAccessories = new HashSet<Type>(_lastFrameAccessories);
            unequippedAccessories.ExceptWith(_activeAccessories);

            // Clean up unequipped accessories
            foreach (var accessoryType in unequippedAccessories)
            {
                // Deactivate the accessory effects
                AccessoryManager.DeactivateAccessoryEffects(Player, accessoryType);

                // Find the instance and reset its variables
                ResetUnequippedAccessoryVariables(accessoryType);
            }
        }

        private void ResetUnequippedAccessoryVariables(Type accessoryType)
        {
            // Search through all inventory slots to find the unequipped accessory
            for (int i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].ModItem?.GetType() == accessoryType &&
                    Player.inventory[i].ModItem is OvermorrowAccessory accessory)
                {
                    accessory.ResetVariables();
                    break;
                }
            }
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj.IsWeaponType(WeaponType.Rapier))
            {
                // Add armor penetration equal to twice the projectile's damage
                modifiers.ArmorPenetration += proj.damage * 2;
            }
        }

        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (drawInfo.shadow == 0)
            {
                Content.Items.Accessories.WarriorsEpic.DrawEffects(Player, drawInfo, ref r, ref g, ref b, ref a, ref fullBright);
            }
        }
    }
}