using OvermorrowMod.Core.Loot;
using Terraria.DataStructures;

namespace OvermorrowMod.Content.Misc
{
    /// <summary>
    /// The reward chest in a treasure room. Spawned by its <see cref="TreasureReward_TE"/>, which holds the offers and the claim.
    /// </summary>
    public class TreasureChest : RewardChest
    {
        public int RewardID = -1;

        private static readonly RarityModifier GearModifier = new(common: -70, rare: 41, epic: 27, legendary: 2);
        private static readonly RarityModifier ConsumableModifier = new(common: -55, rare: 36, epic: 20, legendary: -1);

        private TreasureReward_TE Reward => TileEntity.ByID.TryGetValue(RewardID, out var te) ? te as TreasureReward_TE : null;

        public override void SetDefaults()
        {
            base.SetDefaults();
            isRevealed = true;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override void AI()
        {
            NPC.ShowNameOnHover = false;

            if (UpdateCleanup()) return;

            var reward = Reward;
            if (reward == null || reward.Claimed)
            {
                NPC.active = false;
                return;
            }

            UpdateOpening();
            UpdateChest();
        }

        protected override int[] RollRewards() => Reward.Offers ??= RollOffers(GearModifier, ConsumableModifier);

        protected override void OnRewardClaimed() => Reward.Claimed = true;
    }
}
