using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Core;
using System.Xml;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.Dialogue
{
    public class MoxleyManager : DialogueManager
    {
        public override int NPC => ModContent.NPCType<Moxley>();

        public override XmlDocument GetDialogueWindow()
        {
            XmlDocument doc = new XmlDocument();
            string text = System.Text.Encoding.UTF8.GetString(OvermorrowModFile.Instance.GetFileBytes(AssetDirectory.DialogWindow + "MoxleyIntro.xml"));
            doc.LoadXml(text);

            return doc;
        }
    }
}