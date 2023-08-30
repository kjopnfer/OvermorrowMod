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
        }
    }

    public partial class DialogueState : UIState
    {
        /// <summary>
        /// Initializes the UI with the Quest dialogue if the player is doing a quest assigned by the NPC they are talking to.
        /// </summary>
        private void SetQuestDialogue()
        {
            var npc = Main.npc[Main.LocalPlayer.talkNPC];
            var quest = npc.GetGlobalNPC<QuestNPC>().GetCurrentQuest(npc, out var isDoing);

            if (!isDoing) return;

            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            var questState = Quests.Quests.State.GetActiveQuestState(questPlayer, quest);

            // Check if the player is currently doing the quest or is ready to turn it in
            // If ready, set the traverser to the quest complete chain
            if (quest.CanHandInQuest(questPlayer, questState)) SetID("quest_complete"); 
            else SetID("quest_hint");
        }

        /// <summary>
        /// Change the node that the XML traverser is currently looking at
        /// </summary>
        /// <param name="id"></param>
        public void SetID(string id)
        {
            Text.SetText("");

            shouldRedraw = true;
            dialogueID = id;

            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();
            Dialogue dialogue = player.GetDialogue();

            if (dialogue != null) dialogue.UpdateList(id);
        }

        /// <summary>
        /// Prevents the player from moving and makes the player immune to all damage
        /// </summary>
        private void LockPlayer()
        {
            Player player = Main.LocalPlayer;

            player.mouseInterface = true;
            player.immune = true;
            player.immuneTime = 60;
            player.immuneNoBlink = true;

            player.GetModPlayer<DialoguePlayer>().LockPlayer = true;
        }

        public void ResetTimers()
        {
            DrawTimer = 0;
            DelayTimer = 0;
        }

        /// <summary>
        /// Gets the next Text node by increment the reader index
        /// </summary>
        private void AdvanceText()
        {
            DialoguePlayer player = Main.LocalPlayer.GetModPlayer<DialoguePlayer>();

            player.GetDialogue().IncrementText();

            ResetTimers();
            shouldRedraw = true;
        }

        /// <summary>
        /// Exits the conversation with the NPC and resets UI counters
        /// </summary>
        public void ExitText()
        {
            ResetTimers();
            SetID("start");

            hasInitialized = false;
            Main.LocalPlayer.SetTalkNPC(-1);
        }

        /// <summary>
        /// Draw the main panel where the NPC dialogue is being drawn
        /// </summary>
        /// <param name="player"></param>
        /// <param name="spriteBatch"></param>
        private void DrawBackdrop(DialoguePlayer player, SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            //spriteBatch.Draw(texture, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f), null, Color.White * 0.75f, 0, texture.Size() / 2f, new Vector2(1.25f, 1), 0, 0);

            //Color color = Color.White * 0.9f;
            texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "TrackerPanel").Value;
            float height_padding = 30;
            int width = 600;
            int height = 180;
            //Vector2 position = new Vector2(GetDimensions().X, GetDimensions().Center().Y - (height / 2));
            Vector2 position = new Vector2(Main.screenWidth / 2f - width / 2f, Main.screenHeight / 3f - height / 2f);
            Rectangle drawRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);

            //ModUtils.DrawNineSegmentTexturePanel(spriteBatch, texture, drawRectangle, 35, Color.White * 0.6f);
            Color color = new Color(28, 31, 77);
            Utils.DrawInvBG(spriteBatch, drawRectangle, color * 0.925f);

            /*#region Left Side
            Texture2D topLeftBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_Border_TopLeft").Value;
            Vector2 topLeft = position - new Vector2(4, 4);
            spriteBatch.Draw(topLeftBorder, topLeft + topLeftBorder.Size() / 2f, null, color, 0, topLeftBorder.Size() / 2f, 1f, 0, 0);

            Texture2D bottomLeftBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_Border_BottomLeft").Value;
            Vector2 bottomLeft = position + new Vector2(bottomLeftBorder.Width - 4, height - 6);
            spriteBatch.Draw(bottomLeftBorder, bottomLeft - bottomLeftBorder.Size() / 2f, null, color, 0, bottomLeftBorder.Size() / 2f, 1f, 0, 0);

            Rectangle leftBorderRectangle = new Rectangle((int)topLeft.X + 4, (int)topLeft.Y + topLeftBorder.Height, 4, height - (52 * 2) - 4);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, leftBorderRectangle, TextureAssets.MagicPixel.Value.Frame(), color);
            #endregion

            #region Right Side
            Texture2D topRightBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_Border_TopRight").Value;
            Vector2 topRight = position + new Vector2(width - topRightBorder.Width, -4);
            spriteBatch.Draw(topRightBorder, topRight + topRightBorder.Size() / 2f, null, color, 0, topLeftBorder.Size() / 2f, 1f, 0, 0);
            
            Texture2D bottomRightBorder = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue_Border_BottomRight").Value;
            Vector2 bottomRight = position + new Vector2(width, height - 6);
            spriteBatch.Draw(bottomRightBorder, bottomRight - bottomRightBorder.Size() / 2f, null, color, 0, bottomRightBorder.Size() / 2f, 1f, 0, 0);

            Rectangle rightBorderRectangle = new Rectangle((int)topRight.X + 44, (int)topRight.Y + topLeftBorder.Height - 20, 4, height - (52 * 2) + 18);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rightBorderRectangle, TextureAssets.MagicPixel.Value.Frame(), color);
            #endregion*/

            #region Face
            //Texture2D speaker = player.GetDialogue().speakerBody;
            Texture2D speaker = ModContent.Request<Texture2D>(AssetDirectory.UI + "Dialogue/Orvyn").Value;
            Vector2 offset = new Vector2(-200, -20);
            spriteBatch.Draw(speaker, new Vector2(Main.screenWidth / 2f, Main.screenHeight / 3f) + offset, null, Color.White, 0, speaker.Size() / 2f, 1f, 0, 0);
            #endregion
        }
    }
}