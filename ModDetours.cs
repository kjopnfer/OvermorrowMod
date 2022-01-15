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

                // Get the Quest that has been assigned to the NPC
                Quest CurrentQuest = npc.GetGlobalNPC<QuestNPC>().CurrentQuest;

                // Text changes for the Quest button
                string Text = "";
                if (!NextButton)
                {
                    // Check if the NPC has a Quest, if they have a Quest run through the dialogue
                    if (CurrentQuest != null)
                    {
                        Text = "Quest";
                    }
                    else // Otherwise, the NPC has given the Quest to the player
                    {
                        // Check the Quest's completion
                        if (modPlayer.CurrentQuest.QuestGiver() == npc.type && modPlayer.CurrentQuest.CheckCompleted(player))
                        {
                            Text = "Turn In";
                        }
                        else
                        {
                            Text = "Quest";
                        }
                    }

                    /*if (CurrentQuest != null)
                    {
                        if (CurrentQuest.CheckCompleted(player) && CurrentQuest == player.GetModPlayer<QuestPlayer>().CurrentQuest)
                        {
                            Text = "Turn In";
                        }
                        else
                        {
                            Text = "Quest";
                        }
                    }*/
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

                DynamicSpriteFont font = Main.fontMouseText;
                Color TextColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
                Vector2 TextScale = new Vector2(0.9f);
                Vector2 StringSize = ChatManager.GetStringSize(font, Text, TextScale);
                Vector2 TextPosition = new Vector2((180 + Main.screenWidth / 2) + StringSize.X - 50f, 130 + numLines * 30);

                Vector2 MouseCursor = new Vector2(Main.mouseX, Main.mouseY);

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

                // Check if the NPC has a quest instance
                if (CurrentQuest != null)
                {
                    // Check for Quest Button
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

                        // Player clicks on the button
                        if (Main.mouseLeft && Main.mouseLeftRelease)
                        {
                            // Remove the Quest from the NPC if it is completed
                            if (/*CurrentQuest.IsCompleted*/CurrentQuest.CheckCompleted(player))
                            {
                                Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestTurnIn"), npc.Center);
                                Main.NewText("COMPLETED QUEST: " + CurrentQuest.QuestName(), Color.Yellow);

                                /*(int, int)[] Rewards = CurrentQuest.QuestRewards();
                                foreach ((int, int) Reward in Rewards)
                                {
                                    Main.LocalPlayer.QuickSpawnItem(Reward.Item1, Reward.Item2);
                                }*/

                                // Do reward shenanigans
                                CurrentQuest.GiveRewards(Main.LocalPlayer);

                                OvermorrowModFile.CompletedQuests.Add(CurrentQuest);
                                OvermorrowModFile.ActiveQuests.Remove(CurrentQuest);
                                npc.GetGlobalNPC<QuestNPC>().CurrentQuest = null;

                                Main.LocalPlayer.talkNPC = -1;
                            }
                            else // Quest is not complete
                            {
                                // Dialogue for when a Quest is accepted, but not yet complete
                                if (/*OvermorrowModFile.ActiveQuests.Contains(CurrentQuest)*/player.GetModPlayer<QuestPlayer>().CurrentQuest.QuestGiver() == npc.type)
                                {
                                    // Loop through the Quest's dialogue options
                                    if (DialogueCounter < CurrentQuest.HintDialogue.Count)
                                    {
                                        Main.PlaySound(SoundID.MenuTick);
                                        Main.npcChatText = CurrentQuest.GetHint(DialogueCounter++);
                                    }
                                }
                                else // Quest is not yet accepted
                                {
                                    // This changes it to the 'Next' button
                                    NextButton = true;

                                    // Before the button is changed to 'Accept,' scroll through the dialogue
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
                                        //OvermorrowModFile.ActiveQuests.Add(CurrentQuest);
                                        player.GetModPlayer<QuestPlayer>().SetQuest(CurrentQuest);

                                        Main.PlaySound(OvermorrowModFile.Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/QuestAccept"), npc.Center);

                                        // Remove the Quest from the NPC
                                        npc.GetGlobalNPC<QuestNPC>().CurrentQuest = null;

                                        // Run the Quest Accepted UI
                                        Main.NewText("ACCEPTED QUEST: " + CurrentQuest.QuestName(), Color.Yellow);
                                        Main.LocalPlayer.talkNPC = -1;
                                    }
                                }
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
                }

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, Text, TextPosition + new Vector2(16f, 14f), TextColor, 0f,
                      StringSize * 0.5f, TextScale * new Vector2(1f));
            }

            orig(superColor, chatColor, numLines, focusText, focusText3);
        }
    }
}
