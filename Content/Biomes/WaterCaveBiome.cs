using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Biomes
{
    public class WaterCaveBiome : ModBiome
    {
        public override bool IsBiomeActive(Player player)
        {
            return ModContent.GetInstance<BiomeTileCounts>().FloodedCaves > 50;
        }

        public override int Music => SoundLoader.GetSoundSlot("OvermorrowMod/Sounds/Music/WaterBiomeMusic");
    }
}
