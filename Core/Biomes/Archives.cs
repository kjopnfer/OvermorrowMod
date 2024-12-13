using OvermorrowMod.Content.Backgrounds;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace OvermorrowMod.Core.Biomes
{
    public class Archives : ModBiome
    {
        // Declare biome priority. The default is BiomeLow so this is only necessary if it needs a higher priority.
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<ArchivesBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ArchivesUndergroundBackgroundStyle>();
        public override bool IsBiomeActive(Player player)
        {
            return OvermorrowModSystem.ArchiveTiles >= 50;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["OM:ArchiveSky"] != null && isActive != SkyManager.Instance["OM:ArchiveSky"].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate("OM:ArchiveSky");
                else
                    SkyManager.Instance.Deactivate("OM:ArchiveSky");

            }

            base.SpecialVisuals(player, isActive);
        }
    }
}