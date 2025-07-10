using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Weapons.Guns;
using Terraria.Audio;

namespace OvermorrowMod.Core.Items.Guns
{
    /// <summary>
    /// Fluent builder for creating gun statistics in a readable and maintainable way.
    /// Provides method chaining for easy configuration of gun properties.
    /// </summary>
    public class GunBuilder
    {
        private GunStats _stats;

        /// <summary>
        /// Creates a new GunBuilder with default values.
        /// </summary>
        public GunBuilder()
        {
            _stats = new GunStats();
        }

        /// <summary>
        /// Creates a new GunBuilder starting from an existing GunStats template.
        /// </summary>
        /// <param name="baseStats">The base stats to copy from.</param>
        public GunBuilder(GunStats baseStats)
        {
            _stats = baseStats.Clone();
        }

        // Basic Properties
        public GunBuilder WithMaxShots(int maxShots) { _stats.MaxShots = maxShots; return this; }
        public GunBuilder WithReloadTime(int reloadTime) { _stats.MaxReloadTime = reloadTime; return this; }
        public GunBuilder WithRecoil(int recoil) { _stats.RecoilAmount = recoil; return this; }
        public GunBuilder WithShootTime(int shootTime) { _stats.ShootTime = shootTime; return this; }
        public GunBuilder WithShootAnimation(int shootAnimation) { _stats.ShootAnimation = shootAnimation; return this; }
        public GunBuilder WithChargeTime(int chargeTime) { _stats.MaxChargeTime = chargeTime; return this; }

        // Damage and Combat
        public GunBuilder WithDamageMultiplier(float multiplier) { _stats.DamageMultiplier = multiplier; return this; }
        public GunBuilder WithFlatDamage(float damage) { _stats.DamageFlat = damage; return this; }
        public GunBuilder WithKnockbackMultiplier(float multiplier) { _stats.KnockbackMultiplier = multiplier; return this; }
        public GunBuilder WithBonusBullets(int bullets) { _stats.BonusBullets = bullets; return this; }
        public GunBuilder WithBonusDamage(int damage) { _stats.BonusDamage = damage; return this; }
        public GunBuilder WithBonusAmmo(int ammo) { _stats.BonusAmmo = ammo; return this; }

        // Speed and Timing
        public GunBuilder WithReloadSpeedMultiplier(float multiplier) { _stats.ReloadSpeedMultiplier = multiplier; return this; }
        public GunBuilder WithFireRateMultiplier(float multiplier) { _stats.FireRateMultiplier = multiplier; return this; }
        public GunBuilder WithChargeSpeedMultiplier(float multiplier) { _stats.ChargeSpeedMultiplier = multiplier; return this; }
        public GunBuilder WithUseTimeModifier(int modifier) { _stats.UseTimeModifier = modifier; return this; }

        // Visual and Physical
        public GunBuilder WithProjectileScale(float scale) { _stats.ProjectileScale = scale; return this; }
        public GunBuilder WithRecoilMultiplier(float multiplier) { _stats.RecoilMultiplier = multiplier; return this; }
        public GunBuilder WithAccuracyMultiplier(float multiplier) { _stats.AccuracyMultiplier = multiplier; return this; }

        // Positioning
        public GunBuilder WithPositionOffset(Vector2 left, Vector2 right) { _stats.PositionOffset = (left, right); return this; }
        public GunBuilder WithBulletShootPosition(Vector2 left, Vector2 right) { _stats.BulletShootPosition = (left, right); return this; }

        // Behavior Flags
        public GunBuilder WithTwoHanded(bool twoHanded = true) { _stats.TwoHanded = twoHanded; return this; }
        public GunBuilder WithRightClick(bool canRightClick = true) { _stats.CanRightClick = canRightClick; return this; }
        public GunBuilder WithReload(bool canReload = true) { _stats.CanReload = canReload; return this; }
        public GunBuilder WithAmmoConsumption(bool consumes = true) { _stats.CanConsumeAmmo = consumes; return this; }
        public GunBuilder WithConsumePerShot(bool consumePerShot = true) { _stats.ConsumePerShot = consumePerShot; return this; }
        public GunBuilder WithRightClickDelay(bool useDelay = true) { _stats.UsesRightClickDelay = useDelay; return this; }

        // Ammo System
        public GunBuilder WithBulletType(int bulletType) { _stats.BulletType = bulletType; return this; }
        public GunBuilder WithConvertBullet(int convertBullet) { _stats.ConvertBullet = convertBullet; return this; }

        // Audio
        public GunBuilder WithShootSound(SoundStyle sound) { _stats.ShootSound = sound; return this; }
        public GunBuilder WithReloadSound(SoundStyle sound) { _stats.ReloadFinishSound = sound; return this; }

        // Reload Zones
        public GunBuilder WithClickZone(int startPercent, int endPercent)
        {
            _stats.ClickZones.Clear();
            _stats.ClickZones.Add(new ReloadZone(startPercent, endPercent));
            return this;
        }

        public GunBuilder WithClickZones(params (int start, int end)[] zones)
        {
            _stats.ClickZones.Clear();

            foreach (var zone in zones)
            {
                _stats.ClickZones.Add(new ReloadZone(zone.start, zone.end));
            }
            return this;
        }

        public GunBuilder ClearClickZones()
        {
            _stats.ClickZones.Clear();
            return this;
        }

        // Preset Configurations
        public GunBuilder AsRevolver()
        {
            return WithMaxShots(6)
                   .WithReloadTime(60)
                   .WithRecoil(15)
                   .WithShootTime(30)
                   .WithClickZones((20, 40), (60, 80));
        }

        public GunBuilder AsShotgun()
        {
            return WithMaxShots(2)
                   .WithReloadTime(80)
                   .WithRecoil(25)
                   .WithShootTime(45)
                   .WithBonusBullets(3)
                   .WithClickZone(30, 70);
        }

        public GunBuilder AsRifle()
        {
            return WithMaxShots(10)
                   .WithReloadTime(90)
                   .WithRecoil(8)
                   .WithShootTime(12)
                   .WithTwoHanded()
                   .WithClickZones((15, 25), (35, 45), (55, 65), (75, 85));
        }

        public GunBuilder AsMachineGun()
        {
            return WithMaxShots(100)
                   .WithReloadTime(120)
                   .WithRecoil(5)
                   .WithShootTime(6)
                   .WithChargeTime(60)
                   .WithTwoHanded()
                   .WithReload(false)
                   .WithConsumePerShot();
        }

        public GunBuilder AsSniper()
        {
            return WithMaxShots(1)
                   .WithReloadTime(100)
                   .WithRecoil(30)
                   .WithShootTime(60)
                   .WithChargeTime(120)
                   .WithTwoHanded()
                   .WithRightClick()
                   .WithDamageMultiplier(2.5f)
                   .WithClickZone(40, 60);
        }

        public GunBuilder AsPistol()
        {
            return WithMaxShots(8)
                   .WithReloadTime(45)
                   .WithRecoil(10)
                   .WithShootTime(20)
                   .WithClickZones((25, 45), (55, 75));
        }

        /// <summary>
        /// Builds and returns the final GunStats object.
        /// </summary>
        /// <returns>The configured GunStats instance.</returns>
        public GunStats Build()
        {
            return _stats.Clone();
        }

        /// <summary>
        /// Creates a quick revolver configuration.
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

        /// <summary>
        /// Creates a quick shotgun configuration.
        /// </summary>
        public static GunStats QuickShotgun(int maxShots = 2, int bonusBullets = 3, int recoil = 25)
        {
            return new GunBuilder()
                .AsShotgun()
                .WithMaxShots(maxShots)
                .WithBonusBullets(bonusBullets)
                .WithRecoil(recoil)
                .Build();
        }

        /// <summary>
        /// Creates a quick rifle configuration.
        /// </summary>
        public static GunStats QuickRifle(int maxShots = 10, int shootTime = 12, int recoil = 8)
        {
            return new GunBuilder()
                .AsRifle()
                .WithMaxShots(maxShots)
                .WithShootTime(shootTime)
                .WithRecoil(recoil)
                .Build();
        }
    }
}