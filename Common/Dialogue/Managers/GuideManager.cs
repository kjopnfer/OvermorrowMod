using OvermorrowMod.Core;
using System.Xml;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class GuideManager : DialogueManager
    {
        public override int NPC => NPCID.Guide;

        public override XmlDocument GetDialogueWindow()
        {
            XmlDocument doc = new XmlDocument();
            string text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(AssetDirectory.DialogWindow + "GuideCamp.xml"));
            doc.LoadXml(text);

            return doc;
        }
    }
}