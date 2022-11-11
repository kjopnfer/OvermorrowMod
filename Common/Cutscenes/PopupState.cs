using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.Audio;
using System.Collections.Generic;

namespace OvermorrowMod.Common.Cutscenes
{
    public class PopupState : UIState
    {
        const float OFFSET_DISTANCE = 125;

        private int xPosition = 235;
        private int yPosition = Main.screenHeight - 375/*169*/;

        private List<Popup> PopupList = new List<Popup>();

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            if (PopupList.Count < 3)
            {
                if (player.GetQueueLength() != 0)
                {
                    PopupList.Add(player.RemovePopup());
                }
            }

            int offset = 0;
            List<Popup> PopupRemoval = new List<Popup>(PopupList);
            foreach (Popup currentPopup in PopupList)
            {
                Vector2 textPosition = new Vector2(xPosition - 95, yPosition - 25 - (OFFSET_DISTANCE * offset));
                currentPopup.DrawPopup(spriteBatch, textPosition);

                offset++;
                if (currentPopup.DrawTimer < currentPopup.GetDrawTime() && currentPopup.OpenTimer >= currentPopup.OPEN_TIME)
                {
                    if (currentPopup.DelayTimer++ < currentPopup.DIALOGUE_DELAY) continue;
                    if (!Main.gamePaused) currentPopup.DrawTimer++;

                    currentPopup.DrawText(spriteBatch, textPosition);
                }
                else // Hold the dialogue for the amount of time specified
                {
                    if (currentPopup.DrawTimer < currentPopup.GetDrawTime()) continue;

                    if (SoundEngine.TryGetActiveSound(currentPopup.drawSound, out var result)) result.Stop();

                    if (currentPopup.HoldTimer <= currentPopup.GetDisplayTime())
                    {
                        if (!Main.gamePaused) currentPopup.HoldTimer++;

                        currentPopup.HoldText(spriteBatch, textPosition);
                    }
                    else
                    {
                        if (!Main.gamePaused) currentPopup.CloseTimer++;
                        if (!currentPopup.ShouldClose()) currentPopup.CloseTimer = (int)currentPopup.CLOSE_TIME;

                        // Remove the dialogue from the list and reset counters
                        if (currentPopup.CloseTimer == currentPopup.CLOSE_TIME)
                        {
                            if (currentPopup.GetNodeIteration() < currentPopup.GetListLength() - 1)
                            {
                                currentPopup.GetNextNode();
                                currentPopup.ResetTimers();
                            }
                            else
                            {
                                PopupRemoval.Remove(currentPopup);
                            }
                        }
                    }
                }

                //Main.NewText(offset + "=> delay:" + currentPopup.DelayTimer +  " / draw:" + currentPopup.DrawTimer + " / open:" + currentPopup.OpenTimer + " / hold: " + currentPopup.HoldTimer + " / close: " + currentPopup.CloseTimer);
            }

            PopupList = new List<Popup>(PopupRemoval);
        }
    }
}