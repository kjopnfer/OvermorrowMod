using OvermorrowMod.Core;
using System.Xml;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class GuideManager : DialogueManager
    {
        public override int NPC => NPCID.Guide;

        public override DialogueWindow GetDialogueWindow()
        {
            return new GuideCamp();
        }
    }
}