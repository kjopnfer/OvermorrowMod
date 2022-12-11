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
using Microsoft.Xna.Framework.Input;

namespace OvermorrowMod.Common.Cutscenes
{
    /// <summary>
    /// Literally just an invisible UIPanel to draw the buttons and content on
    /// </summary>
    internal class DummyPanel : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime); // don't remove.
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            //spriteBatch.Draw(texture, GetDimensions().ToRectangle(), Color.White);
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1, 0, 0);

        }
    }

    public class DialogueState : UIState
    {
        private int DrawTimer;
        private int DelayTimer;

        const float DIALOGUE_DELAY = 30;
        const float MAXIMUM_LENGTH = 500;

        private const int WIDTH = 650;
        private const int HEIGHT = 300;

        public int textIterator = 0;

        private DummyPanel DrawSpace = new DummyPanel();
        private UIPanel BackPanel = new UIPanel();
        private UIText Text = new UIText("");
        private UIImage Portrait = new UIImage(ModContent.Request<Texture2D>(AssetDirectory.Empty));

        public string dialogueID = "start";
        public bool drawQuest = false;
        public bool shouldRedraw = true;

        public int questCounter = 0;

        public override void OnInitialize()
        {
            ModUtils.AddElement(DrawSpace, Main.screenWidth / 2 - WIDTH, Main.screenHeight / 2 - HEIGHT, WIDTH * 2, HEIGHT * 2, this);
            //ModUtils.AddElement(BackPanel, Main.screenWidth / 2, Main.screenHeight / 2, WIDTH, HEIGHT - 100, DrawSpace);
            //ModUtils.AddElement(Text, 0, 0, 650, 300, DrawSpace);
            //ModUtils.AddElement(Portrait, 0, 0, 650, 300, DrawSpace);

            base.OnInitialize();
        }

        public bool canInteract = true;
        public int interactDelay = 0;
        private void HandlePlayerInteraction()
        {
            if (!canInteract) return;

            Main.NewText("awaiting actions");
            //Main.NewText(DrawTimer + " / " + Main.LocalPlayer.GetModPlayer<DialoguePlayer>().GetDialogue().drawTime);

            if ((Main.mouseLeft || ModUtils.CheckKeyPress()) && interactDelay == 0 && DelayTimer >= DIALOGUE_DELAY)
            {
                interactDelay = 10;

                if (DrawTimer < Main.LocalPlayer.GetModPlayer<DialoguePlayer>().GetDialogue().drawTime)
                {
                    DrawTimer = Main.LocalPlayer.GetModPlayer<DialoguePlayer>().GetDialogue().drawTime;
                    Main.NewText("complete text during drawing");
                }
                else
                {
                    AdvanceText();
                    Main.NewText("advance to next dialogue after drawing");
                }
            }

            //KeyboardState ks = Main.keyState;
            //Main.NewText(ks.GetPressedKeys());

            if (interactDelay > 0) interactDelay--;
        }

        private void AdvanceText()
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            player.GetDialogue().IncrementText();

            ResetTimers();
            shouldRedraw = true;
        }

        private void LockPlayer()
        {
            Player player = Main.LocalPlayer;

            player.mouseInterface = true;
            player.immune = true;
            player.immuneTime = 60;
            player.immuneNoBlink = true;

            player.GetModPlayer<DialoguePlayer>().LockPlayer = true;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Main.LocalPlayer.talkNPC <= -1 || Main.playerInventory || player.GetDialogue() == null)
            {
                player.ClearDialogue();
                player.LockPlayer = false;

                ResetTimers();
                SetID("start");
                if (drawQuest)
                {
                    drawQuest = false;
                    questCounter = 0;
                }

                return;
            }

            LockPlayer();
            HandlePlayerInteraction();

            DrawBackdrop(player, spriteBatch);

            if (DelayTimer++ >= DIALOGUE_DELAY)
            {
                if (DrawTimer < player.GetDialogue().drawTime) DrawTimer++;

                DrawText(player, spriteBatch, new Vector2(0, 0));
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            Dialogue dialogue = player.GetDialogue();

            if (dialogue == null) return;

            if (shouldRedraw && Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                // Removes the options and then readds the elements back
                this.RemoveAllChildren();

                canInteract = true;

                // Handles the drawing of the UI after the dialogue has finished drawing
                if (DrawTimer >= player.GetDialogue().drawTime)
                {
                    var npc = Main.npc[Main.LocalPlayer.talkNPC];
                    var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                    if (!drawQuest)
                    {
                        if (quest != null && dialogueID == "start" && dialogue.GetTextIteration() == 0)
                        {
                            ModUtils.AddElement(new QuestButton(), 575, 50, 50, 25, DrawSpace);
                        }

                        // Determines which button type is shown in the bottom right corner
                        if (dialogue.GetTextIteration() >= dialogue.GetTextListLength() - 1 && dialogue.GetOptions(dialogueID) == null)
                            ModUtils.AddElement(new ExitButton(), (int)(Main.screenWidth / 2f) + 225, (int)(Main.screenHeight / 2f) - 75, 50, 25, this);
                        else if (dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1)
                        {
                            //Main.NewText("add next button");
                            canInteract = true;

                            ModUtils.AddElement(new NextButton(), (int)(Main.screenWidth / 2f) + 225, (int)(Main.screenHeight / 2f) - 75, 50, 25, this);
                        }
                    }
                    /*else // The UI drawing when the player has clicked on the Quest button
                    {
                        if (!isDoing)
                        {
                            if (questCounter < quest.DialogueCount - 1)
                                ModUtils.AddElement(new NextButton(), 575, 145, 50, 25, DrawSpace);

                            if (questCounter == quest.DialogueCount - 1)
                            {
                                for (int i = 1; i <= 2; i++)
                                {
                                    Vector2 position = OptionPosition(i);
                                    string questText = i == 1 ? "Accept" : "Decline";
                                    //ModUtils.AddElement(new OptionButton(questText, "none"), (int)position.X, (int)position.Y, 285, 75, DrawSpace);
                                }
                            }
                        }
                        else
                        {
                            //Main.NewText("player is doing a quest");
                            var state = Quests.Quests.State.GetActiveQuestState(Main.LocalPlayer.GetModPlayer<QuestPlayer>(), quest);
                            if (quest.CheckRequirements(Main.LocalPlayer.GetModPlayer<QuestPlayer>(), state))
                            {
                                if (questCounter < quest.EndDialogueCount - 1)
                                    ModUtils.AddElement(new NextButton(), 575, 145, 50, 25, DrawSpace);

                                if (questCounter == quest.EndDialogueCount - 1)
                                {
                                    Vector2 position = OptionPosition(1);
                                    //ModUtils.AddElement(new OptionButton("Turn In", "none"), (int)position.X, (int)position.Y, 285, 75, DrawSpace);
                                }
                            }
                        }
                    }*/
                }

                // This shit keeps breaking everything if I move it so I don't care anymore, it's staying here
                int optionNumber = 1;
                if (DrawTimer < player.GetDialogue().drawTime || dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1) return;

                canInteract = false;
                Main.NewText("waiting for player to select option");

                if (player.GetDialogue() != null && player.GetDialogue().GetOptions(dialogueID) != null && !drawQuest)
                {
                    foreach (OptionButton button in player.GetDialogue().GetOptions(dialogueID))
                    {
                        Vector2 position = OptionPosition(optionNumber);
                        ModUtils.AddElement(button, (int)position.X, (int)position.Y, 285, 75, this);

                        optionNumber++;
                    }
                }

                shouldRedraw = false;
            }

            base.Update(gameTime);
        }

        private Vector2 OptionPosition(int optionNumber)
        {
            Vector2 screenPosition = new Vector2(Main.screenWidth, Main.screenHeight) / 2f;

            switch (optionNumber)
            {
                case 1:
                    return screenPosition + new Vector2(-300, 25);
                case 2:
                    return screenPosition + new Vector2(0, 25);
                case 3:
                    return screenPosition + new Vector2(-300, 150);
                case 4:
                    return screenPosition + new Vector2(0, 150);
            }

            return new Vector2(0, 0);
        }

        private void DrawText(DialoguePlayer player, SpriteBatch spriteBatch, Vector2 textPosition)
        {
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            string text = player.GetDialogue().GetText(dialogueID);
            if (drawQuest)
            {
                if (!isDoing)
                {
                    text = quest.GetDialogue(questCounter);
                }
                else
                {
                    // The namespace is named the same thing as the variable lol
                    var state = Quests.Quests.State.GetActiveQuestState(Main.LocalPlayer.GetModPlayer<QuestPlayer>(), quest);
                    if (quest.CheckRequirements(Main.LocalPlayer.GetModPlayer<QuestPlayer>(), state))
                    {
                        text = quest.GetEndDialogue(questCounter);
                    }
                    else
                    {
                        text = quest.GetHint(Main.rand.Next(0, quest.HintCount - 1));
                    }
                }
            }

            int progress = (int)MathHelper.Lerp(0, player.GetDialogue().GetText(dialogueID).Length, DrawTimer / (float)player.GetDialogue().drawTime);
            if (drawQuest) progress = (int)MathHelper.Lerp(0, text.Length, DrawTimer / (float)player.GetDialogue().drawTime); // TODO: This needs to be changed

            var displayText = text.Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (player.GetDialogue().bracketColor != null)
            {
                // The number of opening brackets MUST be the same as the number of closing brackets
                int numOpen = 0;
                int numClose = 0;

                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    "); // Appends to the beginning of the string

                foreach (var character in displayText)
                {
                    if (character == '[') // Insert the hex tag if an opening bracket is found
                    {
                        builder.Append("[c/" + player.GetDialogue().bracketColor + ":");
                        numOpen++;
                    }
                    else
                    {
                        if (character == ']')
                        {
                            numClose++;
                        }

                        builder.Append(character);
                    }
                }

                if (numOpen != numClose)
                {
                    builder.Append(']');
                }

                // Final check for if the tag has two brackets but no characters inbetween
                var hexTag = "[c/" + player.GetDialogue().bracketColor + ":]";
                if (builder.ToString().Contains(hexTag))
                {
                    builder.Replace(hexTag, "[c/" + player.GetDialogue().bracketColor + ": ]");
                }

                displayText = builder.ToString();
            }

            TextSnippet[] snippets = ChatManager.ParseMessage(displayText, Color.White).ToArray();

            float MAX_LENGTH = 400;
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f) + new Vector2(-150, -200), Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAX_LENGTH);
            //Text.SetText(displayText);
        }

        private void DrawBackdrop(DialoguePlayer player, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            spriteBatch.Draw(texture, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f), null, Color.White * 0.75f, 0, texture.Size() / 2f, new Vector2(1.25f, 1), 0, 0);

            Texture2D speaker = player.GetDialogue().speakerBody;
            Vector2 offset = new Vector2(-200, -20);
            spriteBatch.Draw(speaker, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f) + offset, null, Color.White, 0, speaker.Size() / 2f, 1f, 0, 0);
            //Portrait.SetImage(player.GetDialogue().speakerBody);
        }

        public void ResetTimers()
        {
            DrawTimer = 0;
            DelayTimer = 0;
        }

        public void SetID(string id)
        {
            Text.SetText("");

            shouldRedraw = true;
            dialogueID = id;

            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            Dialogue dialogue = player.GetDialogue();

            if (dialogue == null) return;
            dialogue.UpdateList(id);
        }
    }

    public class QuestButton : UIElement
    {
        public QuestButton() { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Quest", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            //if (Parent.Parent is DialogueState parent)
            if (Parent is DialogueState parent)
            {
                Main.NewText("quest BUTTON");
                parent.ResetTimers();
                parent.shouldRedraw = true;
                parent.drawQuest = true;
            }
        }
    }

    public class NextButton : UIElement
    {
        public NextButton() { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Next", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            //if (Parent.Parent is DialogueState parent)
            if (Parent is DialogueState parent)
            {
                DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

                if (parent.drawQuest)
                    parent.questCounter++;
                else
                    player.GetDialogue().IncrementText();

                parent.ResetTimers();
                parent.shouldRedraw = true;

                //Main.NewText("incrementing counter " + player.GetDialogue().GetTextIteration() + " / " + player.GetDialogue().GetTextListLength());
            }
        }
    }

    public class ExitButton : UIElement
    {
        public ExitButton() { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Close", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent is DialogueState parent)
            {
                parent.ResetTimers();
                parent.SetID("start");

                Main.LocalPlayer.SetTalkNPC(-1);
            }
        }
    }

    public class OptionButton : UIElement
    {
        private string displayText;
        private string linkID;
        private string action;

        private int itemID;
        private string itemName;
        private int stack;

        public OptionButton(string displayText, string linkID, string action)
        {
            this.displayText = displayText;
            this.linkID = linkID;
            this.action = action;
        }

        /// <summary>
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
        }


        public string GetText() => displayText;

        public override void Draw(SpriteBatch spriteBatch)
        {

            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White * 0.75f, 0, texture.Size() / 2f, new Vector2(0.55f, 0.5f), 0, 0);

            Utils.DrawBorderString(spriteBatch, displayText, pos + new Vector2(25, 0), Color.White);
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

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            //if (Parent.Parent is DialogueState parent)
            if (Parent is DialogueState parent)
            {
                parent.ResetTimers();

                if (action != "none")
                {
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
                    }
                }

                if (!parent.drawQuest)
                {
                    parent.SetID(linkID);
                    //Main.NewText("changing id to " + linkID);
                    return;
                }

                /*NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
                QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                QuestNPC questNPC = npc.GetGlobalNPC<QuestNPC>();

                var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

                switch (displayText)
                {
                    case "Accept":
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

                        Main.LocalPlayer.SetTalkNPC(-1);
                        break;
                    case "Decline":
                        Main.LocalPlayer.SetTalkNPC(-1);
                        break;
                    case "Turn In":
                        questPlayer.CompleteQuest(quest.QuestID);
                        SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/QuestTurnIn")
                        {
                            Volume = 0.9f,
                            PitchVariance = 0.2f,
                            MaxInstances = 3,
                        }, npc.Center);

                        Main.LocalPlayer.SetTalkNPC(-1);
                        break;
                }*/
            }
        }
    }
}