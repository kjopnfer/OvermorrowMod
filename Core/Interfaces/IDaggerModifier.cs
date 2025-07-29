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
    /// </summary>
    public interface IDaggerModifier
    {
        /// <summary>
        /// Modifies the dagger's stats before they are used for gameplay calculations.
        /// Called every frame while the dagger is being used.
        /// </summary>
        void ModifyDaggerStats(DaggerStats stats, Player player);

        /// <summary>
        /// Called when a dagger performs a charged throw (flash throw).
        /// Use this for special effects when the player charges up before throwing.
        /// </summary>
        void OnChargedThrow(HeldDagger dagger, Player player, ThrownDagger thrownDagger);

        /// <summary>
        /// Called when a dagger successfully hits an enemy during a stab attack.
        /// </summary>
        void OnStabHit(HeldDagger dagger, Player player, NPC target, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// Called when a dagger successfully hits an enemy during a slash attack.
        /// </summary>
        void OnSlashHit(HeldDagger dagger, Player player, NPC target, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// Called when a thrown dagger hits an enemy.
        /// </summary>
        void OnThrownDaggerHit(ThrownDagger thrownDagger, Player player, NPC target, ref NPC.HitModifiers modifiers);

        /// <summary>
        /// Called when a combo sequence is completed successfully.
        /// </summary>
        void OnComboComplete(HeldDagger dagger, Player player, int comboLength);

        /// <summary>
        /// Called when a combo sequence is broken or reset.
        /// </summary>
        void OnComboBreak(HeldDagger dagger, Player player, int currentComboLength);

        /// <summary>
        /// Called when dual wielding daggers both hit the same target simultaneously.
        /// </summary>
        void OnDualWieldHit(HeldDagger mainDagger, HeldDagger offDagger, Player player, NPC target);
    }

    /// <summary>
    /// Interface for accessories that provide visual effects to daggers.
    /// Separates visual modifications from gameplay modifications for better organization.
    /// </summary>
    public interface IDaggerDrawEffects
    {
        /// <summary>
        /// Draws visual effects while the dagger is being charged for throwing.
        /// </summary>
        void DrawChargingEffects(HeldDagger dagger, Player player, SpriteBatch spriteBatch, float chargeProgress);

        /// <summary>
        /// Draws visual effects on the dagger during combat.
        /// </summary>
        void DrawDaggerEffects(HeldDagger dagger, Player player, SpriteBatch spriteBatch, Vector2 daggerPosition, DaggerAttack currentAttack);

        /// <summary>
        /// Draws visual effects on thrown daggers.
        /// </summary>
        void DrawThrownDaggerEffects(ThrownDagger thrownDagger, Player player, SpriteBatch spriteBatch);

        /// <summary>
        /// Draws effects when daggers are dual wielded.
        /// </summary>
        void DrawDualWieldEffects(HeldDagger mainDagger, HeldDagger offDagger, Player player, SpriteBatch spriteBatch);
    }

    /// <summary>
    /// Extended interface for daggers that need special throwing behavior.
    /// Implement this on projectiles or accessories that want custom throw mechanics.
    /// </summary>
    public interface ISpecialThrowBehavior
    {
        /// <summary>
        /// Called when a dagger is thrown to potentially override default throw behavior.
        /// Return true to prevent default throwing logic.
        /// </summary>
        bool OnDaggerThrown(HeldDagger dagger, Player player, Vector2 throwVelocity, ref Projectile thrownProjectile);

        /// <summary>
        /// Modifies the throw velocity before the dagger is launched.
        /// </summary>
        void ModifyThrowVelocity(HeldDagger dagger, Player player, ref Vector2 velocity, float chargeProgress);
    }
}