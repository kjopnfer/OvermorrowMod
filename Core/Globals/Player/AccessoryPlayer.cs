using Terraria.ModLoader;

namespace OvermorrowMod.Core.Globals
{
    public class AccessoryPlayer : ModPlayer
    {
        /// <summary>
        /// Used for calculating whether the player is in the NPC's aggro range.
        /// Higher values reduce the NPC's aggro range while increasing their alert threshold.
        /// </summary>
        public int AlertBonus = 0;

        /// <summary>
        /// Used for calculating how quickly the enemy loses aggro if the Player is their target.
        /// </summary>
        public float AggroLossBonus = 0;

        public override void ResetEffects()
        {
            AlertBonus = 0;
            AggroLossBonus = 0;
        }
    }
}