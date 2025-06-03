using OvermorrowMod.Common.Detours;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Tiles.Archives;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;
namespace OvermorrowMod.Core
{
    public class OvermorrowModSystem : ModSystem
    {
        public static int ArchiveTiles;
        
        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            ArchiveTiles = tileCounts[TileType<CastleBrick>()] + tileCounts[TileType<ArchiveWood>()] + tileCounts[TileType<CastlePlatform>()];
        }

        public override void ResetNearbyTileEffects()
        {
            ArchiveTiles = 0;
        }

        public override void PreUpdateEntities()
        {
            if (!Main.dedServ && !Main.gamePaused && !Main.gameInactive && !Main.gameMenu)
            {
                ParticleManager.UpdateParticles();
            }
        }

        public override void PostUpdateEverything()
        {
            PrimitiveManager.UpdateTrails();
        }
    }
}