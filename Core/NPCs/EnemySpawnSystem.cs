using Terraria.ModLoader;

namespace OvermorrowMod.Core.NPCs
{
    public class EnemySpawnSystem : ModSystem
    {
        public override void PostSetupContent() => ArchiveSpawnPool.Initialize();

        public override void Unload() => ArchiveSpawnPool.Clear();
    }
}
