using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Items.Daggers;
using OvermorrowMod.Core.Items.Daggers;
using Terraria;

namespace OvermorrowMod.Core.Interfaces
{
    /// <summary>
    /// Interface for accessories and items that can modify dagger behavior.
    /// Provides callbacks for various dagger events and stat modifications.
    /// Updated for the renamed dagger classes (removed "New" suffixes).
    /// </summary>
    public interface IDaggerModifier
    {
        /// <summary>
        /// Modifies the dagger's stats before they are used for gameplay calculations.
        /// Called every frame while the dagger is being used.
        /// </summary>
        void ModifyDaggerStats(DaggerStats stats, Player player);

        /// <summary>
        /// Called when a dagger successfully hits an enemy during a slash attack.
        /// This includes both regular slashes and cross slash attacks.
        /// </summary>
        void OnSlashHit(HeldDagger dagger, Player player, NPC target, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// Called when a thrown dagger hits an enemy.
        /// </summary>
        void OnThrownDaggerHit(ThrownDagger thrownDagger, Player player, NPC target, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// Called when a combo sequence is completed successfully.
        /// For daggers with cross slash, this includes the final cross slash attack.
        /// </summary>
        void OnComboComplete(HeldDagger dagger, Player player, int comboLength);

        /// <summary>
        /// Called when a combo sequence is broken or reset before completion.
        /// </summary>
        void OnComboBreak(HeldDagger dagger, Player player, int currentComboLength);

        /// <summary>
        /// Called when dual wielding daggers both hit the same target simultaneously.
        /// This can happen during regular slashes or cross slash attacks.
        /// </summary>
        void OnDualWieldHit(HeldDagger mainDagger, HeldDagger offDagger, Player player, NPC target);

        /// <summary>
        /// Called when a cross slash attack (final combo attack) is performed.
        /// This happens when both daggers are available and the combo reaches its maximum.
        /// </summary>
        void OnCrossSlash(HeldDagger mainDagger, HeldDagger offDagger, Player player);

        /// <summary>
        /// Called when a thrown dagger successfully impales an enemy.
        /// Only triggered if the dagger has impaling enabled.
        /// </summary>
        void OnDaggerImpale(ThrownDagger thrownDagger, Player player, NPC target);

        /// <summary>
        /// Called when a dagger is thrown (right-click).
        /// </summary>
        void OnDaggerThrown(ThrownDagger thrownDagger, Player player, Vector2 throwVelocity);
    }

    /// <summary>
    /// Interface for accessories that provide visual effects to daggers.
    /// Separates visual modifications from gameplay modifications for better organization.
    /// Updated for the renamed dagger classes (removed "New" suffixes).
    /// </summary>
    public interface IDaggerDrawEffects
    {
        /// <summary>
        /// Draws visual effects on the dagger during slash attacks.
        /// Called for both regular slashes and cross slash attacks.
        /// </summary>
        void DrawSlashEffects(HeldDagger dagger, Player player, SpriteBatch spriteBatch, Vector2 daggerPosition, bool isCrossSlash);

        /// <summary>
        /// Draws visual effects on thrown daggers during flight, idle, and impaled states.
        /// </summary>
        void DrawThrownDaggerEffects(ThrownDagger thrownDagger, Player player, SpriteBatch spriteBatch, ThrownDagger.AIStates currentState);

        /// <summary>
        /// Draws special effects when daggers are dual wielded.
        /// Called during regular dual wielding and cross slash attacks.
        /// </summary>
        void DrawDualWieldEffects(HeldDagger mainDagger, HeldDagger offDagger, Player player, SpriteBatch spriteBatch, bool isCrossSlash);

        /// <summary>
        /// Draws effects when a dagger is impaled in an enemy.
        /// </summary>
        void DrawImpaleEffects(ThrownDagger thrownDagger, Player player, SpriteBatch spriteBatch, NPC impaledTarget);

        /// <summary>
        /// Draws combo-related visual effects based on current combo progress.
        /// </summary>
        void DrawComboEffects(Player player, SpriteBatch spriteBatch, int currentCombo, int maxCombo);
    }

    /// <summary>
    /// Extended interface for daggers that need special throwing behavior.
    /// Implement this on projectiles or accessories that want custom throw mechanics.
    /// Updated for the renamed dagger classes (removed "New" suffixes).
    /// </summary>
    public interface ISpecialThrowBehavior
    {
        /// <summary>
        /// Called when a dagger is thrown to potentially override default throw behavior.
        /// Return true to prevent default throwing logic.
        /// </summary>
        bool OnDaggerThrown(ModDagger<HeldDagger, ThrownDagger> dagger, Player player, Vector2 throwVelocity, ref Projectile thrownProjectile);

        /// <summary>
        /// Modifies the throw velocity before the dagger is launched.
        /// No charge progress since there's no charging system anymore.
        /// </summary>
        void ModifyThrowVelocity(ModDagger<HeldDagger, ThrownDagger> dagger, Player player, ref Vector2 velocity);

        /// <summary>
        /// Called when a thrown dagger transitions between states (flight -> idle -> impaled).
        /// </summary>
        void OnStateTransition(ThrownDagger thrownDagger, Player player, ThrownDagger.AIStates fromState, ThrownDagger.AIStates toState);

        /// <summary>
        /// Called when a dagger hits the ground and enters idle state.
        /// </summary>
        void OnGroundHit(ThrownDagger thrownDagger, Player player, Vector2 hitPosition);

        /// <summary>
        /// Called when a player picks up an idle dagger.
        /// </summary>
        void OnDaggerRecovered(ThrownDagger thrownDagger, Player player);
    }
}