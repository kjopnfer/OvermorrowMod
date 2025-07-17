using OvermorrowMod.Common.Items.Guns;
using OvermorrowMod.Core.Items.Guns;
using System.Collections.Generic;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Interface for items that can modify gun behavior and visual appearance.
    /// Provides callbacks for reload events and bullet modification.
    /// </summary>
    public interface IGunModifier
    {
        /// <summary>
        /// Modifies the gun's stats before they are used for gameplay calculations.
        /// This method is called every frame while the gun is being used.
        /// </summary>
        /// <param name="stats">The gun's current stats that can be modified.</param>
        /// <param name="player">The player using the gun.</param>
        void ModifyGunStats(GunStats stats, Player player);

        /// <summary>
        /// Called when any gun shoots. Only called if this modifier affects the gun.
        /// </summary>
        /// <param name="gun">The gun that fired.</param>
        /// <param name="player">The player who fired the gun.</param>
        /// <param name="bullet">The bullet projectile that was created.</param>
        void OnGunShoot(HeldGun gun, Player player, Projectile bullet) { }

        /// <summary>
        /// Called when any gun reloads. Only called if this modifier affects the gun.
        /// </summary>
        /// <param name="gun">The gun that reloaded.</param>
        /// <param name="player">The player who reloaded the gun.</param>
        /// <param name="wasSuccessful">Whether the reload was successful.</param>
        void OnGunReload(HeldGun gun, Player player, bool wasSuccessful) { }

        /// <summary>
        /// Called when reload is successful. Allows modification of bullet display.
        /// </summary>
        /// <param name="gun">The gun that was reloaded.</param>
        /// <param name="player">The player who reloaded the gun.</param>
        /// <param name="bullets">The bullets in the gun's display that can be modified.</param>
        void OnReloadSuccess(HeldGun gun, Player player, List<BulletObject> bullets) { }

        /// <summary>
        /// Called when reload fails. Allows modification of bullet display.
        /// </summary>
        /// <param name="gun">The gun that failed to reload.</param>
        /// <param name="player">The player who failed the reload.</param>
        /// <param name="bullets">The bullets in the gun's display that can be modified.</param>
        void OnReloadFail(HeldGun gun, Player player, List<BulletObject> bullets) { }

        /// <summary>
        /// Called when a reload zone is hit. Allows per-zone bullet modification.
        /// </summary>
        /// <param name="gun">The gun being reloaded.</param>
        /// <param name="player">The player reloading the gun.</param>
        /// <param name="bullets">The bullets in the gun's display that can be modified.</param>
        /// <param name="zoneIndex">The index of the zone that was hit.</param>
        /// <param name="zonesRemaining">The number of zones remaining to hit.</param>
        void OnReloadZoneHit(HeldGun gun, Player player, List<BulletObject> bullets, int zoneIndex, int zonesRemaining) { }
    }
}