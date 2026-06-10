namespace OvermorrowMod.Core.Items.Collectibles
{
    public readonly struct CollectibleBonus
    {
        public readonly CollectibleStat Stat;
        public readonly float Amount;

        public CollectibleBonus(CollectibleStat stat, float amount)
        {
            Stat = stat;
            Amount = amount;
        }
    }
}
