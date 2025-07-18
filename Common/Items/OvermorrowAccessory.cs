using OvermorrowMod.Core.Items.Accessories;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items
{
    public abstract class OvermorrowAccessory : ModItem
    {
        private static Dictionary<Type, AccessoryDefinition> _definitions = new();

        public sealed override void SetDefaults()
        {
            Item.accessory = true;

            if (!_definitions.ContainsKey(GetType()))
            {
                var definition = new AccessoryDefinition(GetType());
                SetAccessoryEffects(definition);
                _definitions[GetType()] = definition;
            }

            SafeSetDefaults();
        }

        /// <summary>
        /// Override this to set item properties (width, height, rarity, value, etc.)
        /// </summary>
        protected abstract void SafeSetDefaults();

        /// <summary>
        /// Override this to apply direct stat modifications and effects that should be active every frame while the accessory is equipped. 
        /// Use for simple stat bonuses, not for keyword-based effects.
        /// </summary>
        protected virtual void UpdateAccessoryEffects(Player player) { }

        /// <summary>
        /// Override this to add keyword effects to the accessory
        /// </summary>
        protected abstract void SetAccessoryEffects(AccessoryDefinition definition);

        /// <summary>
        /// Called every frame while this accessory is equipped. Handles activation of keyword-based effects
        /// and applies direct stat modifications that don't use the keyword system.
        /// </summary>
        public sealed override void UpdateAccessory(Player player, bool hideVisual)
        {
            var accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();
            if (!accessoryPlayer.HasAccessory(GetType()))
            {
                accessoryPlayer.ActivateAccessory(GetType());
                AccessoryManager.ActivateAccessoryEffects(player, GetType());
            }

            UpdateAccessoryEffects(player);
        }
    }
}