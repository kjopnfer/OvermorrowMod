using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Accessories
{
    public class AccessoryManager : ModSystem
    {
        private static List<AccessoryDefinition> _allAccessories = new();
        private static Dictionary<int, Dictionary<Type, List<AccessoryEffect>>> _playerActiveEffects = new();

        public override void PostSetupContent()
        {
            base.PostSetupContent();
        }

        private static void TriggerKeywordEffects(Type keywordType, Player player, params object[] args)
        {
            if (!_playerActiveEffects.TryGetValue(player.whoAmI, out var playerEffects))
                return;

            if (!playerEffects.TryGetValue(keywordType, out var effects))
                return;

            // Only iterate through effects for accessories this player actually has
            foreach (var effect in effects)
            {
                effect.TryExecute(player, args);
            }
        }

        public static void RegisterAccessory(AccessoryDefinition definition)
        {
            _allAccessories.Add(definition);
        }

        // Called when player equips an accessory
        public static void ActivateAccessoryEffects(Player player, Type accessoryType)
        {
            var accessory = _allAccessories.FirstOrDefault(a => a.AccessoryType == accessoryType);
            if (accessory == null) return;

            if (!_playerActiveEffects.ContainsKey(player.whoAmI))
                _playerActiveEffects[player.whoAmI] = new Dictionary<Type, List<AccessoryEffect>>();

            var playerEffects = _playerActiveEffects[player.whoAmI];

            // Add all effects from this accessory to the player's active effects
            foreach (var kvp in accessory.KeywordEffects)
            {
                var keywordType = kvp.Key;
                var effects = kvp.Value;

                if (!playerEffects.ContainsKey(keywordType))
                    playerEffects[keywordType] = new List<AccessoryEffect>();

                playerEffects[keywordType].AddRange(effects);
            }
        }

        // Called when player unequips an accessory
        public static void DeactivateAccessoryEffects(Player player, Type accessoryType)
        {
            if (!_playerActiveEffects.TryGetValue(player.whoAmI, out var playerEffects))
                return;

            var accessory = _allAccessories.FirstOrDefault(a => a.AccessoryType == accessoryType);
            if (accessory == null) return;

            // Remove all effects from this accessory from the player's active effects
            foreach (var kvp in accessory.KeywordEffects)
            {
                var keywordType = kvp.Key;
                var effectsToRemove = kvp.Value;

                if (playerEffects.TryGetValue(keywordType, out var playerKeywordEffects))
                {
                    foreach (var effect in effectsToRemove)
                    {
                        playerKeywordEffects.Remove(effect);
                    }

                    // Clean up empty lists
                    if (playerKeywordEffects.Count == 0)
                        playerEffects.Remove(keywordType);
                }
            }
        }

        // Called when player leaves or resets effects
        public static void ClearPlayerEffects(Player player)
        {
            _playerActiveEffects.Remove(player.whoAmI);
        }
    }
}