using OvermorrowMod.Content.NPCs.Archives;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.NPCs
{
    /// <summary>
    /// Archives biome pools. Initialize rebuilds contents from current world flags.
    /// </summary>
    public static class ArchiveSpawnPool
    {
        public static readonly SpawnPool BaseGroundPool = new();
        public static readonly SpawnPool WallPool = new();

        public static void Initialize()
        {
            Clear();

            BaseGroundPool.Entries.Add(new PoolEntry(ModContent.NPCType<ArchiveRat>(), 1.0f));
            WallPool.Entries.Add(new PoolEntry(ModContent.NPCType<BlasterBook>(), 2.0f));
        }

        public static void Clear()
        {
            BaseGroundPool.Entries.Clear();
            WallPool.Entries.Clear();
        }
    }
}
