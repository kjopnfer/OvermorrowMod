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

        private DummyPanel DrawSpace = new DummyPanel();
        private UIPanel BackPanel = new UIPanel();
        private UIText Text = new UIText("");
        private UIImage Portrait = new UIImage(ModContent.Request<Texture2D>(AssetDirectory.Empty));

        public string dialogueID = "start";
        public bool shouldRedraw = true;
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

            if (player.GetDialogue() == null) return;

            if (shouldRedraw && Main.LocalPlayer.talkNPC > -1 && !Main.playerInventory)
            {
                Main.NewText("redrawing");

                // Removes the options and then readds the elements back
                DrawSpace.RemoveAllChildren();
                ModUtils.AddElement(BackPanel, 0, -25, 650, 200, DrawSpace);
                ModUtils.AddElement(Text, 0, 0, 650, 300, DrawSpace);
                ModUtils.AddElement(Portrait, 0, 0, 650, 300, DrawSpace);

                int optionNumber = 1;
                if (player.GetDialogue() == null) Main.NewText("NULL");
                if (DrawTimer < player.GetDialogue().drawTime) return;

                //var test = player.GetDialogue().GetOptions(dialogueID);
                //if (player.GetDialogue().GetOptions(dialogueID) != null)

                if (player.GetDialogue() != null && player.GetDialogue().GetOptions(dialogueID) != null)
                {
                    foreach (OptionButton button in player.GetDialogue().GetOptions(dialogueID))
                    {
                        Vector2 position = OptionPosition(optionNumber);
                        ModUtils.AddElement(button, (int)position.X, (int)position.Y, 285, 75, DrawSpace);
                        Main.NewText("draw: " + button.GetText());

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
            int progress = (int)MathHelper.Lerp(0, player.GetDialogue().GetText(dialogueID).Length, DrawTimer / (float)player.GetDialogue().drawTime);
            var text = player.GetDialogue().GetText(dialogueID).Substring(0, progress);

            // If for some reason there are no colors specified don't parse the brackets
            if (player.GetDialogue().bracketColor != null)
            {
                // The number of opening brackets MUST be the same as the number of closing brackets
                int numOpen = 0;
                int numClose = 0;

                // Create a new string, adding in hex tags whenever an opening bracket is found
                var builder = new StringBuilder();
                builder.Append("    "); // Appends to the beginning of the string

                foreach (var character in text)
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

                text = builder.ToString();
            }

            Text.SetText(text);
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
        }
    }

    public class OptionButton : UIElement
    {
        private string displayText;
        private string linkID;

        public OptionButton(string displayText, string linkID)
        {
            this.displayText = displayText;
            this.linkID = linkID;
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

            Utils.DrawBorderString(spriteBatch, displayText, pos + new Vector2(0, 25), Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);

            // On the click action, go back into the parent and set the dialogue node to the one stored in here
            if (Parent.Parent is DialogueState parent)
            {
                parent.ResetTimers();
                parent.SetID(linkID);

                Main.NewText("changing id to " + linkID);
            }
        }
    }
}