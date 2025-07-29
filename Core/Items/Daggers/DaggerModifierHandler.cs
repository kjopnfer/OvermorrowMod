using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Items.Daggers;
using Terraria;

namespace OvermorrowMod.Core.Items.Daggers
{
    /// <summary>
    /// Handles the application of dagger modifiers from accessories to dagger stats and behavior.
    /// Centralizes all modifier interactions for clean separation of concerns.
    /// </summary>
    public static class DaggerModifierHandler
    {
        /// <summary>
        /// Applies all active dagger modifiers to the provided base stats.
        /// </summary>
        public static DaggerStats GetModifiedStats(DaggerStats baseStats, Player player)
        {
            DaggerStats modifiedStats = baseStats.Clone();

            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.ModifyDaggerStats(modifiedStats, player);
            }

            return modifiedStats;
        }

        /// <summary>
        /// Triggers charged throw events for all applicable modifiers.
        /// </summary>
        public static void TriggerChargedThrow(HeldDagger dagger, Player player, ThrownDagger thrownDagger)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnChargedThrow(dagger, player, thrownDagger);
            }
        }

        /// <summary>
        /// Triggers stab hit events for all applicable modifiers.
        /// </summary>
        public static void TriggerStabHit(HeldDagger dagger, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnStabHit(dagger, player, target, ref modifiers);
            }
        }

        /// <summary>
        /// Triggers slash hit events for all applicable modifiers.
        /// </summary>
        public static void TriggerSlashHit(HeldDagger dagger, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnSlashHit(dagger, player, target, ref modifiers);
            }
        }

        /// <summary>
        /// Triggers thrown dagger hit events for all applicable modifiers.
        /// </summary>
        public static void TriggerThrownDaggerHit(ThrownDagger thrownDagger, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnThrownDaggerHit(thrownDagger, player, target, ref modifiers);
            }
        }

        /// <summary>
        /// Triggers combo complete events for all applicable modifiers.
        /// </summary>
        public static void TriggerComboComplete(HeldDagger dagger, Player player, int comboLength)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnComboComplete(dagger, player, comboLength);
            }
        }

        /// <summary>
        /// Triggers combo break events for all applicable modifiers.
        /// </summary>
        public static void TriggerComboBreak(HeldDagger dagger, Player player, int currentComboLength)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnComboBreak(dagger, player, currentComboLength);
            }
        }

        /// <summary>
        /// Triggers dual wield hit events when both daggers hit the same target.
        /// </summary>
        public static void TriggerDualWieldHit(HeldDagger mainDagger, HeldDagger offDagger, Player player, NPC target)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var modifier in daggerPlayer.ActiveModifiers)
            {
                modifier.OnDualWieldHit(mainDagger, offDagger, player, target);
            }
        }

        /// <summary>
        /// Checks if any modifier wants to override the default throw behavior.
        /// Returns true if default throwing should be prevented.
        /// </summary>
        public static bool HandleSpecialThrow(HeldDagger dagger, Player player, Vector2 throwVelocity, ref Projectile thrownProjectile)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var throwBehavior in daggerPlayer.ActiveThrowBehaviors)
            {
                if (throwBehavior.OnDaggerThrown(dagger, player, throwVelocity, ref thrownProjectile))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Allows modifiers to adjust throw velocity before the dagger is launched.
        /// </summary>
        public static void ModifyThrowVelocity(HeldDagger dagger, Player player, ref Vector2 velocity, float chargeProgress)
        {
            var daggerPlayer = player.GetModPlayer<DaggerPlayer>();
            foreach (var throwBehavior in daggerPlayer.ActiveThrowBehaviors)
            {
                throwBehavior.ModifyThrowVelocity(dagger, player, ref velocity, chargeProgress);
            }
        }
    }
}