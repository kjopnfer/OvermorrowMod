using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI.Chat;

namespace OvermorrowMod
{
    public static class ModDetours
    {
        public static bool HoverButton = false;

        public static void Load()
        {
            On.Terraria.Main.DrawNPCChatButtons += DrawNPCChatButtons;
        }

        public static void Unload()
        {
            On.Terraria.Main.DrawNPCChatButtons -= DrawNPCChatButtons;
        }

        private static void DrawNPCChatButtons(On.Terraria.Main.orig_DrawNPCChatButtons orig, int superColor, Color chatColor, int numLines, string focusText, string focusText3)
        {
            if (Main.LocalPlayer.talkNPC != -1)
            {
                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];

                if (npc.type == NPCID.Guide)
                {
                    focusText = "test123";
                    focusText3 = "obamna";
                }

                const string text = "cheese burger";
                DynamicSpriteFont font = Main.fontMouseText;
                Color TextColor = new Color(superColor, (int)(superColor / 1.1), superColor / 2, superColor);
                Vector2 TextScale = new Vector2(0.9f);
                Vector2 StringSize = ChatManager.GetStringSize(font, text, TextScale);
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

                /*if (Main.npcChatFocus1)
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
                }*/

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

                    if (Main.mouseLeft && Main.mouseLeftRelease)
                    {
                        var QuestDialogue = new List<string>
                        {
                            "questdialogue1",
                            "questdialogue2",
                            "questdialogue3",
                            "questdialogue4",
                            "questdialogue5"
                        };

                        Main.npcChatText = Main.rand.Next(QuestDialogue);

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

                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, text, TextPosition + new Vector2(16f, 14f), TextColor, 0f,
                        StringSize * 0.5f, TextScale * new Vector2(1f));
            }

            orig(superColor, chatColor, numLines, focusText, focusText3);
        }
    }
}
