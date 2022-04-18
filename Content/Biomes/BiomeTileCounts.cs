using OvermorrowMod.Content.Tiles.DesertTemple;
using OvermorrowMod.Content.Tiles.Underground;
using OvermorrowMod.Content.Tiles.WaterCave;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Biomes
{
    public class BiomeTileCounts : ModSystem
    {
        public int FloodedCaves { get; private set; }
        public int MarbleBiome { get; private set; }
        public int GraniteBiome { get; private set; }
        public int LavaBiome { get; private set; }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            FloodedCaves = tileCounts[ModContent.TileType<GlowBlock>()];
            MarbleBiome = tileCounts[TileID.MarbleBlock];
            GraniteBiome = tileCounts[TileID.GraniteBlock];
            LavaBiome = tileCounts[ModContent.TileType<CrunchyStone>()];
            
            // Make the modded tile weigh more heavily
            Main.SceneMetrics.SandTileCount += tileCounts[ModContent.TileType<SandBrick>()] * 5;
        }
    }
}
