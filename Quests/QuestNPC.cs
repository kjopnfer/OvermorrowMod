using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Quests
{
    public class QuestNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public Quest CurrentQuest = null;

        public override bool PreChatButtonClicked(NPC npc, bool firstButton)
        {
            if (npc.type == NPCID.Guide)
            {
                if (firstButton)
                {
                    return false;
                }
            }

            return base.PreChatButtonClicked(npc, firstButton);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.GetGlobalNPC<QuestNPC>().CurrentQuest != null)
            {
                Texture2D texture = ModContent.GetTexture("OvermorrowMod/Quests/QuestAlert");
                Rectangle drawRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
                spriteBatch.Draw(texture, new Vector2(npc.Center.X, npc.Center.Y - 80) - Main.screenPosition, drawRectangle, Color.White, npc.rotation, new Vector2(drawRectangle.Width / 2, drawRectangle.Height / 2), 1f, SpriteEffects.None, 0f);
            }

            base.PostDraw(npc, spriteBatch, drawColor);
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == NPCID.Guide)
            {
                if (npc.GetGlobalNPC<QuestNPC>().CurrentQuest == null && Main.LocalPlayer.GetModPlayer<QuestPlayer>().CurrentQuest != null)
                //if (npc.GetGlobalNPC<QuestNPC>().CurrentQuest != null && /*npc.GetGlobalNPC<QuestNPC>().CurrentQuest.IsCompleted*/npc.GetGlobalNPC<QuestNPC>().CurrentQuest.CheckCompleted(Main.LocalPlayer))
                {
                    if (Main.LocalPlayer.GetModPlayer<QuestPlayer>().CurrentQuest.CheckCompleted(Main.LocalPlayer))
                    {
                        // Get the Quest ID to determine the Completion dialogue
                        chat = "Nice job! Here, have this, its… (To be expanded once the reward is decided)";
                    }
                }
                else
                {
                    List<string> dialogue = new List<string>
                    {
                        "dialogue1",
                        "dialogue2",
                        "dialogue3",
                        "dialogue4",
                        "dialogue5",
                        "dialogue6",
                        "dialogue7",
                        "dialogue8",
                        "dialogue9",
                    };

                    chat = Main.rand.Next(dialogue);
                }
            }
        }
    }
}