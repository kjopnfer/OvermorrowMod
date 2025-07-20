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
            // Subscribe to all keyword events
            AccessoryKeywords.OnRetaliate += (player, attacker, hurtInfo) =>
                    TriggerKeywordEffects(AccessoryKeywordTypes.Retaliate, player, attacker, hurtInfo);

            AccessoryKeywords.OnStrike += (player, target, hit, damageDone) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Strike, player, target, hit, damageDone);

            AccessoryKeywords.OnStrikeProjectile += (player, projectile, target, hit, damageDone) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.StrikeProjectile, player, projectile, target, hit, damageDone);

            AccessoryKeywords.OnVigor += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Vigor, player);

            AccessoryKeywords.OnDeathsDoor += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.DeathsDoor, player);

            AccessoryKeywords.OnMindDown += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.MindDown, player);

            AccessoryKeywords.OnExecute += (player, killedNPC) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Execute, player, killedNPC);

            AccessoryKeywords.OnAmbush += (player, target, hit, damageDone) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Ambush, player, target, hit, damageDone);

            AccessoryKeywords.OnAerial += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Aerial, player);

            AccessoryKeywords.OnAwakened += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Awakened, player);

            AccessoryKeywords.OnFocus += (player, projectile) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Focus, player, projectile);

            AccessoryKeywords.OnReload += (player, wasSuccessful) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Reload, player, wasSuccessful);

            AccessoryKeywords.OnMisfire += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Misfire, player);

            AccessoryKeywords.OnQuickslot += (player, item) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Quickslot, player, item);

            AccessoryKeywords.OnRespite += (player) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Respite, player);

            AccessoryKeywords.OnSecondary += (player, item) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.Secondary, player, item);

            //AccessoryKeywords.OnTrueMelee += (player, item, projectile, target, modifiers) =>
            //    TriggerKeywordEffects(AccessoryKeywordTypes.TrueMelee, player, item, projectile, target, modifiers);
            AccessoryKeywords.OnTrueMelee += TriggerTrueMeleeEffects;

            AccessoryKeywords.OnProjectileSpawn += (player, projectile, source) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.ProjectileSpawn, player, projectile, source);

            AccessoryKeywords.OnProjectileModifyHit += (player, projectile, target, modifiers) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.ProjectileModifyHit, player, projectile, target, modifiers);

            AccessoryKeywords.OnProjectileKill += (player, projectile, timeLeft) =>
                TriggerKeywordEffects(AccessoryKeywordTypes.ProjectileKill, player, projectile, timeLeft);
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

            // Check if this accessory's effects are already active to prevent duplicates
            foreach (var kvp in accessory.KeywordEffects)
            {
                var keywordType = kvp.Key;
                var effects = kvp.Value;

                if (!playerEffects.ContainsKey(keywordType))
                    playerEffects[keywordType] = new List<AccessoryEffect>();

                // Only add effects that aren't already in the list
                foreach (var effect in effects)
                {
                    if (!playerEffects[keywordType].Contains(effect))
                    {
                        playerEffects[keywordType].Add(effect);
                    }
                }
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

        private static void TriggerTrueMeleeEffects(Player player, Item item, Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            var accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();

            foreach (var accessoryType in accessoryPlayer.GetActiveAccessories())
            {
                var accessory = _allAccessories.FirstOrDefault(a => a.AccessoryType == accessoryType);
                if (accessory?.TrueMeleeCallback != null)
                {
                    accessory.TrueMeleeCallback(player, item, projectile, target, ref modifiers);
                }
            }
        }
    }
}