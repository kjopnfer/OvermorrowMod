using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Core.Items.Daggers
{
    public enum DaggerAttack
    {
        Throw = -1,
        Slash = 0,
        Stab = 1,
    }

    public class DaggerStats
    {
        // Basic Properties
        public float DamageMultiplier = 1f;
        public float KnockbackMultiplier = 1f;
        public float ScaleMultiplier = 1f;
        public float SpeedMultiplier = 1f;

        // Dual Wielding
        public bool IsDualWielded = true;
        public float DualWieldSpeedBonus = 1.2f;
        public float DualWieldDamageMultiplier = 0.9f;
        public Vector2 DualWieldOffset = new Vector2(2, 0);

        // Combo System
        public List<DaggerAttack> ComboSequence = new List<DaggerAttack> { DaggerAttack.Slash, DaggerAttack.Stab };
        public float ComboSpeedMultiplier = 1f;
        public int ComboResetTime = 120; // Ticks before combo resets

        // Attack Timings
        public float SlashBackTime = 15f;
        public float SlashForwardTime = 5f;
        public float SlashHoldTime = 10f;

        public float StabBackTime = 18f;
        public float StabForwardTime = 24f;
        public float StabHoldTime = 12f;

        // Throwing Mechanics
        public bool CanThrow = true;
        public float ThrowVelocity = 10f;
        public float ThrowChargeTime = 15f; // Time to reach "flash" threshold
        public float ThrowMaxChargeTime = 60f;
        public float ThrowDamageMultiplier = 1f;
        public float ThrowCritBonus = 20f; // Bonus crit chance when fully charged
        public int ThrowRecoveryTime = 600; // Time before thrown dagger can be picked up

        // Visual and Audio
        public SoundStyle SlashSound = SoundID.Item1;
        public SoundStyle StabSound = SoundID.Item1;
        public SoundStyle ThrowSound = SoundID.Item1;
        public SoundStyle ChargeSound = SoundID.MaxMana;

        public Color FlashColor = Color.White;
        public Color ThrowTrailColor = Color.Orange;
        public bool ShowChargeEffect = true;

        // Positioning
        public Vector2 SlashPositionOffset = new Vector2(10, 6);
        public Vector2 StabPositionOffset = Vector2.Zero;
        public Vector2 ThrowPositionOffset = new Vector2(10, 6);

        // Flight and Animation Timing Modifiers
        public float FlightDurationMultiplier { get; set; } = 1f;
        public float IdleDurationMultiplier { get; set; } = 1f;
        public float ImpaleDurationMultiplier { get; set; } = 1f;
        public float AnimationSpeedMultiplier { get; set; } = 1f;

        // Physics Modifiers
        public float GravityMultiplier { get; set; } = 1f;
        public float AirResistanceMultiplier { get; set; } = 1f;
        public float GroundFrictionMultiplier { get; set; } = 1f;

        // Special Effects
        public bool ImpalingEnabled { get; set; } = true;
        public float ImpaleDamageMultiplier { get; set; } = 0.33f; // 33% of original damage
        public int ImpaleDamageInterval { get; set; } = 60; // Ticks between DOT damage

        // Visual and Audio
        public Color? OverrideIdleColor { get; set; } = null;
        public Color? OverrideTrailColor { get; set; } = null;
        public SoundStyle? OverrideHitSound { get; set; } = null;

        // Combo Extensions
        public int MaxComboExtension { get; set; } = 0; // Add extra combo attacks
        public bool CrossSlashEnabled { get; set; } = true;

        // Return Behavior
        public bool AutoReturn { get; set; } = false;
        public float ReturnSpeed { get; set; } = 15f;
        public bool PierceOnReturn { get; set; } = false;

        public DaggerStats Clone()
        {
            var clone = (DaggerStats)this.MemberwiseClone();
            clone.ComboSequence = new List<DaggerAttack>(this.ComboSequence);
            return clone;
        }
    }

    public static class DaggerStatsExtensions
    {
        /// <summary>
        /// Applies timing modifications to animation values
        /// </summary>
        public static float ModifyTiming(this DaggerStats stats, float baseValue)
        {
            return baseValue / stats.AnimationSpeedMultiplier;
        }

        /// <summary>
        /// Applies duration modifications to flight/idle/impale durations
        /// </summary>
        public static int ModifyFlightDuration(this DaggerStats stats, int baseDuration)
        {
            return (int)(baseDuration * stats.FlightDurationMultiplier);
        }

        public static int ModifyIdleDuration(this DaggerStats stats, int baseDuration)
        {
            return (int)(baseDuration * stats.IdleDurationMultiplier);
        }

        public static int ModifyImpaleDuration(this DaggerStats stats, int baseDuration)
        {
            return (int)(baseDuration * stats.ImpaleDurationMultiplier);
        }

        /// <summary>
        /// Applies physics modifications
        /// </summary>
        public static float ModifyGravity(this DaggerStats stats, float baseGravity)
        {
            return baseGravity * stats.GravityMultiplier;
        }

        public static float ModifyAirResistance(this DaggerStats stats, float baseResistance)
        {
            return MathHelper.Lerp(1f, baseResistance, stats.AirResistanceMultiplier);
        }

        public static float ModifyGroundFriction(this DaggerStats stats, float baseFriction)
        {
            return MathHelper.Lerp(1f, baseFriction, stats.GroundFrictionMultiplier);
        }
    }
}