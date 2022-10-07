using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        public bool pickupWood = false;
        public bool outDistanceDialogue = false;
        public bool guideGreeting = false;

        private int greetCounter = 0;
        public override void PostUpdateBuffs()
        {
            DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (!dialoguePlayer.guideGreeting && greetCounter++ == 180)
            {
                XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideGreeting.xml");
                dialoguePlayer.AddPopup(doc);
                //dialoguePlayer.AddPopup(texture, "Hello there! That's an unfamiliar face, you must be new around here.", 90, 120, new Color(52, 201, 235), true, true);
                dialoguePlayer.guideGreeting = true;
            }

            base.PostUpdateBuffs();
        }
    }
}
