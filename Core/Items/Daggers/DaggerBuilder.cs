using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Core.Items.Daggers
{
    public class DaggerBuilder
    {
        private DaggerStats _stats;
        public DaggerBuilder()
        {
            _stats = new DaggerStats();
        }
        public DaggerBuilder(DaggerStats baseStats)
        {
            _stats = baseStats.Clone();
        }

        // Basic Properties
        public DaggerBuilder WithDamageMultiplier(float multiplier) { _stats.DamageMultiplier = multiplier; return this; }
        public DaggerBuilder WithKnockbackMultiplier(float multiplier) { _stats.KnockbackMultiplier = multiplier; return this; }
        public DaggerBuilder WithScaleMultiplier(float multiplier) { _stats.ScaleMultiplier = multiplier; return this; }
        public DaggerBuilder WithSpeedMultiplier(float multiplier) { _stats.SpeedMultiplier = multiplier; return this; }

        // Combo System
        public DaggerBuilder WithComboSequence(params DaggerAttack[] attacks)
        {
            _stats.ComboSequence = new List<DaggerAttack>(attacks);
            return this;
        }
        public DaggerBuilder WithComboSpeedMultiplier(float multiplier) { _stats.ComboSpeedMultiplier = multiplier; return this; }
        public DaggerBuilder WithComboResetTime(int time) { _stats.ComboResetTime = time; return this; }

        // Attack Timings
        public DaggerBuilder WithSlashTiming(float backTime, float forwardTime, float holdTime)
        {
            _stats.SlashBackTime = backTime;
            _stats.SlashForwardTime = forwardTime;
            _stats.SlashHoldTime = holdTime;
            return this;
        }
        public DaggerBuilder WithStabTiming(float backTime, float forwardTime, float holdTime)
        {
            _stats.StabBackTime = backTime;
            _stats.StabForwardTime = forwardTime;
            _stats.StabHoldTime = holdTime;
            return this;
        }

        // Throwing
        public DaggerBuilder WithThrow(bool canThrow = true) { _stats.CanThrow = canThrow; return this; }
        public DaggerBuilder WithThrowVelocity(float velocity) { _stats.ThrowVelocity = velocity; return this; }
        public DaggerBuilder WithThrowDamageMultiplier(float multiplier) { _stats.ThrowDamageMultiplier = multiplier; return this; }
        public DaggerBuilder WithThrowCritBonus(float bonus) { _stats.ThrowCritBonus = bonus; return this; }
        public DaggerBuilder WithThrowRecoveryTime(int time) { _stats.ThrowRecoveryTime = time; return this; }

        // Audio
        public DaggerBuilder WithSlashSound(SoundStyle sound) { _stats.SlashSound = sound; return this; }
        public DaggerBuilder WithThrowSound(SoundStyle sound) { _stats.ThrowSound = sound; return this; }

        // Visual Effects
        public DaggerBuilder WithFlashColor(Color color) { _stats.FlashColor = color; return this; }
        public DaggerBuilder WithThrowTrailColor(Color color) { _stats.ThrowTrailColor = color; return this; }

        // Positioning
        public DaggerBuilder WithSlashPositionOffset(Vector2 offset) { _stats.SlashPositionOffset = offset; return this; }
        public DaggerBuilder WithStabPositionOffset(Vector2 offset) { _stats.StabPositionOffset = offset; return this; }
        public DaggerBuilder WithThrowPositionOffset(Vector2 offset) { _stats.ThrowPositionOffset = offset; return this; }

        // Throwing Physics & Behavior
        public DaggerBuilder WithFlightDurationMultiplier(float multiplier) { _stats.FlightDurationMultiplier = multiplier; return this; }
        public DaggerBuilder WithIdleDurationMultiplier(float multiplier) { _stats.IdleDurationMultiplier = multiplier; return this; }
        public DaggerBuilder WithImpaleDurationMultiplier(float multiplier) { _stats.ImpaleDurationMultiplier = multiplier; return this; }
        public DaggerBuilder WithGravityMultiplier(float multiplier) { _stats.GravityMultiplier = multiplier; return this; }
        public DaggerBuilder WithAirResistanceMultiplier(float multiplier) { _stats.AirResistanceMultiplier = multiplier; return this; }
        public DaggerBuilder WithGroundFrictionMultiplier(float multiplier) { _stats.GroundFrictionMultiplier = multiplier; return this; }

        // Animation Speed
        public DaggerBuilder WithAnimationSpeedMultiplier(float multiplier) { _stats.AnimationSpeedMultiplier = multiplier; return this; }

        // Impaling System
        public DaggerBuilder WithImpalingEnabled(bool enabled = true) { _stats.ImpalingEnabled = enabled; return this; }
        public DaggerBuilder WithImpaleDamageMultiplier(float multiplier) { _stats.ImpaleDamageMultiplier = multiplier; return this; }
        public DaggerBuilder WithImpaleDamageInterval(int interval) { _stats.ImpaleDamageInterval = interval; return this; }

        // Visual Overrides
        public DaggerBuilder WithOverrideIdleColor(Color color) { _stats.OverrideIdleColor = color; return this; }
        public DaggerBuilder WithOverrideTrailColor(Color color) { _stats.OverrideTrailColor = color; return this; }

        // Return Behavior
        public DaggerBuilder WithAutoReturn(bool enabled = true) { _stats.AutoReturn = enabled; return this; }
        public DaggerBuilder WithReturnSpeed(float speed) { _stats.ReturnSpeed = speed; return this; }
        public DaggerBuilder WithPierceOnReturn(bool enabled = true) { _stats.PierceOnReturn = enabled; return this; }

        public DaggerStats Build()
        {
            return _stats.Clone();
        }
    }
}