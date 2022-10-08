using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Text;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Audio;
using ReLogic.Utilities;

namespace OvermorrowMod.Common.Cutscenes
{
    public class PopupState : UIState
    {
        const float OPEN_TIME = 15;
        const float CLOSE_TIME = 10;
        const float MAXIMUM_LENGTH = 280;
        const float DIALOGUE_DELAY = 30;

        private int xPosition = 235;
        private int yPosition = Main.screenHeight - 375/*169*/;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            if (player.GetQueueLength() <= 0) return;

            Popup currentPopup = player.GetPopup();
            Vector2 textPosition = new Vector2(xPosition - 95, yPosition - 25);
            //DrawPopup(spriteBatch, player, currentPopup);
            currentPopup.DrawPopup(spriteBatch, player);


            //if (DrawTimer < currentPopup.GetDrawTime() && OpenTimer >= OPEN_TIME)
            if (currentPopup.DrawTimer < currentPopup.GetDrawTime() && currentPopup.OpenTimer >= OPEN_TIME)
            {
                if (currentPopup.DelayTimer++ < DIALOGUE_DELAY) return;
                if (!Main.gamePaused) currentPopup.DrawTimer++;

                //DrawText(spriteBatch, player, textPosition, currentPopup);
                currentPopup.DrawText(spriteBatch, player, textPosition);
            }
            else // Hold the dialogue for the amount of time specified
            {
                if (currentPopup.DrawTimer < currentPopup.GetDrawTime()) return;

                if (SoundEngine.TryGetActiveSound(currentPopup.drawSound, out var result)) result.Stop();

                if (currentPopup.HoldTimer <= currentPopup.GetDisplayTime())
                {
                    if (!Main.gamePaused) currentPopup.HoldTimer++;

                    currentPopup.HoldText(spriteBatch, player, textPosition);
                }
                else
                {
                    if (!Main.gamePaused) currentPopup.CloseTimer++;
                    if (!currentPopup.ShouldClose()) currentPopup.CloseTimer = (int)CLOSE_TIME;

                    // Remove the dialogue from the list and reset counters
                    if (currentPopup.CloseTimer == CLOSE_TIME)
                    {
                        if (currentPopup.GetNodeIteration() < currentPopup.GetListLength() - 1)
                        {
                            currentPopup.GetNextNode();
                        }
                        else
                        {
                            player.RemovePopup();
                        }

                        //ResetTimers();
                        currentPopup.ResetTimers();
                    }
                }
            }
        }
    }
}