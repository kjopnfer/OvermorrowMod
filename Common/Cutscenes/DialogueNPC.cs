using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using OvermorrowMod.Core;
using Terraria.ID;
using System.Xml;

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private bool playerInRange = false;
        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.Guide)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active) continue;

                    DialoguePlayer dialoguePlayer = player.GetModPlayer<DialoguePlayer>();

                    // Don't run the dialogue if the player has already had the interaction
                    /*if (dialoguePlayer.outDistanceDialogue) return base.PreAI(npc);

                    float xDistance = Math.Abs(npc.Center.X - player.Center.X);
                    if (xDistance > 25 * 16 && playerInRange && player.velocity.X != 0)
                    {
                        dialoguePlayer.outDistanceDialogue = true;

                        XmlDocument doc = ModUtils.GetXML(AssetDirectory.Popup + "GuideHurry.xml");
                        dialoguePlayer.AddPopup(doc);
                    }
                    else if (xDistance < 25 * 16) // The dialogue should run if the player has FIRST been within 25 blocks and THEN they leave
                    {
                        playerInRange = true;
                    }*/
                }
            }

            return base.PreAI(npc);
        }
    }
}