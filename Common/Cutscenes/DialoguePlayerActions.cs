using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System.Xml;
using OvermorrowMod.Core;
using OvermorrowMod.Quests;
using System.Linq;
using OvermorrowMod.Quests.ModQuests;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using Terraria.ID;
using OvermorrowMod.Content.WorldGeneration;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialoguePlayer : ModPlayer
    {
        public bool pickupWood = false;
        public bool outDistanceDialogue = false;
        public bool guideGreeting = false;
        public bool kittFirst = true;

        public bool unlockedGuideCampfire = false;

        private int greetCounter = 0;
        private int guideCampfireCounter = 0;

        public bool reachedSlimeCave = false;

        public bool feydenExitCave = false;
        public bool feydenSojournClose = false;

        QuestPlayer questPlayer => Player.GetModPlayer<QuestPlayer>();
        DialoguePlayer dialoguePlayer => Player.GetModPlayer<DialoguePlayer>();

        private void GeneralUpdateDialogue()
        {
            if (Player.Center.X >= GuideCamp.SlimeCaveEntrance.X - (150 * 16) && !dialoguePlayer.reachedSlimeCave)
            {
                dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popups + "FeydenHelp.xml"));
            }
        }
    }
}
