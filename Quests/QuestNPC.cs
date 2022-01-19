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

        public override void GetChat(NPC npc, ref string chat)
        {
            if (npc.type == NPCID.Guide)
            {
                if (npc.GetGlobalNPC<QuestNPC>().CurrentQuest != null)
                {
                    chat = "hi i have a quest for u";
                }
                else if (npc.GetGlobalNPC<QuestNPC>().CurrentQuest == null)
                {
                    foreach (Quest quest in Main.LocalPlayer.GetModPlayer<QuestPlayer>().QuestList)
                    {
                        if (quest.QuestGiver() == npc.type)
                        {
                            if (quest.IsCompleted) // Completion dialogue
                            {
                                switch (quest.QuestID())
                                {
                                    case (int)Quest.ID.TutorialGuideQuest:
                                        chat = "Nice job! Here, have this, its… (To be expanded once the reward is decided)";
                                        break;
                                    case (int)Quest.ID.GuideHouseQuest:
                                        chat = "Well done! This house is pretty comfy! Well I think you're pretty ready now. I'd suggest you start exploring the world and the underground. There's a lot to find which will help you with your journey";
                                        break;
                                }
                            }
                            
                        }
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