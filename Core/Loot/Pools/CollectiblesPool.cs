namespace OvermorrowMod.Core.Loot.Pools
{
    public sealed class CollectiblesPool : LootPool
    {
        public override RarityWeights BaseWeights => new(common: 55, rare: 30, epic: 12, legendary: 3);

        public override bool AcceptsWildcards => false;
    }
}
