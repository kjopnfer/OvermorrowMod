using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Items;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items
{
    public static class WeaponExtensions
    {
        public static bool IsWeaponType(this Item item, WeaponType weaponType)
        {
            // Check modded items first
            if (item.ModItem is IWeaponClassification weapon)
            {
                return weapon.WeaponType.HasFlag(weaponType);
            }

            // Check vanilla items
            return VanillaWeaponRegistry.IsWeaponType(item.type, weaponType);
        }

        public static bool IsWeaponType(this Projectile projectile, WeaponType weaponType)
        {
            // Check modded projectiles first
            if (projectile.ModProjectile is IProjectileClassification projWeapon)
            {
                return projWeapon.WeaponType.HasFlag(weaponType);
            }

            // Check vanilla projectiles
            return VanillaWeaponRegistry.IsProjectileWeaponType(projectile.type, weaponType);
        }

        public static WeaponType GetWeaponType(this Item item)
        {
            // Check modded items first
            if (item.ModItem is IWeaponClassification weapon)
            {
                return weapon.WeaponType;
            }

            // Check manually classified vanilla items
            var manualType = VanillaWeaponRegistry.GetWeaponType(item.type);
            if (manualType != WeaponType.None)
                return manualType;

            return WeaponType.None;
        }

        public static WeaponType GetWeaponType(this Projectile projectile)
        {
            // Check modded projectiles first
            if (projectile.ModProjectile is IProjectileClassification projWeapon)
            {
                return projWeapon.WeaponType;
            }

            // Check manually classified vanilla projectiles
            var manualType = VanillaWeaponRegistry.GetProjectileWeaponType(projectile.type);
            if (manualType != WeaponType.None)
                return manualType;

            return WeaponType.None;
        }

        // Get all active projectiles of a specific weapon type
        public static Projectile[] GetActiveProjectilesOfType(WeaponType weaponType, int owner = -1)
        {
            return Main.projectile
                .Where(p => p.active && p.IsWeaponType(weaponType) && (owner == -1 || p.owner == owner))
                .ToArray();
        }

        // Count active projectiles of a specific weapon type
        public static int CountActiveProjectilesOfType(WeaponType weaponType, int owner = -1)
        {
            return Main.projectile
                .Count(p => p.active && p.IsWeaponType(weaponType) && (owner == -1 || p.owner == owner));
        }
    }
}