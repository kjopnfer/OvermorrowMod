using Microsoft.Xna.Framework;

namespace OvermorrowMod.Core.UI
{
    /// <summary>
    /// Something the reward picker can open over: it anchors the picker and receives the player's choice.
    /// </summary>
    public interface IRewardSource
    {
        /// <summary>
        /// World position the offers pop out of. The picker row sits above it.
        /// </summary>
        Vector2 RewardAnchor { get; }

        /// <summary>
        /// False once the source is gone or claimed, which closes the picker.
        /// </summary>
        bool RewardSourceActive { get; }

        void ClaimReward(int itemId);

        void DeclineReward();

        /// <summary>
        /// Replaces the current offers with a new roll that excludes them and stays at or above their lowest rarity.
        /// </summary>
        /// <returns>The new offers, or null when nothing could be rolled.</returns>
        int[] RerollOffers();
    }
}
