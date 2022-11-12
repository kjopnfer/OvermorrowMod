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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Empty).Value;
            spriteBatch.Draw(texture, GetDimensions().ToRectangle(), Color.White);
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
            ModUtils.AddElement(DrawSpace, Main.screenWidth / 2 - (WIDTH / 2), Main.screenHeight / 2 - 250, WIDTH * 2, HEIGHT * 2, this);
            ModUtils.AddElement(BackPanel, Main.screenWidth / 2 - (WIDTH / 2), Main.screenHeight / 2 - 250, WIDTH, HEIGHT - 100, DrawSpace);
            ModUtils.AddElement(Text, 0, 0, 650, 300, DrawSpace);
            ModUtils.AddElement(Portrait, 0, 0, 650, 300, DrawSpace);

            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Main.LocalPlayer.talkNPC <= -1 || Main.playerInventory || player.GetDialogue() == null)
            {
                player.ClearDialogue();

                ResetTimers();
                SetID("start");
                if (drawQuest)
                {
                    drawQuest = false;
                    questCounter = 0;
                }

                return;
            }

            DrawBackdrop(player);

            if (DelayTimer++ >= DIALOGUE_DELAY)
            {
                if (DrawTimer < player.GetDialogue().drawTime) DrawTimer++;

                DrawText(player);
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
                //Main.NewText("redrawing");

                // Removes the options and then readds the elements back
                DrawSpace.RemoveAllChildren();
                ModUtils.AddElement(BackPanel, 0, -25, 650, 200, DrawSpace);
                ModUtils.AddElement(Text, 0, 0, 650, 300, DrawSpace);
                ModUtils.AddElement(Portrait, 0, 0, 650, 300, DrawSpace);

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
                            ModUtils.AddElement(new ExitButton(), 575, 145, 50, 25, DrawSpace);
                        else if (dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1)
                            ModUtils.AddElement(new NextButton(), 575, 145, 50, 25, DrawSpace);
                    }
                    else // The UI drawing when the player has clicked on the Quest button
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
                                    ModUtils.AddElement(new OptionButton(questText, "none"), (int)position.X, (int)position.Y, 285, 75, DrawSpace);
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
                                    ModUtils.AddElement(new OptionButton("Turn In", "none"), (int)position.X, (int)position.Y, 285, 75, DrawSpace);
                                }
                            }
                        }
                    }
                }

                // This shit keeps breaking everything if I move it so I don't care anymore, it's staying here
                int optionNumber = 1;
                if (DrawTimer < player.GetDialogue().drawTime || dialogue.GetTextIteration() < dialogue.GetTextListLength() - 1) return;

                if (player.GetDialogue() != null && player.GetDialogue().GetOptions(dialogueID) != null && !drawQuest)
                {
                    foreach (OptionButton button in player.GetDialogue().GetOptions(dialogueID))
                    {
                        Vector2 position = OptionPosition(optionNumber);
                        ModUtils.AddElement(button, (int)position.X, (int)position.Y, 285, 75, DrawSpace);
                        //Main.NewText("draw: " + button.GetText());

                        optionNumber++;
                    }
                }

                shouldRedraw = false;
            }

            base.Update(gameTime);
        }

        private Vector2 OptionPosition(int optionNumber)
        {
            switch (optionNumber)
            {
                case 1:
                    return new Vector2(0, 200);
                case 2:
                    return new Vector2(WIDTH / 2, 200);
                case 3:
                    return new Vector2(0, 300);
                case 4:
                    return new Vector2(WIDTH / 2, 300);
            }

            return new Vector2(0, 0);
        }

        private void DrawText(DialoguePlayer player)
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

            Text.SetText(displayText);
        }

        private void DrawBackdrop(DialoguePlayer player)
        {
            Portrait.SetImage(player.GetDialogue().speakerBody);
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
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Quest", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent.Parent is DialogueState parent)
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
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Next", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent.Parent is DialogueState parent)
            {
                DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

                if (parent.drawQuest)
                    parent.questCounter++;
                else
                    player.GetDialogue().IncrementText();

                parent.ResetTimers();
                parent.shouldRedraw = true;

                Main.NewText("incrementing counter " + player.GetDialogue().GetTextIteration() + " / " + player.GetDialogue().GetTextListLength());
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
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, "Close", pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent.Parent is DialogueState parent)
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

        public OptionButton(string displayText, string linkID, string action = "none")
        {
            this.displayText = displayText;
            this.linkID = linkID;
            this.action = action;
        }

        public string GetText() => displayText;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, displayText, pos /*+ new Vector2(0, 25)*/, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent.Parent is DialogueState parent)
            {
                parent.ResetTimers();

                if (action != "none")
                {
                    switch (action)
                    {
                        case "exit":
                            Main.LocalPlayer.SetTalkNPC(-1);
                            return;
                    }
                }

                if (!parent.drawQuest)
                {
                    parent.SetID(linkID);
                    Main.NewText("changing id to " + linkID);
                    return;
                }

                NPC npc = Main.npc[Main.LocalPlayer.talkNPC];
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
                }
            }
        }
    }
}