using Microsoft.Xna.Framework;
using OvermorrowMod.NPCs;
using OvermorrowMod.NPCs.Bosses.SandstormBoss;
using OvermorrowMod.Quests;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
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
            On.Terraria.Player.Update_NPCCollision += UpdateNPCCollision;
        }

        public static void Unload()
        {
            On.Terraria.Main.DrawNPCChatButtons -= DrawNPCChatButtons;
            On.Terraria.Player.Update_NPCCollision -= UpdateNPCCollision;
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

        private static void UpdateNPCCollision(On.Terraria.Player.orig_Update_NPCCollision orig, Player self)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.modNPC is CollideableNPC && npc.active && npc.modNPC != null)
                {
                    Rectangle PlayerBottom = new Rectangle((int)self.position.X, (int)self.position.Y + self.height, self.width, 1);
                    Rectangle NPCTop = new Rectangle((int)npc.position.X, (int)npc.position.Y - (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));

                    if (PlayerBottom.Intersects(NPCTop))
                    {
                        if (self.position.Y <= npc.position.Y && !self.justJumped && self.velocity.Y >= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y - self.height + 4;
                            self.position += npc.velocity;
                            self.velocity.Y = 0;
                            self.fallStart = (int)(self.position.Y / 16f);

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            if (npc.type == ModContent.NPCType<Pillar>())
                            {
                                self.Hurt(PlayerDeathReason.ByCustomReason(self.name + " had an obelisk stuck up their ass"), 20, -1);
                            }

                            orig(self);
                        }
                    }

                    Rectangle PlayerLeft = new Rectangle((int)self.position.X, (int)self.position.Y, 1, self.height);
                    Rectangle PlayerRight = new Rectangle((int)self.position.X + self.width, (int)self.position.Y, 1, self.height);

                    Rectangle NPCRight = new Rectangle((int)npc.position.X + npc.width, (int)npc.position.Y, 8 + (int)Math.Max(self.velocity.X, 0), npc.height);
                    Rectangle NPCLeft = new Rectangle((int)npc.position.X, (int)npc.position.Y, 10 - (int)Math.Max(self.velocity.X, 0), npc.height);

                    if (PlayerLeft.Intersects(NPCRight))
                    {
                        if (self.position.X >= npc.position.X + npc.width && self.velocity.X <= 0)
                        {
                            int offset = npc.modNPC is PushableNPC ? 7 : 2;
                            self.velocity.X = 0;
                            self.position.X = npc.position.X + npc.width + offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }
                        
                        if (npc.modNPC is PushableNPC)
                        {
                            npc.position.X -= 1;
                        } 
                    }

                    if (PlayerRight.Intersects(NPCLeft))
                    {
                        if (npc.modNPC is PushableNPC)
                        {
                            npc.position.X += 1;
                        }

                        if (self.position.X <= npc.position.X && self.velocity.X >= 0)
                        {
                            int offset = npc.modNPC is PushableNPC ? 2 : 0;

                            self.velocity.X = 0;
                            self.position.X = npc.position.X - self.width - offset;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);

                            //orig(self);
                        }        
                    }

                    Rectangle PlayerTop = new Rectangle((int)self.position.X, (int)self.position.Y, self.width, 1);
                    Rectangle NPCBottom = new Rectangle((int)npc.position.X, (int)npc.position.Y + npc.height + (int)npc.velocity.Y, npc.width, 8 + (int)Math.Max(self.velocity.Y, 0));
                    if (PlayerTop.Intersects(NPCBottom))
                    {
                        if (self.position.Y <= npc.position.X && self.velocity.Y <= 0)
                        {
                            self.gfxOffY = npc.gfxOffY;
                            self.position.Y = npc.position.Y + npc.height + 4;
                            self.position += npc.velocity;
                            self.velocity.Y = 0;

                            if (self == Main.LocalPlayer)
                                NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.LocalPlayer.whoAmI);
                        }
                    }
                }
            }

            orig(self);
        }
    }
}
