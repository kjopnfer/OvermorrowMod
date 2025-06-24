using OvermorrowMod.Common.Utilities;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Configuration class for NPC targeting behavior.
    /// Controls how NPCs acquire and maintain targets based on aggro, range, and missed attacks.
    /// </summary>
    public class NPCTargetingConfig
    {
        /// <summary>
        /// The maximum time (in frames) an NPC will stay aggroed on a target before losing interest.
        /// </summary>
        public float MaxAggroTime { get; set; } = 300f;

        /// <summary>
        /// The rate at which aggro decays per frame when the NPC is not actively engaging its target.
        /// </summary>
        public float AggroLossRate { get; set; } = 1f;

        /// <summary>
        /// The cooldown time (in frames) after an NPC loses aggro before it can target the same player again.
        /// </summary>
        public float AggroCooldownTime { get; set; } = 120f;

        /// <summary>
        /// The maximum number of missed attacks an NPC can have before losing interest in the target.
        /// </summary>
        public int MaxMissedAttacks { get; set; } = 5;

        /// <summary>
        /// The aggro radius shape for detecting and targeting entities.
        /// Defaults to a circular range of 10 tiles.
        /// </summary>
        public AggroRadius AggroRadius { get; set; } = AggroRadius.Circle(ModUtils.TilesToPixels(10));

        /// <summary>
        /// The attack radius shape for determining attack range.
        /// Defaults to a circular range of 10 tiles.
        /// </summary>
        public AggroRadius AttackRadius { get; set; } = AggroRadius.Circle(ModUtils.TilesToPixels(10));

        /// <summary>
        /// The alert radius shape for early detection (greater than aggro range).
        /// Leave null to have no alert range.
        /// </summary>
        public AggroRadius? AlertRadius { get; set; } = AggroRadius.Circle(ModUtils.TilesToPixels(12.5f));

        // Legacy properties for backward compatibility
        /// <summary>
        /// Legacy property. Use TargetRadius instead for more control.
        /// </summary>
        [System.Obsolete("Use TargetRadius instead for more flexible range shapes.")]
        public float MaxTargetRange
        {
            get => AggroRadius?.GetMaxRadius() ?? 0f;
            set => AggroRadius = AggroRadius.Circle(value);
        }

        /// <summary>
        /// Legacy property. Use AttackRadius instead for more control.
        /// </summary>
        [System.Obsolete("Use AttackRadius instead for more flexible range shapes.")]
        public float MaxAttackRange
        {
            get => AttackRadius?.GetMaxRadius() ?? 0f;
            set => AttackRadius = AggroRadius.Circle(value);
        }

        /// <summary>
        /// Legacy property. Use AlertRadius instead for more control.
        /// </summary>
        [System.Obsolete("Use AlertRadius instead for more flexible range shapes.")]
        public float? AlertRange
        {
            get => AlertRadius?.GetMaxRadius();
            set => AlertRadius = value.HasValue ? AggroRadius.Circle(value.Value) : null;
        }

        /// <summary>
        /// Determines whether the NPC should prioritize targets based on player aggro values.
        /// If set to true, NPCs will favor players with higher aggro values over closer targets.
        /// </summary>
        public bool PrioritizeAggro { get; set; } = true;

        /// <summary>
        /// Determines whether the NPC displays an aggro indicator above their head when a target is found.
        /// </summary>
        public bool DisplayAggroIndicator { get; set; } = true;

        /// <summary>
        /// Determines whether to show debug visualization of the aggro ranges.
        /// Only works in debug builds or when debug mode is enabled.
        /// </summary>
        public bool ShowDebugVisualization { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of <see cref="NPCTargetingConfig"/> with default values.
        /// </summary>
        public NPCTargetingConfig() { }

        /// <summary>
        /// Initializes a new instance of <see cref="NPCTargetingConfig"/> with custom aggro radius shapes.
        /// </summary>
        /// <param name="maxAggroTime">The maximum duration (in frames) the NPC remains aggroed.</param>
        /// <param name="aggroLossRate">The rate at which aggro decreases per frame.</param>
        /// <param name="aggroCooldownTime">The cooldown time before re-targeting a lost target.</param>
        /// <param name="aggroRadius">The aggro radius shape for detecting targets.</param>
        /// <param name="attackRadius">The attack radius shape for combat range. Targets outside of this range decrease aggro.</param>
        /// <param name="alertRadius">The alert radius shape for early detection. Set null to disable.</param>
        /// <param name="prioritizeAggro">Whether the NPC should prioritize targets with higher aggro values.</param>
        public NPCTargetingConfig(float maxAggroTime, float aggroLossRate, float aggroCooldownTime,
            AggroRadius aggroRadius, AggroRadius attackRadius, AggroRadius? alertRadius, bool prioritizeAggro)
        {
            MaxAggroTime = maxAggroTime;
            AggroLossRate = aggroLossRate;
            AggroCooldownTime = aggroCooldownTime;
            AggroRadius = aggroRadius;
            AttackRadius = attackRadius;
            AlertRadius = alertRadius;
            PrioritizeAggro = prioritizeAggro;
        }

        /// <summary>
        /// Legacy constructor for backward compatibility.
        /// </summary>
        [System.Obsolete("Use the constructor with AggroRadius parameters for more flexible range shapes.")]
        public NPCTargetingConfig(float maxAggroTime, float aggroLossRate, float aggroCooldownTime,
            float maxTargetRange, float maxAttackRange, float? alertRange, bool prioritizeAggro)
        {
            MaxAggroTime = maxAggroTime;
            AggroLossRate = aggroLossRate;
            AggroCooldownTime = aggroCooldownTime;
            AggroRadius = AggroRadius.Circle(maxTargetRange);
            AttackRadius = AggroRadius.Circle(maxAttackRange);
            AlertRadius = alertRange.HasValue ? AggroRadius.Circle(alertRange.Value) : null;
            PrioritizeAggro = prioritizeAggro;
        }
    }
}