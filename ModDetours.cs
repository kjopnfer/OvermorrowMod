using Microsoft.Xna.Framework;
using OvermorrowMod.Quests;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod
{
    public static class ModDetours
    {

        public static void Load()
        {
            On.Terraria.Main.DrawNPCChatButtons += DrawNPCChatButtons;
        }

        public static void Unload()
        {
            On.Terraria.Main.DrawNPCChatButtons -= DrawNPCChatButtons;
        }

        public static bool HoverButton = false;
        public static bool NextButton = false;
        public static bool AcceptButton = false;
        public static int DialogueCounter = 0;
        private static void DrawNPCChatButtons(On.Terraria.Main.orig_DrawNPCChatButtons orig, int superColor, Color chatColor, int numLines, string focusText, string focusText3)
        {
            if (Main.LocalPlayer.talkNPC != -1)
            {
                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                Player player = Main.LocalPlayer;
                QuestPlayer modPlayer = player.GetModPlayer<QuestPlayer>();
                Quest CurrentQuest = npc.GetGlobalNPC<QuestNPC>().CurrentQuest;

                // Text changes for the Quest button
                string Text = "";
                if (!NextButton)
                {
                    if (CurrentQuest != null)
                    {
                        Text = "Quest";
                    }
                    else
                    {
                        foreach (Quest quest in modPlayer.QuestList)
                        {
                            if (quest.QuestGiver() == npc.type)
                            {
                                if (quest.IsCompleted)
                                {
                                    Text = "Turn In";
                                }
                                else
                                {
                                    Text = "Quest";
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (AcceptButton)
                    {
                        Text = "Accept";
                    }
                    else
                    {
                        Text = "Next";
                    }
                }

                #region Text
                DynamicSpriteFont font = Main.fontMouseText;
                Color TextColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
                Vector2 TextScale = new Vector2(0.9f);
                Vector2 StringSize = ChatManager.GetStringSize(font, Text, TextScale);
                Vector2 TextPosition = new Vector2((180 + Main.screenWidth / 2) + StringSize.X - 50f, 130 + numLines * 30);
                Vector2 MouseCursor = new Vector2(Main.mouseX, Main.mouseY);
                #endregion

                // Check for Help button
                if (Main.npcChatFocus2 && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    var HelpDialogue = new List<string>
                        {
                            "helpdialogue1",
                            "helpdialogue2",
                            "helpdialogue3",
                            "helpdialogue4",
                            "helpdialogue5"
                        };

                    Main.npcChatText = Main.rand.Next(HelpDialogue);
                }

                #region Quest Button/Dialogue
                if (MouseCursor.Between(TextPosition, TextPosition + StringSize * TextScale) && !PlayerInput.IgnoreMouseInterface)
                {
                    Main.LocalPlayer.mouseInterface = true;
                    Main.LocalPlayer.releaseUseItem = true;

                    // Plays the sound once
                    if (!HoverButton)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        HoverButton = true;
                    }

                    TextScale *= 1.1f;

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        if (CurrentQuest != null)
                        {
                            #region Quest Available
                            // This changes it to the 'Next' button
                            NextButton = true;

                            if (!AcceptButton)
                            {
                                // Loop through the Quest's dialogue options
                                if (DialogueCounter < CurrentQuest.QuestDialogue.Count)
                                {
                                    Main.PlaySound(SoundID.MenuTick);
                                    Main.npcChatText = CurrentQuest.GetDialogue(DialogueCounter++);

                                    if (DialogueCounter == CurrentQuest.QuestDialogue.Count)
                                    {
                                        AcceptButton = true;
                                    }
                                }
                            }
                            else
                            {
                                // Add the thing to the player's list of Quests
                                modPlayer.QuestList.Add(CurrentQuest);
                                Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestAccept"), npc.Center);

                                // Run the Quest Accepted UI
                                Main.NewText("ACCEPTED QUEST: " + CurrentQuest.QuestName(), Color.Yellow);
                                npc.GetGlobalNPC<QuestNPC>().CurrentQuest = null;

                                Main.LocalPlayer.talkNPC = -1;
                            }
                            #endregion
                        }
                        else
                        {
                            #region Quest Accepted
                            // Convert the list to prevent collection modification error
                            foreach (Quest quest in modPlayer.QuestList.ToList())
                            {
                                if (quest.QuestGiver() == npc.type)
                                {
                                    if (quest.IsCompleted) // Completion dialogue
                                    {
                                        Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestTurnIn"), npc.Center);
                                        Main.NewText("COMPLETED QUEST: " + quest.QuestName(), Color.Yellow);

                                        // Do reward shenanigans
                                        quest.GiveRewards(player);
                                        if (quest.QuestID() == (int)Quest.ID.TutorialGuideQuest)
                                        {
                                            foreach (Quest newQuest in OvermorrowModFile.QuestList)
                                            {
                                                if (newQuest.QuestID() == (int)Quest.ID.GuideHouseQuest)
                                                {
                                                    npc.GetGlobalNPC<QuestNPC>().CurrentQuest = newQuest;
                                                }
                                            }
                                        }

                                        OvermorrowModFile.CompletedQuests.Add(quest);
                                        modPlayer.QuestList.Remove(quest);

                                        Main.LocalPlayer.talkNPC = -1;
                                    }
                                    else // Hint dialogue
                                    {
                                        if (DialogueCounter < quest.HintDialogue.Count)
                                        {
                                            Main.PlaySound(SoundID.MenuTick);
                                            Main.npcChatText = quest.GetHint(DialogueCounter++);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                else
                {
                    // Plays the sound once
                    if (HoverButton)
                    {
                        Main.PlaySound(SoundID.MenuTick);
                        HoverButton = false;
                    }
                }
                #endregion

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, Text, TextPosition + new Vector2(16f, 14f), TextColor, 0f,
                      StringSize * 0.5f, TextScale * new Vector2(1f));
            }

            orig(superColor, chatColor, numLines, focusText, focusText3);
        }
    }
}
