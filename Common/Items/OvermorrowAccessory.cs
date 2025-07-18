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
        /// Override this to reset any variables that should be reset each equip cycle
        /// </summary>
        protected virtual void ResetVariables() { }

        /// <summary>
        /// Called every frame while this accessory is equipped. Handles activation of keyword-based effects
        /// and applies direct stat modifications that don't use the keyword system.
        /// </summary>
        public sealed override void UpdateAccessory(Player player, bool hideVisual)
        {
            var accessoryPlayer = player.GetModPlayer<AccessoryPlayer>();
            if (!accessoryPlayer.HasAccessory(GetType()))
            {
                // Reset variables on first frame of being equipped
                ResetVariables();

                accessoryPlayer.ActivateAccessory(GetType());
                AccessoryManager.ActivateAccessoryEffects(player, GetType());
            }

            UpdateAccessoryEffects(player);
        }


        /// <summary>
        /// Gets the equipped instance of this accessory type for the given player
        /// </summary>
        protected T GetInstance<T>(Player player) where T : OvermorrowAccessory
        {
            // Check normal accessory slots (3-9)
            for (int i = 3; i <= 9; i++)
            {
                if (player.armor[i].ModItem is T instance && player.armor[i].type == ModContent.ItemType<T>())
                    return instance;
            }

            return null;
        }
    }
}