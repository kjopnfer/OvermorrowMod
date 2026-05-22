using System.Collections.Generic;

namespace OvermorrowMod.Core.NPCs
{
    public enum SpawnTier { Common, Elite }

    public readonly record struct PoolEntry(int NpcType, float Threat, SpawnTier Tier = SpawnTier.Common);

    public class SpawnPool
    {
        public List<PoolEntry> Entries { get; } = new();
    }
}
