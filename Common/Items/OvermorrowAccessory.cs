using OvermorrowMod.Core.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Items
{
    public abstract class OvermorrowAccessory : ModItem
    {
        protected AccessoryDefinition Definition { get; private set; }

        public sealed override void SetDefaults()
        {
            Item.accessory = true;

            Definition = new AccessoryDefinition(GetType());

            SafeSetDefaults();
            SetAccessoryEffects(Definition);
        }

        /// <summary>
        /// Override this to set item properties (width, height, rarity, value, etc.)
        /// </summary>
        protected abstract void SafeSetDefaults();

        /// <summary>
        /// Override this to add frame-by-frame updates while the accessory is equipped
        /// </summary>
        protected virtual void UpdateAccessoryEffects(Player player) { }

        /// <summary>
        /// Override this to add keyword effects to the accessory
        /// </summary>
        protected abstract void SetAccessoryEffects(AccessoryDefinition definition);

        /// <summary>
        /// Helper method to check if this accessory is active for a player
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