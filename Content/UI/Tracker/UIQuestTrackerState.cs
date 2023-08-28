using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using OvermorrowMod.Core;
using System.Collections.Generic;
using Terraria.GameContent;
using Terraria.UI.Chat;
using System;
using OvermorrowMod.Quests;
using OvermorrowMod.Quests.State;
using OvermorrowMod.Quests.Requirements;
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
            ModUtils.AddElement(back, (int)(Main.screenWidth / 2f - 130), (int)(/*Main.screenHeight / 2f*/ 32), 272, 120, this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

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
                        objectives.Add(AddOrQuestObjective(orRequirement, questPlayer, questState));
                    }

                    if (task is ChainRequirement chainRequirement)
                    {
                        foreach (var completedClause in chainRequirement.AllClauses)
                        {
                            if (!completedClause.IsCompleted(questPlayer, questState)) continue;
                            objectives.Add(AddQuestObjective(completedClause, questPlayer, questState));
                        }

                        foreach (var chainRequirementClause in chainRequirement.GetActiveClauses(questPlayer, questState))
                        {
                            objectives.Add(AddQuestObjective(chainRequirementClause, questPlayer, questState));
                        }
                    }
                    else
                    {
                        objectives.Add(AddQuestObjective(task, questPlayer, questState));
                    }

                }

                bool canTurnIn = quest.TryUpdateQuestRequirements(questPlayer, questState);
                questEntries.Add(new QuestEntry(quest.QuestName, objectives, canTurnIn));
            }

            //questEntries.Add(new QuestEntry("Stryke's Stryfe", new List<QuestObjective>() { new QuestObjective(0, 1, "Defeat Stryfe of Phantasia") }));
            //questEntries.Add(new QuestEntry("I am the Grass Man", new List<QuestObjective>() { new QuestObjective(2, 4, "Destroy 4 Lawnmowers and Race Cars"), new QuestObjective(0, 1, "Water the Lawn Before it Dries Out and Everyone Gets Sad From Dry Grass") }));
            //questEntries.Add(new QuestEntry("Town of Sojourn", new List<QuestObjective>() { new QuestObjective(0, 1, "Escort Feyden to Sojourn"), new QuestObjective(0, 1, "Speak to the Guards within the Town Garrison") }));

            base.Update(gameTime);
        }

        private QuestObjective AddOrQuestObjective(OrRequirement orRequirement, QuestPlayer questPlayer, BaseQuestState questState)
        {
            int progress = 0;
            int amount = 0;
            string description = "";

            int clauseCounter = 0;
            foreach (var orRequirementClause in orRequirement.AllClauses)
            {
                if (orRequirementClause is ItemRequirement orItemRequirement)
                {
                    if (orItemRequirement.GetCurrentStack(questPlayer) > progress)
                        progress = orItemRequirement.GetCurrentStack(questPlayer);

                    amount = orItemRequirement.stack;
                    description += orItemRequirement.description;
                }

                if (orRequirementClause is KillRequirement orKillRequirement)
                {
                    if (orKillRequirement.GetCurrentKilled(questPlayer, questState) > progress)
                        progress = orKillRequirement.GetCurrentKilled(questPlayer, questState);

                    amount = orKillRequirement.amount;
                    description += orKillRequirement.description;
                }

                if (clauseCounter < orRequirement.GetClauseLength() - 1)
                    description += " or ";

                clauseCounter++;
            }

            return new QuestObjective(progress, amount, description);
        }

        private QuestObjective AddQuestObjective(IQuestRequirement requirementClause, QuestPlayer questPlayer, BaseQuestState questState)
        {
            int progress = 0;
            int amount = 0;
            string description = "";

            if (requirementClause is ItemRequirement itemRequirement)
            {
                progress = itemRequirement.GetCurrentStack(questPlayer);
                amount = itemRequirement.stack;
                description = itemRequirement.description;
            }
            else if (requirementClause is KillRequirement killRequirement)
            {
                progress = killRequirement.GetCurrentKilled(questPlayer, questState);
                amount = killRequirement.amount;
                description = killRequirement.description;
            }
            else if (requirementClause is TravelRequirement travelRequirement)
            {
                progress = travelRequirement.IsCompleted(questPlayer, questState) ? 1 : 0;
                amount = 1;
                description = travelRequirement.description;
            }
            else if (requirementClause is MiscRequirement miscRequirement)
            {
                progress = miscRequirement.IsCompleted(questPlayer, questState) ? 1 : 0;
                amount = 1;
                description = miscRequirement.description;
            }

            return new QuestObjective(progress, amount, description, requirementClause.IsCompleted(questPlayer, questState));
        }
    }

    public class QuestTrackerPanel : DragableUIPanel
    {
        
        private int drawHeight = 160;
        public override void Update(GameTime gameTime)
        {
            // update height based on the parent's list of quests
            base.Update(gameTime);

            // Compute the height needed for the container here
            if (Parent is UIQuestTrackerState state)
            {
                Height.Pixels = drawHeight;
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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "TrackerPanel").Value;
            //spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.25f, 0.25f), 0, 0);
            //spriteBatch.Draw(texture, new Vector2(GetDimensions().X, GetDimensions().Y), null, Color.White, 0, texture.Size() / 2f, new Vector2(0.7f, 0.25f), 0, 0);
            ModUtils.DrawNineSegmentTexturePanel(spriteBatch, texture, new Rectangle((int)(GetDimensions().X), (int)(GetDimensions().Center().Y - (Height.Pixels / 2)), 272, drawHeight), 35, Color.White * 0.6f);

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
                    Color titleColor = entry.CanTurnIn ? new Color(127, 255, 212) : Color.Yellow;

                    ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, entry.Name, new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + offset + entryOffset), titleColor, 0f, Vector2.Zero, Vector2.One * 0.9f, MAXIMUM_LENGTH);

                    int objectiveCount = 0;
                    int objectivePadding = 0;
                    if (entry.CanTurnIn)
                    {
                        objectiveCount++;

                        string objectiveText = "- Can be turned in!";

                        Color textColor = new Color(127, 255, 212);
                        TextSnippet[] objectiveSnippets = ChatManager.ParseMessage(objectiveText, textColor).ToArray();
                        ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + INITIAL_OBJECTIVE_OFFSET + offset + entryOffset + objectivePadding), Color.Yellow, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAXIMUM_LENGTH);

                        entryOffset += objectiveSnippets.Length * 15 + (GetNumberLines(objectiveText, (int)MAXIMUM_LENGTH) * 20);
                    }
                    else
                    {
                        foreach (QuestObjective objective in entry.Objectives)
                        {
                            objectiveCount++;

                            string objectiveText = "- " + objective.Progress + "/" + objective.Quantity + " " + objective.Description;
                            if (objective.IsCompleted) objectiveText = "    " + objective.Description;

                            Color textColor = objective.IsCompleted ? new Color(127, 255, 212) : Color.White;
                            TextSnippet[] objectiveSnippets = ChatManager.ParseMessage(objectiveText, textColor).ToArray();
                            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, objectiveSnippets, new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + INITIAL_OBJECTIVE_OFFSET + offset + entryOffset + objectivePadding), Color.Yellow, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAXIMUM_LENGTH);

                            if (objective.IsCompleted)
                            {
                                Vector2 checkBoxOffset = new Vector2(8, 8);
                                Texture2D checkTexture = ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBox_Checked").Value;
                                spriteBatch.Draw(checkTexture, checkBoxOffset + new Vector2(GetDimensions().X + LEFT_PADDING, GetDimensions().Y + INITIAL_OBJECTIVE_OFFSET + offset + entryOffset + objectivePadding), null, Color.White, 0f, checkTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
                            }

                            entryOffset += objectiveSnippets.Length * 15 + (GetNumberLines(objectiveText, (int)MAXIMUM_LENGTH) * 20);
                            //objectivePadding += 5;
                        }
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
        public bool CanTurnIn;

        public QuestEntry(string name, List<QuestObjective> objectives, bool canTurnIn)
        {
            Name = name;
            Objectives = objectives;
            CanTurnIn = canTurnIn;
        }
    }

    public class QuestObjective
    {
        public int Progress;
        public int Quantity;
        public string Description;
        public bool IsCompleted;

        public QuestObjective(int progress, int quantity, string description, bool isCompleted = false)
        {
            Progress = progress;
            Quantity = quantity;
            Description = description;
            IsCompleted = isCompleted;
        }
    }
}