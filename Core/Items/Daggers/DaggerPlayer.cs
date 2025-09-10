using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Items.Daggers
{
    /// <summary>
    /// Manages dagger-related modifiers and effects for the player.
    /// Handles collecting and organizing modifiers from equipped accessories.
    /// </summary>
    public class DaggerPlayer : ModPlayer
    {
        /// <summary>
        /// All currently active dagger modifiers from equipped accessories.
        /// </summary>
        public List<IDaggerModifier> ActiveModifiers { get; private set; } = new List<IDaggerModifier>();

        /// <summary>
        /// All currently active special throw behaviors from equipped accessories.
        /// </summary>
        public List<ISpecialThrowBehavior> ActiveThrowBehaviors { get; private set; } = new List<ISpecialThrowBehavior>();

        /// <summary>
        /// All currently active draw effects from equipped accessories.
        /// </summary>
        public List<IDaggerDrawEffects> ActiveDrawEffects { get; private set; } = new List<IDaggerDrawEffects>();

        // Combo tracking
        public int CurrentComboIndex = 0;
        public int ComboHitCount = 0;
        public int LastComboTime = 0;
        public int ConsecutiveHits = 0;

        // Statistics for accessory effects
        public int TotalDaggerHits = 0;
        public int TotalThrows = 0;
        public int SuccessfulCombos = 0;
        public float DaggerDamageDealt = 0f;

        public override void ResetEffects()
        {
            ActiveModifiers.Clear();
            ActiveThrowBehaviors.Clear();
            ActiveDrawEffects.Clear();

            CollectAccessoryModifiers();
        }

        /// <summary>
        /// Collects all dagger modifiers from equipped accessories and other sources.
        /// </summary>
        private void CollectAccessoryModifiers()
        {
            // Check all accessory slots for dagger modifiers
            for (int i = 0; i < Player.armor.Length; i++)
            {
                if (i >= Player.armor.Length) break; // Safety check

                Item accessory = Player.armor[i];
                if (accessory?.IsAir != false) continue;

                // Check if the accessory's ModItem implements dagger interfaces
                if (accessory.ModItem is IDaggerModifier daggerModifier)
                    ActiveModifiers.Add(daggerModifier);

                if (accessory.ModItem is ISpecialThrowBehavior throwBehavior)
                    ActiveThrowBehaviors.Add(throwBehavior);

                if (accessory.ModItem is IDaggerDrawEffects drawEffects)
                    ActiveDrawEffects.Add(drawEffects);
            }

            // Could also check buffs, other equipment slots, etc.
            // Example: Check for dagger-related buffs
            foreach (int buffType in Player.buffType)
            {
                if (buffType <= 0) continue;

                var modBuff = ModContent.GetModBuff(buffType);
                if (modBuff is IDaggerModifier buffModifier)
                    ActiveModifiers.Add(buffModifier);
            }
        }

        /// <summary>
        /// Gets all currently active held daggers for the player.
        /// </summary>
        private List<HeldDagger> GetActiveDaggers()
        {
            var daggers = new List<HeldDagger>();

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                var proj = Main.projectile[i];
                if (proj.active && proj.owner == Player.whoAmI && proj.ModProjectile is HeldDagger heldDagger)
                {
                    daggers.Add(heldDagger);
                }
            }

            return daggers;
        }

        /// <summary>
        /// Records a successful hit and updates combo tracking.
        /// </summary>
        public void RecordHit(DaggerAttack attackType, int damage)
        {
            TotalDaggerHits++;
            DaggerDamageDealt += damage;
            ComboHitCount++;
            ConsecutiveHits++;
            LastComboTime = (int)Main.GameUpdateCount;
        }

        /// <summary>
        /// Records a successful throw.
        /// </summary>
        public void RecordThrow()
        {
            TotalThrows++;
        }

        /// <summary>
        /// Advances the combo to the next attack in the sequence.
        /// </summary>
        public void AdvanceCombo(List<DaggerAttack> comboSequence)
        {
            CurrentComboIndex++;
            if (CurrentComboIndex >= comboSequence.Count)
            {
                // Combo completed
                SuccessfulCombos++;
                var activeDaggers = GetActiveDaggers();
                foreach (var dagger in activeDaggers)
                {
                    DaggerModifierHandler.TriggerComboComplete(dagger, Player, ComboHitCount);
                }

                CurrentComboIndex = 0;
            }
        }

        /// <summary>
        /// Resets combo tracking to initial state.
        /// </summary>
        public void ResetCombo()
        {
            CurrentComboIndex = 0;
            ComboHitCount = 0;
            ConsecutiveHits = 0;
        }

        /// <summary>
        /// Forces a combo break, useful for special abilities or effects.
        /// </summary>
        public void ForceComboBreak()
        {
            var activeDaggers = GetActiveDaggers();
            foreach (var dagger in activeDaggers)
            {
                DaggerModifierHandler.TriggerComboBreak(dagger, Player, ComboHitCount);
            }
            ResetCombo();
        }
    }
}