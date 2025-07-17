using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Items;
using Terraria.ID;

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
        /// Checks if a weapon type is a gun/ranged weapon
        /// </summary>
        public static bool IsRangedWeapon(this WeaponType weaponType)
        {
            return weaponType.HasFlag(WeaponType.Revolver) ||
                   weaponType.HasFlag(WeaponType.Handgun) ||
                   weaponType.HasFlag(WeaponType.Shotgun) ||
                   weaponType.HasFlag(WeaponType.Musket) ||
                   weaponType.HasFlag(WeaponType.Rifle) ||
                   weaponType.HasFlag(WeaponType.SubMachineGun) ||
                   weaponType.HasFlag(WeaponType.MachineGun) ||
                   weaponType.HasFlag(WeaponType.Launcher) ||
                   weaponType.HasFlag(WeaponType.Sniper) ||
                   weaponType.HasFlag(WeaponType.Bow);
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
            return builder.AsWeaponType(WeaponType.Revolver);
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
                .WithBonusBullets(3); // Shotguns fire multiple pellets
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
                .WithDamageMultiplier(3.0f);
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

        /// <summary>
        /// Quick factory methods for common weapon types
        /// </summary>
        public static GunStats QuickRevolver(int maxShots = 6, int reloadTime = 60, int recoil = 15)
        {
            return new GunBuilder()
                .AsRevolver()
                .WithMaxShots(maxShots)
                .WithReloadTime(reloadTime)
                .WithRecoil(recoil)
                .Build();
        }

        public static GunStats QuickShotgun(int maxShots = 2, int bonusBullets = 3, int recoil = 25)
        {
            return new GunBuilder()
                .AsShotgun()
                .WithMaxShots(maxShots)
                .WithBonusBullets(bonusBullets)
                .WithRecoil(recoil)
                .Build();
        }

        public static GunStats QuickRifle(int maxShots = 10, int shootTime = 12, int recoil = 8)
        {
            return new GunBuilder()
                .AsRifle()
                .WithMaxShots(maxShots)
                .WithShootTime(shootTime)
                .WithRecoil(recoil)
                .Build();
        }

        public static GunStats QuickSniper(float damageMultiplier = 2.5f, int chargeTime = 120)
        {
            return new GunBuilder()
                .AsSniper()
                .WithDamageMultiplier(damageMultiplier)
                .WithChargeTime(chargeTime)
                .Build();
        }

        public static GunStats QuickMachineGun(int maxShots = 100, int chargeTime = 60)
        {
            return new GunBuilder()
                .AsMachineGun()
                .WithMaxShots(maxShots)
                .WithChargeTime(chargeTime)
                .Build();
        }
    }

    /// <summary>
    /// Updated gun presets using the WeaponType system
    /// </summary>
    public static class WeaponTypeGunPresets
    {
        public static readonly GunStats BasicRevolver = new GunBuilder()
            .AsRevolver()
            .Build();

        public static readonly GunStats FastRevolver = new GunBuilder()
            .AsRevolver()
            .WithFireRateMultiplier(0.7f)
            .WithReloadSpeedMultiplier(0.8f)
            .Build();

        public static readonly GunStats HeavyRevolver = new GunBuilder()
            .AsRevolver()
            .WithDamageMultiplier(1.5f)
            .WithRecoil(25)
            .WithMaxShots(5)
            .Build();

        public static readonly GunStats DoubleShotgun = new GunBuilder()
            .AsShotgun()
            .WithMaxShots(2)
            .WithBonusBullets(5)
            .WithRecoil(30)
            .Build();

        public static readonly GunStats PumpShotgun = new GunBuilder()
            .AsShotgun()
            .WithMaxShots(6)
            .WithBonusBullets(3)
            .WithRecoil(20)
            .WithReloadTime(100)
            .Build();

        public static readonly GunStats AssaultRifle = new GunBuilder()
            .AsRifle()
            .WithMaxShots(30)
            .WithShootTime(8)
            .WithReloadTime(120)
            .Build();

        public static readonly GunStats BattleRifle = new GunBuilder()
            .AsRifle()
            .WithMaxShots(20)
            .WithShootTime(15)
            .WithDamageMultiplier(1.3f)
            .WithRecoil(15)
            .Build();

        public static readonly GunStats AntiMaterielRifle = new GunBuilder()
            .AsSniper()
            .WithDamageMultiplier(4.0f)
            .WithRecoil(50)
            .WithReloadTime(150)
            .WithChargeTime(180)
            .Build();

        public static readonly GunStats LightMachineGun = new GunBuilder()
            .AsMachineGun()
            .WithMaxShots(80)
            .WithChargeTime(45)
            .WithRecoil(8)
            .Build();

        public static readonly GunStats GrenadeLauncher = new GunBuilder()
            .AsLauncher()
            .WithMaxShots(6)
            .WithReloadTime(120)
            .WithDamageMultiplier(2.5f)
            .WithBulletType(ProjectileID.Grenade)
            .Build();

        public static readonly GunStats FlintlockMusket = new GunBuilder()
            .AsMusket()
            .WithMaxShots(1)
            .WithReloadTime(90)
            .WithDamageMultiplier(2.0f)
            .WithRecoil(20)
            .Build();

        public static readonly GunStats Uzi = new GunBuilder()
            .AsSubMachineGun()
            .WithMaxShots(25)
            .WithShootTime(4)
            .WithRecoil(4)
            .Build();
    }
}