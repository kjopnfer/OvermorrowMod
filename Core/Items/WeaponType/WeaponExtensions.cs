using OvermorrowMod.Core.Interfaces;
using System.Linq;
using Terraria;

namespace OvermorrowMod.Core.Items
{
    public static class WeaponExtensions
    {
        public static bool IsWeaponType(this Item item, WeaponType weaponType)
        {
            if (item.ModItem is IWeaponClassification weapon)
            {
                return weapon.WeaponType.HasFlag(weaponType);
            }
            return false;
        }

        public static bool IsWeaponType(this Projectile projectile, WeaponType weaponType)
        {
            if (projectile.ModProjectile is IProjectileClassification projWeapon)
            {
                return projWeapon.WeaponType.HasFlag(weaponType);
            }
            return false;
        }

        public static WeaponType GetWeaponType(this Item item)
        {
            if (item.ModItem is IWeaponClassification weapon)
            {
                return weapon.WeaponType;
            }
            return WeaponType.None;
        }

        public static WeaponType GetWeaponType(this Projectile projectile)
        {
            if (projectile.ModProjectile is IProjectileClassification projWeapon)
            {
                return projWeapon.WeaponType;
            }
            return WeaponType.None;
        }

        /// <summary>
        /// Get all active projectiles of a specific weapon type
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static Projectile[] GetActiveProjectilesOfType(WeaponType weaponType, int owner = -1)
        {
            return Main.projectile
                .Where(p => p.active && p.IsWeaponType(weaponType) && (owner == -1 || p.owner == owner))
                .ToArray();
        }

        /// <summary>
        /// Count active projectiles of a specific weapon type
        /// </summary>
        /// <param name="weaponType"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static int CountActiveProjectilesOfType(WeaponType weaponType, int owner = -1)
        {
            return Main.projectile
                .Count(p => p.active && p.IsWeaponType(weaponType) && (owner == -1 || p.owner == owner));
        }
    }
}