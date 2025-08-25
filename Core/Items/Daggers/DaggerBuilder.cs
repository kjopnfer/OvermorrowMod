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
        public DaggerBuilder WithThrowChargeTime(float chargeTime) { _stats.ThrowChargeTime = chargeTime; return this; }
        public DaggerBuilder WithThrowMaxChargeTime(float maxChargeTime) { _stats.ThrowMaxChargeTime = maxChargeTime; return this; }
        public DaggerBuilder WithThrowDamageMultiplier(float multiplier) { _stats.ThrowDamageMultiplier = multiplier; return this; }
        public DaggerBuilder WithThrowCritBonus(float bonus) { _stats.ThrowCritBonus = bonus; return this; }
        public DaggerBuilder WithThrowRecoveryTime(int time) { _stats.ThrowRecoveryTime = time; return this; }

        // Audio
        public DaggerBuilder WithSlashSound(SoundStyle sound) { _stats.SlashSound = sound; return this; }
        public DaggerBuilder WithThrowSound(SoundStyle sound) { _stats.ThrowSound = sound; return this; }

        // Visual Effects
        public DaggerBuilder WithFlashColor(Color color) { _stats.FlashColor = color; return this; }
        public DaggerBuilder WithThrowTrailColor(Color color) { _stats.ThrowTrailColor = color; return this; }
        public DaggerBuilder WithChargeEffect(bool show = true) { _stats.ShowChargeEffect = show; return this; }

        // Positioning
        public DaggerBuilder WithSlashPositionOffset(Vector2 offset) { _stats.SlashPositionOffset = offset; return this; }
        public DaggerBuilder WithStabPositionOffset(Vector2 offset) { _stats.StabPositionOffset = offset; return this; }
        public DaggerBuilder WithThrowPositionOffset(Vector2 offset) { _stats.ThrowPositionOffset = offset; return this; }


        public DaggerStats Build()
        {
            return _stats.Clone();
        }
    }
}