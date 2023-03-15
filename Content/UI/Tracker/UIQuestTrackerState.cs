using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.GameContent.UI.Elements;
using OvermorrowMod.Common.Configs;
using OvermorrowMod.Common;
using OvermorrowMod.Content.UI.AmmoSwap;
using Terraria.GameContent;
using Terraria.UI.Chat;
using System;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.State;
using OvermorrowMod.Quests.Requirements;
using System.Linq;
using OvermorrowMod.Core.Interfaces;

namespace OvermorrowMod.Content.UI.Tracker
{
    public class UIQuestTrackerState : UIState
    {
        private DragableUIPanel testPanel = new DragableUIPanel();
        private QuestTrackerPanel back = new QuestTrackerPanel();

        public List<QuestEntry> questEntries = new List<QuestEntry>();
        public override void OnInitialize()
        {
            //ModUtils.AddElement(testPanel, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 240, 120, this);
            ModUtils.AddElement(back, (int)(Main.screenWidth / 2f), (int)(Main.screenHeight / 2f), 272, 120, this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        bool canDo = true;
        public override void Update(GameTime gameTime)
        {
            questEntries.Clear();

            // get player's current active quests here and then create entries for them
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            foreach (BaseQuestState questState in questPlayer.CurrentQuests)
            {
                BaseQuest quest = questState.Quest;

                List<QuestObjective> objectives = new List<QuestObjective>();
                foreach (var task in quest.Requirements)
                {
                    if (task is OrRequirement orRequirement)
                    {
                        int quantity = 0;
                        int stack = 0;
                        string description = "";

                        int clauseCounter = 0;
                        foreach (var orRequirementClause in orRequirement.AllClauses)
                        {
                            if (orRequirementClause is ItemRequirement orItemRequirement)
                            {
                                if (orItemRequirement.GetCurrentStack(questPlayer) > quantity)
                                    quantity = orItemRequirement.GetCurrentStack(questPlayer);

                                stack = orItemRequirement.stack;
                                description += orItemRequirement.description;
                            }

                            if (orRequirementClause is KillRequirement orKillRequirement)
                            {
                                if (orKillRequirement.GetCurrentKilled(questPlayer, questState) > quantity)
                                    quantity = orKillRequirement.GetCurrentKilled(questPlayer, questState);

                                stack = orKillRequirement.amount;
                                description += orKillRequirement.description;
                            }

                            if (clauseCounter < orRequirement.GetClauseLength() - 1)
                                description += " or ";

                            clauseCounter++;
                        }

                        objectives.Add(new QuestObjective(quantity, stack, description));
                    }

                    if (task is ChainRequirement chainRequirement)
                    {
                        // TODO: Convert to a method that gets completed clauses and appends it
                        foreach (var completedClause in chainRequirement.AllClauses)
                        {
                            if (!completedClause.IsCompleted(questPlayer, questState)) continue;

                            /*int quantity = 0;
                            int stack = 0;
                            string description = "";

                            if (completedClause is ItemRequirement chainItemRequirement)
                            {
                                quantity = chainItemRequirement.GetCurrentStack(questPlayer);
                                stack = chainItemRequirement.stack;
                                description = chainItemRequirement.description;
                            }

                            if (completedClause is KillRequirement chainKillRequirement)
                            {
                                quantity = chainKillRequirement.GetCurrentKilled(questPlayer, questState);
                                stack = chainKillRequirement.amount;
                                description = chainKillRequirement.description;
                            }

                            if (completedClause is TravelRequirement travelRequirement)
                            {
                                quantity = 1;
                                stack = 1;
                                description = travelRequirement.description;
                            }

                            objectives.Add(new QuestObjective(quantity, stack, description));*/
                            objectives.Add(AddQuestObjective(completedClause, questPlayer, questState));
                        }

                        // TODO: Convert to a method that gets active clause and appends it at the end
                        foreach (var chainRequirementClause in chainRequirement.GetActiveClauses(questPlayer, questState))
                        {
                            /*int quantity = 0;
                            int stack = 0;
                            string description = "";

                            AddQuestObjective(chainRequirementClause, questPlayer, questState);

                            if (chainRequirementClause is ItemRequirement chainItemRequirement)
                            {
                                quantity = chainItemRequirement.GetCurrentStack(questPlayer);
                                stack = chainItemRequirement.stack;
                                description = chainItemRequirement.description;
                            }

                            if (chainRequirementClause is KillRequirement chainKillRequirement)
                            {
                                quantity = chainKillRequirement.GetCurrentKilled(questPlayer, questState);
                                stack = chainKillRequirement.amount;
                                description = chainKillRequirement.description;
                            }

                            if (chainRequirementClause is TravelRequirement travelRequirement)
                            {
                                quantity = travelRequirement.IsCompleted(questPlayer, questState) ? 1 : 0;
                                stack = 1;
                                description = travelRequirement.description;
                            }*/

                            objectives.Add(AddQuestObjective(chainRequirementClause, questPlayer, questState));

                            //objectives.Add(new QuestObjective(quantity, stack, description));
                        }
                    }

                    if (task is ItemRequirement itemRequirement)
                    {
                        objectives.Add(new QuestObjective(itemRequirement.GetCurrentStack(questPlayer), itemRequirement.stack, itemRequirement.description));
                    }
                    else if (task is KillRequirement killRequirement)
                    {
                        objectives.Add(new QuestObjective(killRequirement.GetCurrentKilled(questPlayer, questState), killRequirement.amount, killRequirement.description));
                    }
                }

                questEntries.Add(new QuestEntry(quest.QuestName, objectives));
            }

            //questEntries.Add(new QuestEntry("Stryke's Stryfe", new List<QuestObjective>() { new QuestObjective(0, 1, "Defeat Stryfe of Phantasia") }));
            //questEntries.Add(new QuestEntry("I am the Grass Man", new List<QuestObjective>() { new QuestObjective(2, 4, "Destroy 4 Lawnmowers and Race Cars"), new QuestObjective(0, 1, "Water the Lawn Before it Dries Out and Everyone Gets Sad From Dry Grass") }));
            //questEntries.Add(new QuestEntry("Town of Sojourn", new List<QuestObjective>() { new QuestObjective(0, 1, "Escort Feyden to Sojourn"), new QuestObjective(0, 1, "Speak to the Guards within the Town Garrison") }));

            base.Update(gameTime);
        }

        private QuestObjective AddQuestObjective(IQuestRequirement requirementClause, QuestPlayer questPlayer, BaseQuestState questState)
        {
            int progress = 0;
            int amount = 0;
            string description = "";

            if (requirementClause is ItemRequirement itemRequirement)
            {
                Main.NewText("item requirement");
                progress = itemRequirement.GetCurrentStack(questPlayer);
                amount = itemRequirement.stack;
                description = itemRequirement.description;
            }
            else if (requirementClause is KillRequirement killRequirement)
            {
                Main.NewText("kill requirement");
                progress = killRequirement.GetCurrentKilled(questPlayer, questState);
                amount = killRequirement.amount;
                description = killRequirement.description;
            }
            else if (requirementClause is TravelRequirement travelRequirement)
            {
                Main.NewText("travel requirement");
                progress = travelRequirement.IsCompleted(questPlayer, questState) ? 1 : 0;
                amount = 1;
                description = travelRequirement.description;
            }


            return new QuestObjective(progress, amount, description);
        }
    }

    public class QuestTrackerPanel : DragableUIPanel
    {
        // Code by Seraph
        public void DrawNineSegmentTexturePanel(SpriteBatch spriteBatch, Texture2D texture, Rectangle dimensions, int cornerWidth, Color drawColor)
        {
            Rectangle cornerFrameTopLeft = new Rectangle(0, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomLeft = new Rectangle(0, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerFrameTopRight = new Rectangle(texture.Width - cornerWidth, 0, cornerWidth, cornerWidth);
            Rectangle cornerFrameBottomRight = new Rectangle(texture.Width - cornerWidth, texture.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideFrameTop = new Rectangle(cornerWidth, 0, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameBottom = new Rectangle(cornerWidth, texture.Height - cornerWidth, texture.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideFrameLeft = new Rectangle(0, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle sideFrameRight = new Rectangle(texture.Width - cornerWidth, cornerWidth, cornerWidth, texture.Height - (cornerWidth * 2));
            Rectangle centreFrame = new Rectangle(cornerWidth, cornerWidth, texture.Width - (cornerWidth * 2), texture.Height - (cornerWidth * 2));

            Rectangle cornerRectTopLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle cornerRectTopRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y, cornerWidth, cornerWidth);
            Rectangle cornerRectBottomRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, cornerWidth, cornerWidth);
            Rectangle sideRectTop = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectBottom = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + (int)dimensions.Height - cornerWidth, (int)dimensions.Width - (cornerWidth * 2), cornerWidth);
            Rectangle sideRectLeft = new Rectangle((int)dimensions.X, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle sideRectRight = new Rectangle((int)dimensions.X + (int)dimensions.Width - cornerWidth, (int)dimensions.Y + cornerWidth, cornerWidth, (int)dimensions.Height - (cornerWidth * 2));
            Rectangle centreRect = new Rectangle((int)dimensions.X + cornerWidth, (int)dimensions.Y + cornerWidth, (int)dimensions.Width - (cornerWidth * 2), (int)dimensions.Height - (cornerWidth * 2));

            spriteBatch.Draw(texture, cornerRectTopLeft, cornerFrameTopLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectTopRight, cornerFrameTopRight, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomLeft, cornerFrameBottomLeft, drawColor);
            spriteBatch.Draw(texture, cornerRectBottomRight, cornerFrameBottomRight, drawColor);
            spriteBatch.Draw(texture, sideRectTop, sideFrameTop, drawColor);
            spriteBatch.Draw(texture, sideRectBottom, sideFrameBottom, drawColor);
            spriteBatch.Draw(texture, sideRectLeft, sideFrameLeft, drawColor);
            spriteBatch.Draw(texture, sideRectRight, sideFrameRight, drawColor);
            spriteBatch.Draw(texture, centreRect, centreFrame, drawColor);
        }

        private int drawHeight = 160;
        public override void Update(GameTime gameTime)
        {
            // update height based on the parent's list of quests
            base.Update(gameTime);

            // Compute the height needed for the container here
            if (Parent is UIQuestTrackerState state)
            {
                Height.Pixels = drawHeight;
                //if (state.questEntries.Count == 0) drawHeight = 160;
                //else drawHeight = 160;
                //Main.NewText(state.questEntries[0].Name);
            }
        }

        /// <summary>
        /// Uses ChatManager to get the string size based on the max width,
        /// ChatManager returns a Vector2 where y is the height of the string that has been split.
        /// The y value is then divided by 28 and then rounded up in order to get the number of lines
        /// </summary>
        private int GetNumberLines(string text, int maxWidth)
        {
            const int lineHeight = 28;
            float lineCount = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One * 0.9f, MAXIMUM_LENGTH).Y / lineHeight;

            return (int)Math.Round(lineCount);
        }

        public readonly float MAXIMUM_LENGTH = 250;

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "DialoguePanel").Value;
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.25f, 0.25f), 0, 0);
            //spriteBatch.Draw(texture, new Vector2(GetDimensions().X, GetDimensions().Y), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.7f, 0.25f), 0, 0);
            DrawNineSegmentTexturePanel(spriteBatch, texture, new Rectangle((int)(GetDimensions().X), (int)(GetDimensions().Center().Y - (Height.Pixels / 2)), 272, drawHeight), 35, Color.White * 0.6f);

            if (Parent is UIQuestTrackerState state)
            {
                int LEFT_PADDING = 15;
                int INITIAL_OBJECTIVE_OFFSET = 30; // Offset by the height of the text so that it draws below the name

                if (state.questEntries.Count == 0)
                {
                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, "No Quests Available!", new Vector2(GetDimensions().X + LEFT_PADDING + 50, GetDimensions().Y + 30), Color.Yellow, 0f, Vector2.Zero, Vector2.One * 0.9f, MAXIMUM_LENGTH);
                    drawHeight = 80;
                    return;
                }


                int entryCount = 0;
                int entryOffset = 0;
                foreach (QuestEntry entry in state.questEntries)
                {
                    int offset = 30 + entryCount * 10;

                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, entry.Name, new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + offset + entryOffset), Color.Yellow, 0f, Vector2.Zero, Vector2.One * 0.9f, MAXIMUM_LENGTH);

                    int objectiveCount = 0;
                    int objectivePadding = 0;
                    foreach (QuestObjective objective in entry.Objectives)
                    {
                        objectiveCount++;

                        string objectiveText = "- " + objective.Progress + "/" + objective.Quantity + " " + objective.Description;
                        Color textColor = objective.Progress >= objective.Quantity ? new Color(127, 255, 212) : Color.White;
                        TextSnippet[] objectiveSnippets = ChatManager.ParseMessage(objectiveText, textColor).ToArray();
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + INITIAL_OBJECTIVE_OFFSET + offset + entryOffset + objectivePadding), Color.Yellow, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAXIMUM_LENGTH);

                        entryOffset += objectiveSnippets.Length * 15 + (GetNumberLines(objectiveText, (int)MAXIMUM_LENGTH) * 20);
                        //objectivePadding += 5;
                    }

                    entryOffset += 20; // Always increase by 20 because of the entry's name height is ~20
                    entryCount++;
                }

                int totalOffset = INITIAL_OBJECTIVE_OFFSET + state.questEntries.Count * 20;
                int calculatedHeight = entryOffset + totalOffset;
                drawHeight = calculatedHeight;
            }
        }
    }

    public class QuestEntry
    {
        public string Name;
        public List<QuestObjective> Objectives;

        public QuestEntry(string name, List<QuestObjective> objectives)
        {
            Name = name;
            Objectives = objectives;
        }
    }

    public class QuestObjective
    {
        public int Progress;
        public int Quantity;
        public string Description;

        public QuestObjective(int progress, int quantity, string description)
        {
            Progress = progress;
            Quantity = quantity;
            Description = description;
        }
    }
}