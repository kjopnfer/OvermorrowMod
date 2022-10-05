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

        public override bool PreAI(NPC npc)
        {
            if (npc.type == NPCID.Guide)
            {
                foreach (Player player in Main.player)
                {
                    if (!player.active) continue;

                    DialoguePlayer dialoguePlayer = player.GetModPlayer<DialoguePlayer>();

                    if (dialoguePlayer.distanceGuide) return base.PreAI(npc);

                    float xDistance = Math.Abs(npc.Center.X - player.Center.X);
                    if (xDistance > 15 * 16)
                    {
                        dialoguePlayer.distanceGuide = true;

                        Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "Portraits/Guide/GuideSmug", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        dialoguePlayer.AddPopup(texture, "Where are you off to in such a hurry?", 60, 120, new Color(52, 201, 235), true, true);
                    }
                }
            }

            return base.PreAI(npc);
        }
    }
}