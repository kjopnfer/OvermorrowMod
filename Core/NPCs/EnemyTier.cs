namespace OvermorrowMod.Core.NPCs
{
    public enum EnemyTier { Common, Elite, Miniboss, Boss, Hazard }

    public interface ITieredEnemy
    {
        EnemyTier Tier { get; }
    }
}
