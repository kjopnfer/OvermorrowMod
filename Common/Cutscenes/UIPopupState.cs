using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.Audio;
using System.Collections.Generic;
using Terraria.ModLoader;
using OvermorrowMod.Core;
using Terraria.UI.Chat;
using Terraria.GameContent;

namespace OvermorrowMod.Common.Cutscenes
{
    public class UIPopupState : UIState
    {
        const float MAXIMUM_LENGTH = 280;
        const float OFFSET_DISTANCE = 125;

        private int xPosition = 235;
        private int yPosition = Main.screenHeight - 375/*169*/;

        private List<Popup> PopupList = new List<Popup>();

        /*public void DrawPopup(SpriteBatch spriteBatch, Vector2 textPosition)
        {
            if (OpenTimer == 0 && ShouldOpen()) SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PopupShow")
            {
                Volume = 1.25f,
                PitchVariance = 1.1f,
                MaxInstances = 2,
            }, Main.LocalPlayer.Center);

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialogueBack").Value;
            if (!ShouldOpen()) OpenTimer = (int)OPEN_TIME;
            float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(OpenTimer++, 0, OPEN_TIME) / OPEN_TIME);

            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
            float yScale = MathHelper.Lerp(0, 1, drawProgress);
            if (HoldTimer >= GetDrawTime())
            {
                xScale = 1;
                yScale = MathHelper.Lerp(1, 0, CloseTimer / 15f);
            }

            //spriteBatch.Draw(backDrop, new Vector2(xPosition + 48, yPosition + 7), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            spriteBatch.Draw(backDrop, new Vector2(textPosition.X + 96 + 30, textPosition.Y + 36), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);

            float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
            float xOffset = MathHelper.Lerp(-155, 0, drawProgress);
            if (HoldTimer >= GetDisplayTime())
            {
                //spriteBatch.Draw(GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
                spriteBatch.Draw(GetPortrait(), new Vector2(textPosition.X + 62 + xOffset, textPosition.Y + 14), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            }
            else
            {
                //spriteBatch.Draw(GetPortrait(), new Vector2(xPosition - 36 + xOffset, yPosition - 16), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
                spriteBatch.Draw(GetPortrait(), new Vector2(textPosition.X + 62 + xOffset, textPosition.Y + 14), null, Color.White, 0f, backDrop.Size() / 2, scale, SpriteEffects.None, 1f);
            }

            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

 */

        private void DrawPopup(SpriteBatch spriteBatch, PopupState popupState, Vector2 textPosition)
        {
            float drawProgress = ModUtils.EaseOutQuint(Utils.Clamp(popupState.OpenTimer, 0, popupState.OPEN_TIME) / popupState.OPEN_TIME);

            //float scale = MathHelper.Lerp(0.5f, 1f, drawProgress);
            float xOffset = MathHelper.Lerp(-155, 0, drawProgress);

            Texture2D backDrop = ModContent.Request<Texture2D>(AssetDirectory.UI + "PopupBack").Value;
            spriteBatch.Reload(SpriteSortMode.Immediate);

            Effect effect = OvermorrowModFile.Instance.Whiteout.Value;
            effect.Parameters["WhiteoutColor"].SetValue(Color.White.ToVector3());
            effect.Parameters["WhiteoutProgress"].SetValue(1 - drawProgress);
            effect.CurrentTechnique.Passes["Whiteout"].Apply();

            float xScale = MathHelper.Lerp(1.25f, 1, drawProgress);
            float yScale = MathHelper.Lerp(0, 1, drawProgress);

            if (popupState.CanClose) // Case for drawing the text and portrait when closing
            {
                xScale = 1;
                yScale = MathHelper.Lerp(1, 0, popupState.CloseTimer / popupState.CLOSE_TIME);
            }

            float initialXOffset = 110;
            float initialYOffset = 30;
            float yOffset = 4;

            // there is the potential for the y offset of the popup to mess with the closing animation, just a note for the future when it happens
            spriteBatch.Draw(backDrop, new Vector2(textPosition.X + initialXOffset, textPosition.Y + initialYOffset + yOffset), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            spriteBatch.Draw(popupState.GetPopupFace(), new Vector2(textPosition.X + initialXOffset + xOffset, textPosition.Y + initialYOffset), null, Color.White, 0f, backDrop.Size() / 2, new Vector2(xScale, yScale), SpriteEffects.None, 1f);
            
            spriteBatch.Reload(SpriteSortMode.Deferred);
        }

        public void DrawText(SpriteBatch spriteBatch, PopupState popupState, Vector2 textPosition)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(popupState.GetPopupText(), Color.White).ToArray();
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, snippets, textPosition, Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out _, 300);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            int offset = 0;
            foreach (var popupState in player.PopupStates.Values)
            {
                Vector2 textPosition = new Vector2(xPosition - 95, yPosition - 25 - (OFFSET_DISTANCE * offset));

                //Main.NewText(popupState.GetPopupText());
                DrawPopup(spriteBatch, popupState, textPosition);
                if (!popupState.CanClose) DrawText(spriteBatch, popupState, textPosition);

                offset++;
            }

            /*if (PopupList.Count < 3)
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

            PopupList = new List<Popup>(PopupRemoval);*/
        }
    }
}