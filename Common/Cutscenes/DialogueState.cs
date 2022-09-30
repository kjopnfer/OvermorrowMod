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

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueState : UIState
    {
        private int DrawTimer;
        private int DelayTimer;

        const float DIALOGUE_DELAY = 30;
        const float MAXIMUM_LENGTH = 500;

        private float xPosition = Main.screenWidth / 2f - (MAXIMUM_LENGTH / 2f);
        private int yPosition = 375;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (Main.LocalPlayer.talkNPC <= -1 || Main.playerInventory)
            {
                DrawTimer = 0;
                DelayTimer = 0;
                player.AddedDialogue = false;

                return;
            }

            DrawBackdrop(spriteBatch, player);

            if (DelayTimer++ > DIALOGUE_DELAY)
            {
                if (DrawTimer < player.GetDialogue().drawTime) DrawTimer++;
                DrawText(spriteBatch, player, new Vector2(xPosition, yPosition));
            }

            //Main.npcChatText = "";


            base.Draw(spriteBatch);
        }

        private void DrawBackdrop(SpriteBatch spriteBatch, DialoguePlayer player)
        {
            Vector2 drawPosition = new Vector2(xPosition, yPosition);
            Texture2D backdrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "Chat_Back").Value;

            //spriteBatch.Draw(backdrop, drawPosition, null, Color.White, 0f, backdrop.Size() / 2, 1f, SpriteEffects.None, 1f);

            Texture2D speaker = player.GetDialogue().speakerBody;
            spriteBatch.Draw(speaker, drawPosition, null, Color.White, 0f, speaker.Size() / 2, 1f, SpriteEffects.None, 1f);
        }

        private void DrawText(SpriteBatch spriteBatch, DialoguePlayer player, Vector2 textPosition)
        {
            int progress = (int)MathHelper.Lerp(0, player.GetDialogue().displayText.Length, DrawTimer / (float)player.GetDialogue().drawTime);
            var text = player.GetDialogue().displayText.Substring(0, progress);

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

            TextSnippet[] snippets = ChatManager.ParseMessage(text, Color.White).ToArray();
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, 0f, Color.White, Color.Black, Vector2.Zero, Vector2.One, out int hoveredSnippet, MAXIMUM_LENGTH, 2f);
        }
    }

    public class OptionButton : UIElement
    {
        private string displayText;
        private Dialogue nextState;

        public OptionButton(string displayText, Dialogue nextState)
        {
            this.displayText = displayText;
            this.nextState = nextState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (isHovering)
            {
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White * 0.25f);
            }

            Utils.DrawBorderString(spriteBatch, displayText, pos, Color.White);
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuTick);
            Main.LocalPlayer.GetModPlayer<DialoguePlayer>().SetDialogue(nextState);
        }
    }
}