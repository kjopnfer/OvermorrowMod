using OvermorrowMod.Content.Tiles.Archives;
using System;
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
    }
}