using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Items;

namespace OvermorrowMod.Common.Items.Guns
{
    /// <summary>
    /// Extension methods for the WeaponType enum to provide gun-specific functionality
    /// </summary>
    public static class WeaponTypeExtensions
    {
        /// <summary>
        /// Gets the default bullet texture for a weapon type
        /// </summary>
        public static string GetDefaultBulletTexture(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Shotgun => "GunBullet_Shotgun",
                WeaponType.Launcher => "GunBullet_Rocket",
                _ => "GunBullet"
            };
        }

        /// <summary>
        /// Gets whether a weapon type typically uses two hands
        /// </summary>
        public static bool IsTwoHanded(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Rifle => true,
                WeaponType.Shotgun => true,
                WeaponType.MachineGun => true,
                WeaponType.SubMachineGun => true,
                WeaponType.Sniper => true,
                WeaponType.Launcher => true,
                WeaponType.Musket => true,
                _ => false
            };
        }

        /// <summary>
        /// Gets the typical reload time for a weapon type
        /// </summary>
        public static int GetTypicalReloadTime(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Revolver => 60,
                WeaponType.Handgun => 45,
                WeaponType.Rifle => 90,
                WeaponType.Shotgun => 80,
                WeaponType.MachineGun => 120,
                WeaponType.SubMachineGun => 75,
                WeaponType.Sniper => 100,
                WeaponType.Launcher => 150,
                WeaponType.Musket => 90,
                _ => 60
            };
        }

        /// <summary>
        /// Gets typical magazine size for a weapon type
        /// </summary>
        public static int GetTypicalMagazineSize(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Revolver => 6,
                WeaponType.Handgun => 8,
                WeaponType.Rifle => 10,
                WeaponType.Shotgun => 2,
                WeaponType.MachineGun => 100,
                WeaponType.SubMachineGun => 30,
                WeaponType.Sniper => 1,
                WeaponType.Launcher => 1,
                WeaponType.Musket => 1,
                _ => 6
            };
        }

        /// <summary>
        /// Gets the typical recoil amount for a weapon type
        /// </summary>
        public static int GetTypicalRecoil(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Revolver => 15,
                WeaponType.Handgun => 10,
                WeaponType.Rifle => 8,
                WeaponType.Shotgun => 25,
                WeaponType.MachineGun => 5,
                WeaponType.SubMachineGun => 6,
                WeaponType.Sniper => 30,
                WeaponType.Launcher => 40,
                WeaponType.Musket => 20,
                _ => 10
            };
        }

        /// <summary>
        /// Gets the typical fire rate for a weapon type
        /// </summary>
        public static int GetTypicalFireRate(this WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Revolver => 30,
                WeaponType.Handgun => 20,
                WeaponType.Rifle => 12,
                WeaponType.Shotgun => 45,
                WeaponType.MachineGun => 6,
                WeaponType.SubMachineGun => 8,
                WeaponType.Sniper => 60,
                WeaponType.Launcher => 90,
                WeaponType.Musket => 50,
                _ => 30
            };
        }

        /// <summary>
        /// Checks if a weapon type is a gun (excludes bows)
        /// </summary>
        public static bool IsGun(this WeaponType weaponType)
        {
            return weaponType.HasFlag(WeaponType.Revolver) ||
                   weaponType.HasFlag(WeaponType.Handgun) ||
                   weaponType.HasFlag(WeaponType.Shotgun) ||
                   weaponType.HasFlag(WeaponType.Musket) ||
                   weaponType.HasFlag(WeaponType.Rifle) ||
                   weaponType.HasFlag(WeaponType.SubMachineGun) ||
                   weaponType.HasFlag(WeaponType.MachineGun) ||
                   weaponType.HasFlag(WeaponType.Launcher) ||
                   weaponType.HasFlag(WeaponType.Sniper);
        }

        /// <summary>
        /// Checks if a weapon type is a ranged weapon (guns plus bows).
        /// </summary>
        public static bool IsRangedWeapon(this WeaponType weaponType)
        {
            return weaponType.IsGun() || weaponType.HasFlag(WeaponType.Bow);
        }
    }
}

namespace OvermorrowMod.Core.Items.Guns
{
    /// <summary>
    /// Enhanced builder methods for WeaponType-based gun creation
    /// </summary>
    public static class WeaponTypeGunBuilderExtensions
    {
        public static GunBuilder AsWeaponType(this GunBuilder builder, WeaponType weaponType)
        {
            return builder
                .WithMaxShots(weaponType.GetTypicalMagazineSize())
                .WithReloadTime(weaponType.GetTypicalReloadTime())
                .WithRecoil(weaponType.GetTypicalRecoil())
                .WithShootTime(weaponType.GetTypicalFireRate())
                .WithTwoHanded(weaponType.IsTwoHanded())
                .WithClickZones(GetDefaultClickZones(weaponType));
        }

        public static GunBuilder AsRevolver(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Revolver)
                .WithSpinCylinderOnReload();
        }

        public static GunBuilder AsHandgun(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Handgun)
                .WithShootTime(20)
                .WithShootAnimation(20);
        }

        public static GunBuilder AsShotgun(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Shotgun)
                .WithBonusBullets(3) // Shotguns fire multiple pellets
                .WithBulletUITexture("GunBullet_Shotgun");
        }

        public static GunBuilder AsRifle(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Rifle);
        }

        public static GunBuilder AsSubMachineGun(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.SubMachineGun);
        }

        public static GunBuilder AsMachineGun(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.MachineGun)
                .WithFireMode(GunFireMode.Automatic)
                .WithShootTime(6)
                .WithShootAnimation(6)
                .WithReload(false)
                .WithConsumePerShot()
                .WithChargeTime(60);
        }

        public static GunBuilder AsSniper(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Sniper)
                .WithRightClick()
                .WithDamageMultiplier(2.5f)
                .WithChargeTime(120);
        }

        public static GunBuilder AsLauncher(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Launcher)
                .WithDamageMultiplier(3.0f)
                .WithBulletUITexture("GunBullet_Rocket");
        }

        public static GunBuilder AsMusket(this GunBuilder builder)
        {
            return builder.AsWeaponType(WeaponType.Musket)
                .WithDamageMultiplier(2.0f);
        }

        private static (int, int)[] GetDefaultClickZones(WeaponType weaponType)
        {
            return weaponType switch
            {
                WeaponType.Revolver => new[] { (20, 40), (60, 80) },
                WeaponType.Handgun => new[] { (25, 45), (55, 75) },
                WeaponType.Shotgun => new[] { (30, 70) },
                WeaponType.Rifle => new[] { (15, 25), (35, 45), (55, 65), (75, 85) },
                WeaponType.SubMachineGun => new[] { (20, 35), (50, 65), (80, 95) },
                WeaponType.MachineGun => new (int, int)[0], // No reload zones for machine guns
                WeaponType.Sniper => new[] { (40, 60) },
                WeaponType.Launcher => new[] { (35, 65) },
                WeaponType.Musket => new[] { (40, 60) },
                _ => new[] { (30, 70) }
            };
        }
    }
}