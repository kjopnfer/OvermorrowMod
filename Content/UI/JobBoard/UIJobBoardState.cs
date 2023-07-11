using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Core;
using Terraria.Audio;
using OvermorrowMod.Content.Tiles.Town;
using OvermorrowMod.Quests;
using Terraria.UI.Chat;
using Terraria.GameContent;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Quests.Rewards;
using OvermorrowMod.Quests.Requirements;
using System;

namespace OvermorrowMod.Content.UI.JobBoard
{
    public class JobEntry
    {
        public int jobRank;
        public string jobName;

        public JobEntry(int rank, string name)
        {
            jobRank = rank;
            jobName = name;
        }
    }

    public class UIJobBoardState : UIState
    {
        // has 3 states:
        // 1. opening animation
        // 2. displaying jobs
        // 3. closing animation

        // there will be unique job boards tied to various towns
        // therefore should load and save jobs tied to the job board
        private UIJobBoardPanel drawSpace = new UIJobBoardPanel();
        internal UIJobBoardCloseButton closeButton = new UIJobBoardCloseButton();
        private UIText timerText = new UIText("Time");

        public bool showBoard = false;
        public JobBoard_TE boardTileEntity { get; private set; }

        // TODO: make a job entry ui object that gets created when running through hashset
        public void OpenJobBoard(JobBoard_TE entity)
        {
            showBoard = true;
            boardTileEntity = entity;

            this.RemoveAllChildren();
            ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);
            drawSpace.RemoveAllChildren();
            ModUtils.AddElement(closeButton, 700, 0, 22, 22, drawSpace);

            DisplayBoard();
        }

        public void ResetJobBoard()
        {
            this.RemoveAllChildren();
            ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);
            drawSpace.RemoveAllChildren();
            ModUtils.AddElement(closeButton, 700, 0, 22, 22, drawSpace);

            DisplayBoard();
        }

        //double boardTimer = 0;
        public override void Update(GameTime gameTime)
        {
            if (!showBoard) return;

            const int totalTime = 86400;
            Main.LocalPlayer.mouseInterface = true;

            drawSpace.RemoveChild(timerText);

            double boardTimer = boardTileEntity.boardElapsedTime;
            string hours = Math.Floor((totalTime - boardTimer) / 3600).ToString();
            string minutes = Math.Floor((totalTime - boardTimer) % 3600 / 60).ToString();

            if (int.Parse(minutes) < 10) minutes = "0" + minutes;

            timerText = new UIText("Resets in: " + hours.ToString() + ":" + minutes + " minutes", 1.25f);
            ModUtils.AddElement(timerText, 0, 450, 100, 100, drawSpace);
            //this.RemoveAllChildren();
            //ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);
            //drawSpace.RemoveAllChildren();
            //ModUtils.AddElement(closeButton, 700, 0, 22, 22, drawSpace);

        }

        /// <summary>
        /// Handles the opening animation of the board and loading all the jobs
        /// </summary>
        private void OnBoardOpen() { }

        /// <summary>
        /// Handles drawing of the board UI
        /// </summary>
        private void DisplayBoard()
        {
            if (boardTileEntity.JobQuests.Count > 0)
            {
                int entryCount = 0;
                foreach (BaseQuest quest in boardTileEntity.JobQuests)
                {
                    Vector2 position = GetBoardEntryPosition(entryCount);
                    ModUtils.AddElement(new UIJobBoardEntry(quest), (int)position.X, (int)position.Y, 200, 200, drawSpace);
                    entryCount++;
                }

                Main.NewText(entryCount);
            }
            else
            {
                Main.NewText("no");
            }
        }

        private Vector2 GetBoardEntryPosition(int entryCount)
        {
            float xOffset = 220 * entryCount;
            if (entryCount > 2) xOffset = 220 * (entryCount - 3);

            float yOffset = (entryCount > 2 ? 220 : 0);

            return new Vector2(xOffset, yOffset);
        }


        /// <summary>
        /// Handles the closing animation of the board
        /// </summary>
        private void OnBoardClose() { }
    }

    public class UIJobBoardPanel : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardBack").Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1f, 0, 0);
        }
    }

    public class UIJobBoardEntry : UIPanel
    {
        public BaseQuest quest { get; private set; }
        public UIJobBoardEntry(BaseQuest quest)
        {
            this.quest = quest;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = new Vector2(GetDimensions().X, GetDimensions().Y);
            float textScale = 1f;
            float titleScale = 1.5f;
            float maxWidth = 200;

            Texture2D temp = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(temp, new Rectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels), Color.White);

            float titleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, quest.QuestName, Vector2.One * titleScale, maxWidth).X;
            float titleOffset = ChatManager.GetStringSize(FontAssets.MouseText.Value, quest.QuestName, Vector2.One * titleScale, maxWidth).Y + 20;
            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, quest.QuestName, position + new Vector2((Width.Pixels - titleLength) / 2f, 0), Color.Black, 0f, Vector2.Zero, Vector2.One * titleScale, maxWidth);

            foreach (IQuestRequirement requirement in quest.Requirements)
            {
                if (requirement is ItemRequirement itemRequirement)
                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, itemRequirement.description, position + new Vector2(0, titleOffset), Color.Black, 0f, Vector2.Zero, Vector2.One * textScale, maxWidth);
            }

            this.RemoveAllChildren();
            int rewardCount = 0;
            foreach (IQuestReward ireward in quest.Rewards)
            {
                if (ireward is ItemReward reward)
                {
                    int xOffset = 50 * rewardCount;
                    ModUtils.AddElement(new DisplayItemSlot(reward.type, reward.stack), xOffset, 100, 42, 42, this);
                    rewardCount++;
                }
                //ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, requirement, position + new Vector2(0, titleOffset), Color.Black, 0f, Vector2.Zero, Vector2.One * textScale, maxWidth);
            }

            ModUtils.AddElement(new UIJobBoardAcceptButton(), 50, 150, 64, 32, this);

            //ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, quest.Requirements[0]., position + new Vector2(0, titleOffset), Color.Black, 0f, Vector2.Zero, Vector2.One * scale, maxWidth);

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
        }
    }

    public class UIJobBoardCloseButton : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (Parent.Parent is UIJobBoardState parent)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BookClose").Value;
                Color color = isHovering ? Color.White * 0.5f : Color.White;
                spriteBatch.Draw(texture, pos + new Vector2(texture.Width / 2f, texture.Height / 2f), null, color, 0f, texture.Size() / 2f, 1f, 0, 0);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            if (Parent.Parent is UIJobBoardState parent)
            {
                parent.RemoveAllChildren();
                parent.showBoard = false;
            }
        }
    }

    public class UIJobBoardAcceptButton : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Parent is UIJobBoardEntry boardEntry)
            {
                QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                var quest = boardEntry.quest;
                var questState = Quests.Quests.State.GetActiveQuestState(questPlayer, quest);

                Vector2 pos = GetDimensions().ToRectangle().TopLeft();
                bool isHovering = ContainsPoint(Main.MouseScreen);

                Texture2D texture = isHovering ? ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardAccept_Hover").Value : ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardAccept").Value;
                if (questState != null && !questState.Completed)
                {
                    //if (quest.CanHandInQuest(questPlayer, questState)
                    //Main.NewText(Quests.Quests.State.CheckDoingQuest(questPlayer, quest.QuestID) && quest.CanHandInQuest(questPlayer, questState));
                    if (quest.TryUpdateQuestRequirements(questPlayer, questState))
                        texture = isHovering ? ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardTurnIn_Hover").Value : ModContent.Request<Texture2D>(AssetDirectory.UI + "BoardTurnIn").Value;
                }
                //Main.NewText(questPlayer.IsDoingQuest(quest.QuestID));

                Color color = isHovering ? Color.White * 0.5f : Color.White;
                spriteBatch.Draw(texture, pos + new Vector2(texture.Width / 2f, texture.Height / 2f), null, color, 0f, texture.Size() / 2f, 1f, 0, 0);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            if (Parent is UIJobBoardEntry boardEntry)
            {
                QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
                var quest = boardEntry.quest;
                var questState = Quests.Quests.State.GetActiveQuestState(questPlayer, quest);

                if (questState != null/*Quests.Quests.State.CheckDoingQuest(questPlayer, quest.QuestID)*/ && !questState.Completed)
                {
                    if (quest.TryUpdateQuestRequirements(questPlayer, questState))
                    {
                        questPlayer.CompleteQuest(quest.QuestID);
                        Main.NewText("COMPLETED QUEST: " + quest.QuestName, Color.Yellow);
                        //questState.Completed = false;
                    }
                }
                else
                {
                    if (questState != null)
                    {
                        if (questState.Completed) questState.Completed = false;
                    }

                    questPlayer.AddQuest(quest);
                    Main.NewText("ACCEPTED QUEST: " + quest.QuestName, Color.Yellow);

                    // When a quest is accepted, add it into the board with the following info:
                    if (Parent.Parent.Parent is UIJobBoardState boardState)
                    {
                        var acceptedQuests = boardState.boardTileEntity.AcceptedQuests;
                        if (!acceptedQuests.ContainsKey(questPlayer.PlayerUUID))
                        {
                            QuestTakerInfo info = new QuestTakerInfo(questPlayer.PlayerUUID, questPlayer.Name);
                            info.Quests.Add(quest);

                            acceptedQuests.Add(questPlayer.PlayerUUID, info);
                        }
                        else
                        {
                            acceptedQuests[questPlayer.PlayerUUID].Quests.Add(quest);
                        }
                    }
                    // PlayerUUID, player name, and the accepted quest object
                }
            }

            base.MouseDown(evt);
        }
    }
}