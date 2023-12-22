using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Biomes
{
    public class Ravensfell : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("OvermorrowMod/RavensfellBGStyle");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsBiomeActive(Player player)
        {
            return true;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {          
            if (SkyManager.Instance["OM:RavensfellSky"] != null && isActive != SkyManager.Instance["OM:RavensfellSky"].IsActive())
            {
                if (isActive)
                    SkyManager.Instance.Activate("OM:RavensfellSky");
                else
                    SkyManager.Instance.Deactivate("OM:RavensfellSky");
                
            }

            base.SpecialVisuals(player, isActive);
        }
    }
}