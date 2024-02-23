using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Quests;
using Terraria.Audio;
using OvermorrowMod.Common.NPCs;
using System;
using OvermorrowMod.Content.NPCs.Town.Sojourn;
using OvermorrowMod.Quests.ModQuests;
using OvermorrowMod.Common.Dialogue;

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

        private Vector2 _dialogueAnchor = new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f) - new Vector2(600, 180) / 2f;

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

        private DialogueWindow dialogueWindow => Main.LocalPlayer.GetModPlayer<DialoguePlayer>().GetDialogueWindow();
        private Text dialogueText => dialogueWindow.GetText(dialogueID, textIterator);
        private int dialogueTextLength => dialogueWindow.GetTexts(dialogueID).Length - 1;

        private const float PANEL_WIDTH = 300;
        private const float PANEL_HEIGHT = 90;
        public override void Draw(SpriteBatch spriteBatch)
        {
            _dialogueAnchor = new Vector2(Main.screenWidth / 2f, Main.screenHeight - PANEL_HEIGHT) - new Vector2(PANEL_WIDTH + 80, PANEL_HEIGHT + 35);

            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Main.LocalPlayer.talkNPC <= -1 || Main.playerInventory || player.GetDialogueWindow() == null)
            {
                player.ClearWindow();
                player.LockPlayer = false;

                ExitText();

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
                if (DrawTimer < dialogueText.drawTime) DrawTimer++;

                DrawText(player, spriteBatch, new Vector2(0, 0));
            }

            // Handles the drawing of the UI after the dialogue has finished drawing
            if (DrawTimer >= dialogueText.drawTime)
            {
                var npc = Main.npc[Main.LocalPlayer.talkNPC];
                var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                // Draw the continue icon if there is more text to be read
                if (textIterator < dialogueWindow.GetTexts(dialogueID).Length - 1)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "ContinueIcon").Value;
                    Vector2 arrowPosition = _dialogueAnchor;

                    arrowOffset = MathHelper.Lerp(10, 0, (float)(Math.Sin(continueButtonCounter++ / 20f) / 2 + 0.5f));
                    Vector2 panelOffset = new Vector2(PANEL_WIDTH + 65, PANEL_HEIGHT) * 2;
                    spriteBatch.Draw(texture, arrowPosition + panelOffset + new Vector2(-24, -28 + arrowOffset), null, Color.White * 0.75f, MathHelper.ToRadians(90), texture.Size() / 2f, 1f, 0, 0);

                    canInteract = true;
                }
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (dialogueWindow == null) return;

            if (Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                this.RemoveAllChildren(); // Removes the options and then readds the elements back
                canInteract = true;

                // This shit keeps breaking everything if I move it so I don't care anymore, it's staying here
                int optionNumber = 1;
                if (DrawTimer < dialogueText.drawTime || textIterator < dialogueTextLength) return;

                canInteract = false;

                if (textIterator >= dialogueTextLength && dialogueWindow.GetOptions(dialogueID) == null)
                {
                    canInteract = true;
                }
                else
                {
                    //Main.NewText("yes");
                    canInteract = false;
                }

                if (dialogueWindow != null && dialogueWindow.GetOptions(dialogueID) != null && !drawQuest)
                {
                    foreach (DialogueChoice choice in dialogueWindow.GetOptions(dialogueID))
                    {
                        OptionButton button = new OptionButton(choice.texture, choice.text, choice.link, choice.dialogueAction);

                        Vector2 position = GetOptionPosition(optionNumber);
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
                interactDelay = 10;

                if (DrawTimer < dialogueText.drawTime)
                {
                    DrawTimer = dialogueText.drawTime;
                }
                else
                {
                    // If there are no dialogue options left, then make it so clicking will exit out of the dialogue completely
                    if (textIterator >= dialogueTextLength && dialogueWindow.GetOptions(dialogueID) == null)
                        ExitText();
                    else
                        AdvanceText();
                }
            }

            if (interactDelay > 0) interactDelay--;
        }

        /// <summary>
        /// Handles drawing the text for the UI
        /// </summary>
        private void DrawText(DialoguePlayer player, SpriteBatch spriteBatch, Vector2 textPosition)
        {
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            string text = dialogueText.text;
            int progress = (int)MathHelper.Lerp(0, text.Length, Utils.Clamp(DrawTimer / (float)dialogueText.drawTime, 0, 1));

            var displayText = ParseColoredText(text.Substring(0, progress));
            TextSnippet[] snippets = ChatManager.ParseMessage(displayText, Color.White).ToArray();

            float MAX_LENGTH = 500;
            Vector2 offsets = new Vector2(PANEL_WIDTH - 112, 24);
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, _dialogueAnchor + offsets, Color.White, 0f, Vector2.Zero, Vector2.One * 1.1f, out var hoveredSnippet, MAX_LENGTH);
        }
    }

    public class OptionButton : UIElement
    {
        private Texture2D icon;
        private string displayText;
        private string linkID;
        private Action<Player, NPC> action;

        public OptionButton(Texture2D icon, string displayText, string linkID, Action<Player, NPC> action)
        {
            this.icon = icon;
            this.displayText = displayText;
            this.linkID = linkID;
            this.action = action;
        }

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

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "TEMP").Value;
            spriteBatch.Draw(texture, GetDimensions().Center() + new Vector2(40, 0), null, Color.White * 0.75f, 0, texture.Size() / 2f, 1f, 0, 0);
            //Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "TrackerPanel").Value;

            //float height_padding = 30;
            float height = Height.Pixels/* + height_padding*/;
            Vector2 position = new Vector2(GetDimensions().X, GetDimensions().Center().Y - (height / 2));
            Rectangle drawRectangle = new Rectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)height);

            //ModUtils.DrawNineSegmentTexturePanel(spriteBatch, texture, drawRectangle, 35, Color.White * 0.6f);
            //Utils.DrawInvBG(Main.spriteBatch, drawRectangle, color * 0.925f);

            spriteBatch.Draw(icon, pos + new Vector2(icon.Width - 6, icon.Height - 8), null, Color.White, 0f, icon.Size() / 2f, 1f, 0, 0);
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

            // TODO: This shit is gonna SUCK in the long run, gotta fix it

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent is DialogueState parent)
            {
                parent.ResetTimers();

                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                if (action != null) action.Invoke(Main.LocalPlayer, npc);

                if (linkID != null)
                {
                    if (!parent.drawQuest)
                    {
                        parent.SetDialogueID(linkID);
                        return;
                    }
                }
                else
                {
                    parent.ExitText();
                    return;
                }


                /*if (action != "none")
                {
                    DialoguePlayer dialoguePlayer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
                    QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();

                    if (Main.LocalPlayer.talkNPC == -1) return;

                    NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                    QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

                    var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);
                    bool isFeyden = (npc.ModNPC is Feyden); // Temporary

                    switch (action)
                    {
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
                        case "feyden_escort":
                            if (quest.QuestName == "Rekindle the Flame") dialoguePlayer.AddNPCPopup(NPCID.Guide, ModUtils.GetXML(AssetDirectory.Popups + "GuideCampAxe.xml"));
                            if (isFeyden)
                            {
                                questPlayer.SetTravelLocation(quest, "sojourn_travel");
                                questPlayer.CompleteQuest(questPlayer.GetQuestID<FeydenRescue>());
                                var feyden = npc.ModNPC as Feyden;
                                feyden.followPlayer = questPlayer.Player;

                                // TODO: complete the cave quest here
                                //questPlayer.CompleteQuest("");
                            }

                            questPlayer.AddQuest(quest);
                            questNPC.TakeQuest();

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
                            if (isFeyden && quest is FeydenEscort)
                            {
                                Main.NewText("reset npc tracking");

                                var feyden = npc.ModNPC as Feyden;
                                feyden.followPlayer = null;
                            }

                            if (rewardIndex != "none") // Provide the index of the reward to the method 
                                questPlayer.CompleteQuest(quest.QuestID, rewardIndex);
                            else  // If the quest doesn't offer a choose your own reward, use default behavior
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
                    parent.SetDialogueID(linkID);
                    return;
                }*/
            }
        }
    }
}