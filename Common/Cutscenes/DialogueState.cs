using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;
using OvermorrowMod.Core;

namespace OvermorrowMod.Common.Cutscenes
{
    public class DialogueState : UIState
    {
        private UIElement DialogueBox;
        private UIText Name;
        private UIText Dialogue;
        public UIImage BackDrop;

        private int DialogueTimer;
        private int SecondaryTimer;

        public override void OnInitialize()
        {
            DialogueBox = new UIElement();
            DialogueBox.Width.Set(600f, 0f);
            DialogueBox.HAlign = .5f;
            DialogueBox.Top.Set(169, 0f);

            BackDrop = new UIImage(ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag"));
            BackDrop.Left.Set(0, 0f);
            BackDrop.Top.Set(0, 0f);

            Name = new UIText("", 1f);
            Name.Top.Set(20, 0f);
            Name.Left.Set(130, 0f);

            Dialogue = new UIText("", 1f);
            Dialogue.Top.Set(60, 0f);
            Dialogue.Left.Set(143, 0f);

            DialogueBox.Append(BackDrop);
            DialogueBox.Append(Name);
            DialogueBox.Append(Dialogue);
            Append(DialogueBox);
        }

        // This determines whether the UI is shown or not
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.LocalPlayer.GetModPlayer<DialoguePlayer>().DialogueList.Count > 0)
            {
                BackDrop.Draw(spriteBatch);
                Name.Draw(spriteBatch);
                Dialogue.Draw(spriteBatch);
            }

            /*int xPosition = (int)(Main.screenWidth * Main.UIScale) / 2;
            int yPosition = Main.screenHeight - 100;

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.Textures + "GamerTag").Value;
            spriteBatch.Draw(backDrop, new Vector2(xPosition + 65, yPosition), null, Color.White, 0f, backDrop.Size() / 2, 1f, SpriteEffects.None, 1f);

            base.Draw(spriteBatch);*/
        }

        // This handles the dialogue that the player has, if it detects that the player has new dialogue then it starts drawing it
        public override void Update(GameTime gameTime)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (player.DialogueList.Count > 0)
            {
                Name.SetText(player.DialogueList[0].speakerName);
                Name.TextColor = player.DialogueList[0].speakerColor;

                // Draw out the entire dialogue or something
                if (DialogueTimer++ < player.DialogueList[0].drawTime)
                {
                    // We need to detect if any color coded text is present, if it is then skip forward by the progression
                    int progress = (int)MathHelper.Lerp(0, player.DialogueList[0].displayText.Length, DialogueTimer / (float)player.DialogueList[0].drawTime);
                    Dialogue.SetText(player.DialogueList[0].displayText.Substring(0, progress));
                }
                else // Hold the dialogue for the amount of time specified
                {
                    //Main.NewText("HOLD" + SecondaryTimer);
                    if (SecondaryTimer++ <= player.DialogueList[0].showTime)
                    {
                        Dialogue.SetText(player.DialogueList[0].displayText);

                        // Remove the dialogue from the list and reset counters
                        if (SecondaryTimer == player.DialogueList[0].showTime)
                        {
                            Main.NewText("REMOVED");

                            player.DialogueList.RemoveAt(0);
                            DialogueTimer = 0;
                            SecondaryTimer = 0;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }
    }
}