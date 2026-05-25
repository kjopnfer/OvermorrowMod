namespace OvermorrowMod.Core.Loot.Pools
{
    public sealed class ArchivePool : LootPool
    {
        public override RarityWeights BaseWeights => new(common: 70, rare: 25, epic: 5);
    }
}
