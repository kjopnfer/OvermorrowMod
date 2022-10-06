using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using System;
using OvermorrowMod.Core;
using Terraria.ID;

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
                    if (dialoguePlayer.outDistanceDialogue) return base.PreAI(npc);

                    float xDistance = Math.Abs(npc.Center.X - player.Center.X);
                    if (xDistance > 25 * 16 && playerInRange)
                    {
                        dialoguePlayer.outDistanceDialogue = true;

                        Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Guide/GuideSmug", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        dialoguePlayer.AddPopup(texture, "Where are you off to in such a hurry?", 60, 120, new Color(52, 201, 235), true, true);
                    }
                    else if (xDistance < 25 * 16) // The dialogue should run if the player has FIRST been within 25 blocks and THEN they leave
                    {
                        playerInRange = true;
                    }
                }
            }

            return base.PreAI(npc);
        }
    }
}