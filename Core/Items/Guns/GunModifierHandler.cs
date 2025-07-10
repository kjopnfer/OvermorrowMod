using OvermorrowMod.Common.Weapons.Guns;
using OvermorrowMod.Core.Globals;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Guns
{
    /// <summary>
    /// Handles the application of gun modifiers from accessories to gun stats.
    /// This static class manages the interaction between accessories and gun behavior.
    /// </summary>
    public static class GunModifierHandler
    {
        /// <summary>
        /// Applies all active gun modifiers to the provided base stats and returns the modified result.
        /// This method processes modifiers from equipped accessories that implement IGunModifier.
        /// </summary>
        /// <param name="baseStats">The gun's original stats before modification.</param>
        /// <param name="player">The player whose accessories should be checked for modifiers.</param>
        /// <returns>A new GunStats instance with all modifiers applied.</returns>
        public static GunStats GetModifiedStats(GunStats baseStats, Player player)
        {
            GunStats modifiedStats = baseStats.Clone();

            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.ModifyGunStats(modifiedStats, player);
            }

            return modifiedStats;
        }

        /// <summary>
        /// Triggers gun shoot events for all active gun modifiers.
        /// This method is called every time a gun fires.
        /// </summary>
        /// <param name="gun">The gun that fired.</param>
        /// <param name="player">The player who fired the gun.</param>
        /// <param name="bullet">The bullet projectile that was created.</param>
        public static void TriggerGunShoot(HeldGun gun, Player player, Projectile bullet)
        {
            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.OnGunShoot(gun, player, bullet);
            }
        }

        /// <summary>
        /// Triggers gun reload events for all active gun modifiers.
        /// This method is called when a gun reload completes.
        /// </summary>
        /// <param name="gun">The gun that reloaded.</param>
        /// <param name="player">The player who reloaded the gun.</param>
        /// <param name="wasSuccessful">Whether the reload was successful.</param>
        public static void TriggerGunReload(HeldGun gun, Player player, bool wasSuccessful)
        {
            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.OnGunReload(gun, player, wasSuccessful);
            }
        }

        /// <summary>
        /// Triggers reload success events for all active gun modifiers.
        /// This method is called when a gun reload is successful.
        /// </summary>
        /// <param name="gun">The gun that was reloaded.</param>
        /// <param name="player">The player who reloaded the gun.</param>
        /// <param name="bullets">The bullets in the gun's display.</param>
        public static void TriggerReloadSuccess(HeldGun gun, Player player, List<BulletObject> bullets)
        {
            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.OnReloadSuccess(gun, player, bullets);
            }
        }

        /// <summary>
        /// Triggers reload fail events for all active gun modifiers.
        /// This method is called when a gun reload fails.
        /// </summary>
        /// <param name="gun">The gun that failed to reload.</param>
        /// <param name="player">The player who failed the reload.</param>
        /// <param name="bullets">The bullets in the gun's display.</param>
        public static void TriggerReloadFail(HeldGun gun, Player player, List<BulletObject> bullets)
        {
            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.OnReloadFail(gun, player, bullets);
            }
        }

        /// <summary>
        /// Triggers reload zone hit events for all active gun modifiers.
        /// This method is called when a reload zone is successfully hit.
        /// </summary>
        /// <param name="gun">The gun being reloaded.</param>
        /// <param name="player">The player reloading the gun.</param>
        /// <param name="bullets">The bullets in the gun's display.</param>
        /// <param name="zoneIndex">The index of the zone that was hit.</param>
        /// <param name="zonesRemaining">The number of zones remaining to hit.</param>
        public static void TriggerReloadZoneHit(HeldGun gun, Player player, List<BulletObject> bullets, int zoneIndex, int zonesRemaining)
        {
            var gunPlayer = player.GetModPlayer<GunPlayer>();
            foreach (var modifier in gunPlayer.ActiveModifiers)
            {
                modifier.OnReloadZoneHit(gun, player, bullets, zoneIndex, zonesRemaining);
            }
        }
    }
}