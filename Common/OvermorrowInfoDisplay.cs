using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common
{
    public class OvermorrowInfoDisplay : GlobalInfoDisplay
    {
        public override bool? Active(InfoDisplay currentDisplay)
        {
            foreach (NPC npc in Main.npc) if (npc.type == NPCID.EyeofCthulhu && npc.active)
            {
                return false;
            }
           
            return base.Active(currentDisplay);
        }
    }
}