namespace OvermorrowMod.Core.Loot
{
    public abstract class LootPool
    {
        public abstract RarityWeights BaseWeights { get; }

        /// <summary>
        /// When false, items tagged with the wildcard <see cref="LootAttribute"/>
        /// and the vanilla wildcard set are not fanned out into this pool.
        /// </summary>
        public virtual bool AcceptsWildcards => true;
    }
}
