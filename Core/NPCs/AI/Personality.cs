using Microsoft.Xna.Framework;
using Terraria;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Per-instance behavioral bias for an NPC, rolled once at spawn.
    /// Each axis is a 0 to 1 scalar that substates interpret in their own way.
    /// </summary>
    public class Personality
    {
        /// <summary>
        /// How hard the NPC commits to offense, such as chaining attacks or closing distance.
        /// </summary>
        public float Aggression;

        /// <summary>
        /// How readily the NPC disengages, repositions, or keeps spacing.
        /// </summary>
        public float Caution;

        /// <summary>
        /// How often and how fast the NPC reacts to incoming threats.
        /// </summary>
        public float Reactivity;
    }

    /// <summary>
    /// Per-type roll ranges for the personality axes. Each enemy type declares its
    /// bounds and every spawned instance rolls a point inside them.
    /// </summary>
    public class PersonalityProfile
    {
        public (float min, float max) Aggression = (0f, 1f);
        public (float min, float max) Caution = (0f, 1f);
        public (float min, float max) Reactivity = (0f, 1f);

        /// <summary>
        /// Rolls a concrete personality from the declared ranges.
        /// </summary>
        public Personality Roll()
        {
            return new Personality
            {
                Aggression = Range(Aggression),
                Caution = Range(Caution),
                Reactivity = Range(Reactivity)
            };
        }

        private static float Range((float min, float max) r) => MathHelper.Lerp(r.min, r.max, Main.rand.NextFloat());
    }
}
