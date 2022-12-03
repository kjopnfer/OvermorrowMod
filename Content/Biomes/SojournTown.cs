using Microsoft.Xna.Framework;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Content.Tiles.Underground;
using System;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Biomes
{
    public class SojournTown : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Sounds/Music/StormDrake");

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sojourn");
        }

        public override bool IsBiomeActive(Player player)
        {
            return OvermorrowModSystem.SojournTiles >= 50;
        }
    }
}