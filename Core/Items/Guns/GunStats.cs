using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Weapons.Guns;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Core.Items.Guns
{
    /// <summary>
    /// Contains all statistical and behavioral properties for a gun.
    /// This class is used by the gun system to determine how a gun behaves and can be modified by accessories.
    /// </summary>
    public class GunStats
    {
        /// <summary>
        /// Maximum number of shots the gun can fire before needing to reload.
        /// </summary>
        public int MaxShots = 6;

        /// <summary>
        /// Maximum time in ticks for the reload process to complete.
        /// </summary>
        public int MaxReloadTime = 60;

        /// <summary>
        /// Amount of recoil the gun produces when fired.
        /// </summary>
        public int RecoilAmount = 10;

        /// <summary>
        /// Time in ticks between each shot.
        /// </summary>
        public int ShootTime = 30;

        /// <summary>
        /// Animation time for shooting in ticks.
        /// </summary>
        public int ShootAnimation = 30;

        /// <summary>
        /// Maximum charge time for guns that require charging (like snipers).
        /// </summary>
        public int MaxChargeTime = 120;

        /// <summary>
        /// Multiplier applied to the gun's base damage.
        /// </summary>
        public float DamageMultiplier = 1f;

        /// <summary>
        /// Flat damage bonus added to the gun's base damage.
        /// </summary>
        public float DamageFlat = 0f;

        /// <summary>
        /// Multiplier applied to the gun's base knockback.
        /// </summary>
        public float KnockbackMultiplier = 1f;

        /// <summary>
        /// Scale multiplier for the gun's projectile sprite.
        /// </summary>
        public float ProjectileScale = 1f;

        /// <summary>
        /// Multiplier for reload speed. Lower values = faster reload.
        /// </summary>
        public float ReloadSpeedMultiplier = 1f;

        /// <summary>
        /// Multiplier for fire rate. Lower values = faster fire rate.
        /// </summary>
        public float FireRateMultiplier = 1f;

        /// <summary>
        /// Multiplier for recoil intensity. Lower values = less recoil.
        /// </summary>
        public float RecoilMultiplier = 1f;

        /// <summary>
        /// Multiplier for charge speed. Higher values = faster charging.
        /// </summary>
        public float ChargeSpeedMultiplier = 1f;

        /// <summary>
        /// Multiplier for accuracy. Lower values = more accurate.
        /// </summary>
        public float AccuracyMultiplier = 1f;

        /// <summary>
        /// Additional shots added to the gun's magazine.
        /// </summary>
        public int MaxShotsBonus = 0;

        /// <summary>
        /// Positioning offsets for left and right directions respectively.
        /// </summary>
        public (Vector2, Vector2) PositionOffset = (new Vector2(15, 0), new Vector2(15, 0));

        /// <summary>
        /// Bullet spawn positions for left and right directions respectively.
        /// </summary>
        public (Vector2, Vector2) BulletShootPosition = (new Vector2(15, -5), new Vector2(15, 15));

        /// <summary>
        /// Whether the gun requires two hands to operate.
        /// </summary>
        public bool TwoHanded = false;

        /// <summary>
        /// Whether the gun can perform right-click actions.
        /// </summary>
        public bool CanRightClick = false;

        /// <summary>
        /// Whether the gun can be reloaded.
        /// </summary>
        public bool CanReload = true;

        /// <summary>
        /// Whether the gun consumes ammo when firing.
        /// </summary>
        public bool CanConsumeAmmo = true;

        /// <summary>
        /// Whether the gun consumes ammo per individual bullet fired (for multi-shot guns).
        /// </summary>
        public bool ConsumePerShot = false;

        /// <summary>
        /// Whether the gun uses a delay after right-clicking.
        /// </summary>
        public bool UsesRightClickDelay = true;

        /// <summary>
        /// Reload zones that the player must click during reload.
        /// </summary>
        public List<ReloadZone> ClickZones = new List<ReloadZone>();

        /// <summary>
        /// Custom bullet projectile type to fire. Uses ProjectileID.None for default behavior.
        /// </summary>
        public int BulletType = ProjectileID.None;

        /// <summary>
        /// Required bullet item type to convert to BulletType. Uses ItemID.None for no conversion.
        /// </summary>
        public int ConvertBullet = ItemID.None;

        /// <summary>
        /// Sound played when the gun fires.
        /// </summary>
        public SoundStyle ShootSound = SoundID.Item41;

        /// <summary>
        /// Sound played when the gun finishes reloading.
        /// </summary>
        public SoundStyle ReloadFinishSound = new($"{nameof(OvermorrowMod)}/Sounds/RevolverReload");

        /// <summary>
        /// Additional bullets fired per shot (bonus projectiles).
        /// </summary>
        public int BonusBullets = 0;

        /// <summary>
        /// Additional damage applied to shots.
        /// </summary>
        public int BonusDamage = 0;

        /// <summary>
        /// Additional shots added to the current magazine.
        /// </summary>
        public int BonusAmmo = 0;

        /// <summary>
        /// Modifier applied to use time/animation.
        /// </summary>
        public int UseTimeModifier = 0;

        /// <summary>
        /// Creates a deep copy of this GunStats instance.
        /// </summary>
        /// <returns>A new GunStats instance with identical values.</returns>
        public GunStats Clone()
        {
            var cloned = (GunStats)this.MemberwiseClone();
            cloned.ClickZones = new List<ReloadZone>(ClickZones);
            return cloned;
        }
    }
}