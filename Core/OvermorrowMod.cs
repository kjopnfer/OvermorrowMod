using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;
using Terraria;
using Terraria.ModLoader;
using OvermorrowMod.Content.Skies;

namespace OvermorrowMod.Core
{
	public class OvermorrowModFile : Mod
	{
        public static OvermorrowModFile Instance { get; set; }
        public OvermorrowModFile() => Instance = this;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Activate this with ManageSpecialBiomeVisuals probably... 
                //Filters.Scene["OM:RavensfellSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0.2f, 0f, 0.3f).UseOpacity(0.5f), EffectPriority.Medium);
                SkyManager.Instance["OM:ArchiveSky"] = new ArchiveSky();

            }
        }
    }
}
