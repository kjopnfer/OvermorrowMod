using System;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Content.Skies;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Detours;

namespace OvermorrowMod.Core
{
	public partial class OvermorrowModFile : Mod
	{
        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Particle.Load();
                DetourLoader.Load();
                LoadEffects();

                foreach (Type type in Code.GetTypes())
                {
                    Particle.TryRegisteringParticle(type);
                }

                // Activate this with ManageSpecialBiomeVisuals probably... 
                //Filters.Scene["OM:RavensfellSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0.3f).UseOpacity(0.5f), EffectPriority.Medium);
                SkyManager.Instance["OM:ArchiveSky"] = new ArchiveSky();
            }
        }

        public override void Unload()
        {
            Particle.Unload();
            DetourLoader.Unload();
            UnloadEffects();
        }
    }
}
