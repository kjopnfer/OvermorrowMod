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
        /// The maximum distance (in pixels) an NPC can detect and target an entity.
        /// Defaults to 10 tiles (16 pixels per tile).
        /// </summary>
        public float MaxTargetRange { get; set; } = ModUtils.TilesToPixels(10);

        /// <summary>
        /// The maximum distance (in pixels) for an NPC to attack.
        /// Defaults to 10 tiles (16 pixels per tile).
        /// </summary>
        public float MaxAttackRange { get; set; } = ModUtils.TilesToPixels(10);

        /// <summary>
        /// The distance at which the NPC enters an alert state (greater than aggro range).
        /// Leave null to have no alert range.
        /// </summary>
        public float? AlertRange { get; set; } = ModUtils.TilesToPixels(10 + 2.5f); // Example: 160px if aggro is 120px

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
        /// Initializes a new instance of <see cref="NPCTargetingConfig"/> with default values.
        /// </summary>
        public NPCTargetingConfig() { }

        /// <summary>
        /// Initializes a new instance of <see cref="NPCTargetingConfig"/> with custom values.
        /// </summary>
        /// <param name="maxAggroTime">The maximum duration (in frames) the NPC remains aggroed.</param>
        /// <param name="aggroLossRate">The rate at which aggro decreases per frame.</param>
        /// <param name="aggroCooldownTime">The cooldown time before re-targeting a lost target.</param>
        /// <param name="maxTargetRange">The maximum detection range (in pixels) for acquiring a target.</param>
        /// <param name="maxAttackRange">The maximum attack range after acquiring a target, not the same as the aggro range which is used for finding a target.</param>
        /// <param name="alertRange">The maximum detection range (in pixels) to be within alert. Set null to have no alert range.</param>
        /// <param name="prioritizeAggro">Whether the NPC should prioritize targets with higher aggro values.</param>
        public NPCTargetingConfig(float maxAggroTime, float aggroLossRate, float aggroCooldownTime, float maxTargetRange, float maxAttackRange, float? alertRange, bool prioritizeAggro)
        {
            MaxAggroTime = maxAggroTime;
            AggroLossRate = aggroLossRate;
            AggroCooldownTime = aggroCooldownTime;
            MaxTargetRange = maxTargetRange;
            MaxAttackRange = maxAttackRange;
            AlertRange = alertRange;
            PrioritizeAggro = prioritizeAggro;
        }
    }
}