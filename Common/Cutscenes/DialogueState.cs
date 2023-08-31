using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Quests;
using Terraria.Audio;
using OvermorrowMod.Common.NPCs;
using System;

namespace OvermorrowMod.Common.Cutscenes
{
    public partial class DialogueState : UIState
    {
        private int DrawTimer;
        private int DelayTimer;

        const float DIALOGUE_DELAY = 30;
        const float MAXIMUM_LENGTH = 500;

        private const int WIDTH = 650;
        private const int HEIGHT = 300;

        public int textIterator = 0;

        private DummyPanel DrawSpace = new DummyPanel();
        private UIText Text = new UIText("");

        /// <summary>
        /// The starting ID of the dialogue that the XML traverser starts at
        /// </summary>
        public string dialogueID = "start";
        public bool drawQuest = false;
        public bool shouldRedraw = true;

        public int questCounter = 0;
        public int continueButtonCounter = 0; // i have this stupid thing because for some reason i delete all the buttons every tick
        public override void OnInitialize()
        {
            ModUtils.AddElement(DrawSpace, Main.screenWidth / 2 - WIDTH, Main.screenHeight / 2 - HEIGHT, WIDTH * 2, HEIGHT * 2, this);
            base.OnInitialize();
        }

        public bool canInteract = true;
        public int interactDelay = 0;

        public bool hasInitialized = false;
        float arrowOffset;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Main.LocalPlayer.talkNPC <= -1 || Main.playerInventory || player.GetDialogue() == null)
            {
                player.ClearDialogue();
                player.LockPlayer = false;

                ResetTimers();
                SetID("start");
                hasInitialized = false;

                return;
            }

            if (!hasInitialized)
            {
                SetQuestDialogue();
                hasInitialized = true;
            }

            LockPlayer();
            HandlePlayerInteraction();

            DrawBackdrop(player, spriteBatch);

            if (DelayTimer++ >= DIALOGUE_DELAY)
            {
                if (DrawTimer < player.GetDialogue().drawTime) DrawTimer++;

                DrawText(player, spriteBatch, new Vector2(0, 0));
            }

            // Handles the drawing of the UI after the dialogue has finished drawing
            if (DrawTimer >= player.GetDialogue().drawTime)
            {                    
                Dialogue dialogue = player.GetDialogue();

                var npc = Main.npc[Main.LocalPlayer.talkNPC];
                var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                // Draw the continue icon if there is more text to be read
                if (dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "ContinueIcon").Value;
                    Vector2 arrowPosition = new Vector2(Main.screenWidth / 2f + 265, Main.screenHeight / 3f + 50);
                    
                    arrowOffset = MathHelper.Lerp(10, 0, (float)(Math.Sin(continueButtonCounter++ / 20f) / 2 + 0.5f));
                    spriteBatch.Draw(texture, arrowPosition + new Vector2(0, 10 + arrowOffset), null, Color.White * 0.75f, MathHelper.ToRadians(90), texture.Size() / 2f, 1f, 0, 0);

                    canInteract = true;
                }
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            Dialogue dialogue = player.GetDialogue();

            if (dialogue == null) return;

            if (Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                this.RemoveAllChildren(); // Removes the options and then readds the elements back
                canInteract = true;

                

                //Main.NewText(dialogue.GetTextIteration() + " / " + (dialogue.GetTextListLength() - 1));

                // This shit keeps breaking everything if I move it so I don't care anymore, it's staying here
                int optionNumber = 1;
                if (DrawTimer < player.GetDialogue().drawTime || dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1) return;

                canInteract = false;

                if (dialogue.GetTextIteration() >= dialogue.GetTextListLength() - 1 && dialogue.GetOptions(dialogueID) == null)
                {
                    canInteract = true;
                }
                else
                {
                    //Main.NewText("yes");
                    canInteract = false;
                }

                if (player.GetDialogue() != null && player.GetDialogue().GetOptions(dialogueID) != null && !drawQuest)
                {
                    foreach (OptionButton button in player.GetDialogue().GetOptions(dialogueID))
                    {
                        Vector2 position = OptionPosition(optionNumber);
                        ModUtils.AddElement(button, (int)position.X, (int)position.Y, 375, 45, this);

                        optionNumber++;
                    }
                }

                shouldRedraw = false;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Handles player input for the UI and when the UI can receive player input
        /// </summary>
        private void HandlePlayerInteraction()
        {
            if (!canInteract) return;

            if ((Main.mouseLeft || ModUtils.CheckKeyPress()) && interactDelay == 0 && DelayTimer >= DIALOGUE_DELAY)
            {
                DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
                Dialogue dialogue = player.GetDialogue();

                interactDelay = 10;

                if (DrawTimer < dialogue.drawTime)
                {
                    DrawTimer = dialogue.drawTime;
                }
                else
                {
                    if (dialogue.GetTextIteration() >= dialogue.GetTextListLength() - 1 && dialogue.GetOptions(dialogueID) == null)
                        ExitText();
                    else            
                        AdvanceText();          
                }
            }

            if (interactDelay > 0) interactDelay--;
        }

        /// <summary>
        /// Determines the position that the Option button will be drawn at based on the id.
        /// </summary>
        private Vector2 OptionPosition(int optionNumber)
        {
            Vector2 screenPosition = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f);
            Vector2 offsets = new Vector2(600, 180) / 2f; // This is the size of the dialogue box

            return screenPosition + offsets + new Vector2(-600, -35 + (60 * optionNumber - 1));
        }

        private string ParseColoredText(string text)
        {
            string displayText = text;

            // The number of opening brackets MUST be the same as the number of closing brackets
            int numOpen = 0;
            int numClose = 0;

            int openSquareBrackets = 0;
            int closedSquareBrackets = 0;

            int openCurlyBrackets = 0;
            int closedCurlyBrackets = 0;

            // Create a new string, adding in hex tags whenever an opening bracket is found
            var builder = new StringBuilder();
            //builder.Append("    "); // Appends a tab to the beginning of the string

            foreach (var character in displayText)
            {
                switch (character)
                {
                    case '[':
                        openSquareBrackets++;
                        builder.Append("[c/34c9eb:");
                        break;             
                    case '{':
                        openCurlyBrackets++;
                        builder.Append("[c/f8595f:");
                        break;
                    case ']':
                        closedSquareBrackets++;
                        builder.Append(character);
                        break;
                    case '}':
                        closedCurlyBrackets++;
                        builder.Append("]");
                        break;
                    default:
                        builder.Append(character);
                        break;
                }
                /*if (character == '[') // Insert the hex tag if an opening bracket is found
                {
                    //builder.Append("[c/" + player.GetDialogue().bracketColor + ":");
                    builder.Append("[c/34c9eb:");
                    numOpen++;
                }
                else
                {
                    if (character == ']')
                    {
                        numClose++;
                    }

                    builder.Append(character);
                }*/
            }

            if (openSquareBrackets != closedSquareBrackets)
            {
                builder.Append(']');
            }

            if (openCurlyBrackets != closedCurlyBrackets)
            {
                builder.Append(']');
            }

            // Final check for if the tag has two brackets but no characters inbetween which does weird things
            //var hexTag = "[c/" + player.GetDialogue().bracketColor + ":]";
            var hexTag = "[c/34c9eb:]";
            if (builder.ToString().Contains("[c/34c9eb:]") )
            {
                builder.Replace(hexTag, "[c/34c9eb: ]");
            }

            hexTag = "[c/FFA500:]";
            if (builder.ToString().Contains("[c/FFA500:]"))
            {
                builder.Replace(hexTag, "[c/FFA500: ]");
            }

            displayText = builder.ToString();

            return displayText;
        }

        /// <summary>
        /// Handles drawing the text for the UI
        /// </summary>
        private void DrawText(DialoguePlayer player, SpriteBatch spriteBatch, Vector2 textPosition)
        {
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            string text = player.GetDialogue().GetText();
            int progress = (int)MathHelper.Lerp(0, player.GetDialogue().GetText().Length, DrawTimer / (float)player.GetDialogue().drawTime);

            var displayText = ParseColoredText(text.Substring(0, progress));
            TextSnippet[] snippets = ChatManager.ParseMessage(displayText, Color.White).ToArray();

            float MAX_LENGTH = 400;
            Vector2 offsets = new Vector2(-125, -60);
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f) + offsets, Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAX_LENGTH);
        }
    }

    public class OptionButton : UIElement
    {
        private string icon;
        private string displayText;
        private string linkID;
        private string action;

        private int itemID;
        private string itemName;
        private int stack;

        public OptionButton(string icon, string displayText, string linkID, string action)
        {
            this.icon = icon;
            this.displayText = displayText;
            this.linkID = linkID;
            this.action = action;
        }

        // TODO: Make these not stupid
        /*/// <summary>
        /// <para>Used to handle button with a vanilla item action. Vanilla uses static ids for their items which can be passed directly.</para> 
        /// For modded items, the name of the file must be used instead.
        /// </summary>
        /// <param name="displayText">The text displayed on the dialogue option.</param>
        /// <param name="itemID">The STATIC ID for the vanilla item.</param>
        /// <param name="stack">The number of items given, defaults to 1.</param>
        public OptionButton(string displayText, string linkID, int itemID, int stack = 1)
        {
            this.displayText = displayText;
            this.linkID = linkID;
            this.itemID = itemID;
            this.itemName = null;
            this.stack = stack;
            this.action = "item";
        }

        /// <summary>
        /// <para>Used to handle button with a modded item action. Modded items can be found by their file name string.</para> 
        /// For vanilla items, the id of the item must be used instead.
        /// </summary>
        /// <param name="displayText">The text displayed on the dialogue option.</param>
        /// <param name="itemName">The FILE NAME for the modded item.</param>
        /// <param name="stack">The number of items given, defaults to 1.</param>
        public OptionButton(string displayText, string linkID, string itemName, int stack = 1)
        {
            this.displayText = displayText;
            this.linkID = linkID;
            this.itemName = itemName;
            this.itemID = -1;
            this.stack = stack;
            this.action = "item";
        }*/

        public string GetText() => displayText;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            Color color = new Color(22, 25, 62);

            if (isHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                color = new Color(32, 35, 78);
                //spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White * 0.75f, 0, texture.Size() / 2f, new Vector2(0.55f, 0.5f), 0, 0);
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "TrackerPanel").Value;

            float height_padding = 30;
            float height = Height.Pixels/* + height_padding*/;
            Vector2 position = new Vector2(GetDimensions().X, GetDimensions().Center().Y - (height / 2));
            Rectangle drawRectangle = new Rectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)height);

            //ModUtils.DrawNineSegmentTexturePanel(spriteBatch, texture, drawRectangle, 35, Color.White * 0.6f);
            Utils.DrawInvBG(Main.spriteBatch, drawRectangle, color * 0.925f);

            Texture2D dialogueIcon = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChatIcon").Value;
            switch (icon)
            {
                case "quest":
                    dialogueIcon = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_QuestIcon").Value;
                    break;
                case "chest":
                    dialogueIcon = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_ChestIcon").Value;
                    break;
                case "sword":
                    dialogueIcon = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_SwordIcon").Value;
                    break;
            }

            spriteBatch.Draw(dialogueIcon, pos + new Vector2(dialogueIcon.Width + 12, dialogueIcon.Height + 10), null, Color.White, 0f, texture.Size() / 2f, 1f, 0, 0);


            Utils.DrawBorderString(spriteBatch, displayText, pos + new Vector2(64, 12), Color.White);
        }

        public static int NPCToShop(int type)
        {
            switch (type)
            {
                case 17: return 1;
                case 19: return 2;
                case 20: return 3;
                case 38: return 4;
                case 54: return 5;
                case 107: return 6;
                case 108: return 7;
                case 124: return 8;
                case 142: return 9;
                case 160: return 10;
                case 178: return 11;
                case 207: return 12;
                case 208: return 13;
                case 209: return 14;
                case 227: return 15;
                case 228: return 16;
                case 229: return 17;
                case 353: return 18;
                case 368: return 19;
                case 453: return 20;
                case 550: return 21;
                default: return 22;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            //if (Parent.Parent is DialogueState parent)
            if (Parent is DialogueState parent)
            {
                parent.ResetTimers();

                if (action != "none")
                {
                    DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
                    QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                    NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                    QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

                    var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                    switch (action)
                    {
                        case "item":
                            if (itemID != -1 && itemName == null)
                                Main.LocalPlayer.QuickSpawnItem(null, itemID, stack);
                            else if (itemName != null && itemID == -1)
                                Main.LocalPlayer.QuickSpawnItem(null, OvermorrowModFile.Instance.Find<ModItem>(itemName).Type, stack);
                            //Main.LocalPlayer.SetTalkNPC(-1);
                            break;
                        case "marker":
                            break;
                        case "shop":
                            int type = Main.npc[Main.LocalPlayer.talkNPC].type;

                            Main.player[Main.myPlayer].chest = -1;
                            Main.npcChatText = "";

                            int shop = NPC.NewNPC(null, (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<ShopHandler>(), Main.LocalPlayer.whoAmI);
                            Main.LocalPlayer.SetTalkNPC(shop);

                            Main.recBigList = false;
                            Main.playerInventory = true;
                            Main.InGuideCraftMenu = false;
                            Main.InReforgeMenu = false;

                            // Sets the player to the NPC's shop ID, currently DOESNT check for modded NPCs yet
                            Main.SetNPCShopIndex(NPCToShop(type));

                            Main.instance.shop[Main.npcShop].SetupShop(Main.npcShop < Main.MaxShopIDs - 1 ? Main.npcShop : type);

                            return;
                        case "quest":
                            questPlayer.AddQuest(quest);
                            questNPC.TakeQuest();

                            if (quest.QuestName == "Rekindle the Flame")
                            {
                                dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popup + "GuideCampAxe.xml"));
                            }

                            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/QuestAccept")
                            {
                                Volume = 0.9f,
                                PitchVariance = 0.2f,
                                MaxInstances = 3,
                            }, npc.Center);

                            // Run the Quest Accepted UI
                            Main.NewText("ACCEPTED QUEST: " + quest.QuestName, Color.Yellow);

                            parent.ExitText();
                            return;
                        case "quest_complete":
                            var baseQuest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out _);
                            questPlayer.CompleteQuest(quest.QuestID);

                            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/QuestTurnIn")
                            {
                                Volume = 0.9f,
                                PitchVariance = 0.2f,
                                MaxInstances = 3,
                            }, npc.Center);

                            // Run the Quest Complete UI
                            Main.NewText("COMPLETED QUEST: " + quest.QuestName, Color.Yellow);

                            parent.ExitText();
                            return;
                        case "exit":
                            parent.ExitText();
                            return;
                    }
                }

                if (!parent.drawQuest)
                {
                    parent.SetID(linkID);
                    return;
                }
            }
        }
    }
}