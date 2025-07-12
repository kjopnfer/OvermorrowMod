
using OvermorrowMod.Core.Items;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using OvermorrowMod.Common.Weapons.Guns;

namespace OvermorrowMod.Common.Utilities
{
    /// <summary>
    /// Helper methods for setting up items with WeaponType
    /// </summary>
    public static class ItemWeaponTypeExtensions
    {
        /// <summary>
        /// Sets up basic item properties based on weapon type
        /// </summary>
        public static void SetWeaponType(this Item item, WeaponType weaponType)
        {
            if (weaponType.IsGun())
            {
                item.DamageType = DamageClass.Ranged;
                item.useStyle = ItemUseStyleID.Shoot;
                item.noMelee = true;
                item.noUseGraphic = true;
                item.useAmmo = AmmoID.Bullet;

                // Set defaults based on weapon type
                item.useTime = weaponType.GetTypicalFireRate();
                item.useAnimation = weaponType.GetTypicalFireRate();

                // Special cases
                if (weaponType == WeaponType.Launcher)
                {
                    item.useAmmo = AmmoID.Rocket;
                }
            }
            else if (weaponType == WeaponType.Bow)
            {
                item.DamageType = DamageClass.Ranged;
                item.useStyle = ItemUseStyleID.Shoot;
                item.noMelee = true;
                item.noUseGraphic = true;
                item.useAmmo = AmmoID.Arrow;
                item.useTime = 25;
                item.useAnimation = 25;
            }
            // Add other weapon type setups as needed
        }
    }
}