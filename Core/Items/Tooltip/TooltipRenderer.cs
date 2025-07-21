using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Tooltips;
using OvermorrowMod.Core.NPCs;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.Items
{
    /// <summary>
    /// Handles all tooltip rendering logic
    /// </summary>
    public static class TooltipRenderer
    {
        private static readonly Dictionary<string, string> TextHighlights = new Dictionary<string, string>
        {
            { "Type", "FAD5A5" },
            { "Increase", "58D68D" },
            { "Decrease", "ff5555" },
            { "Keyword", "ff79c6" },
        };

        private static readonly Dictionary<string, string> ObjectHighlights = new Dictionary<string, string>
        {
            { "Key", "808080" },
            { "Buff", "50fa7b" },
            { "Debuff", "ff5555" },
            { "Projectile", "8be9fd" },
        };

        #region Main Drawing Methods

        /// <summary>
        /// Draws all tooltip entities with proper positioning and overflow handling
        /// </summary>
        public static void DrawTooltipEntities(SpriteBatch spriteBatch, List<TooltipEntity> tooltips, string widestLine, int x, int y)
        {
            float yOffset = 0;
            foreach (var tooltip in tooltips)
            {
                Vector2 position = new Vector2(x, y + yOffset);

                // Calculate position with proper offset handling
                float containerOffset = ChatManager.GetStringSize(FontAssets.MouseText.Value, widestLine, Vector2.One).X;
                position += new Vector2(containerOffset + 30, 0);

                // Adjust for screen edges
                if (Main.MouseScreen.X > Main.screenWidth / 2)
                    position = new Vector2(x, y + yOffset) - new Vector2(360, 0);

                float actualHeight = 0;

                if (tooltip is SetBonusTooltip setBonus)
                {
                    actualHeight = DrawSetBonusTooltip(spriteBatch, setBonus, position, Color.White);
                }
                else if (tooltip is ProjectileTooltip projectileTooltip)
                {
                    actualHeight = DrawProjectileTooltip(spriteBatch, projectileTooltip, position);
                }
                else if (tooltip is BuffTooltip buffTooltip)
                {
                    actualHeight = DrawBuffTooltip(spriteBatch, buffTooltip, position);
                }

                // Use the actual height returned from drawing methods
                yOffset += actualHeight + 15;
            }
        }

        /// <summary>
        /// Draws keyword explanation tooltips
        /// </summary>
        public static void DrawKeywordTooltips(SpriteBatch spriteBatch, HashSet<string> keywords, string widestLine, int x, int y, int tooltipCount)
        {
            if (keywords.Count == 0) return;

            float offset = 0;
            float containerWidth = tooltipCount == 0 ? 0 : TooltipConfiguration.CONTAINER_WIDTH;

            foreach (string keyword in keywords)
            {
                Vector2 position = new Vector2(
                    x + containerWidth + TooltipConfiguration.CONTAINER_OFFSET +
                    ChatManager.GetStringSize(FontAssets.MouseText.Value, widestLine, Vector2.One).X,
                    y + offset);

                float height = CalculateKeywordHeight(keyword);
                position = AdjustForScreenBounds(position, height, x, widestLine);

                DrawKeywordTooltip(spriteBatch, keyword, position, height);
                offset += height + 5 + TooltipConfiguration.BOTTOM_PADDING;
            }
        }

        #endregion

        #region Specific Tooltip Drawing

        public static float DrawSetBonusTooltip(SpriteBatch spriteBatch, SetBonusTooltip setBonus, Vector2 containerPosition, Color primaryColor)
        {
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.Title, new Vector2(1.25f));
            var setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, Vector2.One) + new Vector2(8, 0);
            var subtitleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Set Bonus", Vector2.One);

            float descriptionHeight = 0;
            foreach (string descriptionLine in setBonus.Description)
            {
                string parsedLine = ParseTooltipObjects(descriptionLine);
                descriptionHeight += ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine, Vector2.One * 0.95f).Y;
            }

            float setItemsHeight = setBonusTitleLength.Y + 8;
            setItemsHeight += setBonus.SetItems.Count * 20;

            float actualTotalHeight = 0;

            float iconTitleHeight = Math.Max(setBonus.ObjectIcon.Height, titleSize.Y + 16);
            actualTotalHeight += iconTitleHeight;

            actualTotalHeight += 2;
            actualTotalHeight += 16;

            actualTotalHeight += subtitleSize.Y;
            actualTotalHeight += 8;

            actualTotalHeight += descriptionHeight;
            actualTotalHeight += 16;

            actualTotalHeight += 2;
            actualTotalHeight += 8;

            actualTotalHeight += setItemsHeight;

            actualTotalHeight += TooltipConfiguration.BOTTOM_PADDING;

            containerPosition = AdjustPositionForOverflow(containerPosition, actualTotalHeight);

            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)actualTotalHeight),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            DrawIconAndTitle(spriteBatch, setBonus, containerPosition, titleSize, primaryColor);

            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            var titleHeight = titleSize.Y + TooltipConfiguration.BOTTOM_OFFSET;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Set Bonus",
                new Vector2(containerPosition.X, containerPosition.Y + titleHeight + 16),
                TooltipConfiguration.SubtitleColor, 0f, Vector2.Zero, Vector2.One);

            var subtitleHeight = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Set Bonus", Vector2.One).Y;
            var descriptionStartY = titleHeight + subtitleHeight + 16;

            float currentDescriptionY = descriptionStartY;
            foreach (string descriptionLine in setBonus.Description)
            {
                string parsedLine = ParseTooltipText(descriptionLine); // This handles {Keyword:} syntax
                parsedLine = ParseTooltipObjects(parsedLine);
                
                var snippets = ChatManager.ParseMessage(parsedLine, Color.White);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, parsedLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentDescriptionY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f);

                currentDescriptionY += ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine, Vector2.One * 0.95f).Y;
            }

            var setItemsStartY = currentDescriptionY;

            DrawDivider(spriteBatch, containerPosition, (int)setItemsStartY);
            DrawSetItems(spriteBatch, setBonus, containerPosition, setBonusTitleLength, setItemsStartY + 8);

            return actualTotalHeight;
        }

        public static float DrawProjectileTooltip(SpriteBatch spriteBatch, ProjectileTooltip projectileTooltip, Vector2 containerPosition)
        {
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, projectileTooltip.Title, new Vector2(1.25f));
            var subtitleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, projectileTooltip.Type.ToString(), Vector2.One);

            float descriptionHeight = 0;
            foreach (string descriptionLine in projectileTooltip.Description)
            {
                string parsedLine = ParseTooltipObjects(descriptionLine);
                var wrappedSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);
                descriptionHeight += wrappedSize.Y;
            }

            string damageText = (projectileTooltip.ProjectileDamage * 100) + "%";
            var damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, Vector2.One * 1.25f);
            var damageTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Damage", Vector2.One * 0.8f);
            float projectileStatsHeight = Math.Max(damageSize.Y, damageTypeSize.Y);

            float actualTotalHeight = 0;

            float iconTitleHeight = Math.Max(projectileTooltip.ObjectIcon.Height, titleSize.Y + 16);
            actualTotalHeight += iconTitleHeight;

            actualTotalHeight += 2;
            actualTotalHeight += 16;

            actualTotalHeight += subtitleSize.Y;
            actualTotalHeight += 8;

            actualTotalHeight += descriptionHeight;
            actualTotalHeight += 16;

            actualTotalHeight += 2;
            actualTotalHeight += 8;

            actualTotalHeight += projectileStatsHeight;

            actualTotalHeight += TooltipConfiguration.BOTTOM_PADDING;

            containerPosition = AdjustPositionForOverflow(containerPosition, actualTotalHeight);

            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)actualTotalHeight),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            DrawIconAndTitle(spriteBatch, projectileTooltip, containerPosition, titleSize);

            var titleHeight = titleSize.Y + TooltipConfiguration.BOTTOM_OFFSET;

            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value,
                projectileTooltip.Type.ToString(),
                new Vector2(containerPosition.X, containerPosition.Y + titleHeight + 16),
                TooltipConfiguration.SubtitleColor, 0f, Vector2.Zero, Vector2.One);

            var subtitleHeight = ChatManager.GetStringSize(FontAssets.MouseText.Value, projectileTooltip.Type.ToString(), Vector2.One).Y;
            var descriptionStartY = titleHeight + subtitleHeight + 24;

            float currentDescriptionY = descriptionStartY;
            foreach (string descriptionLine in projectileTooltip.Description)
            {
                string parsedLine = ParseTooltipText(descriptionLine); // This handles {Keyword:} syntax
                parsedLine = ParseTooltipObjects(parsedLine);
                
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, parsedLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentDescriptionY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);

                var wrappedSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);
                currentDescriptionY += wrappedSize.Y;
            }

            DrawProjectileStats(spriteBatch, projectileTooltip, containerPosition, currentDescriptionY);

            return actualTotalHeight;
        }

        public static float DrawBuffTooltip(SpriteBatch spriteBatch, BuffTooltip buffTooltip, Vector2 containerPosition)
        {
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Title, new Vector2(1.25f));
            var subtitleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Type.ToString(), Vector2.One);

            float descriptionHeight = 0;
            foreach (string descriptionLine in buffTooltip.Description)
            {
                string parsedLine = ParseTooltipText(descriptionLine);
                parsedLine = ParseTooltipObjects(parsedLine);
                
                var wrappedSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);
                descriptionHeight += wrappedSize.Y;
            }

            string durationText = buffTooltip.BuffTime + " seconds";
            var durationSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, durationText, Vector2.One * 1.25f);
            var timeTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Time", Vector2.One * 0.8f);
            float buffStatsHeight = Math.Max(durationSize.Y, timeTypeSize.Y);

            float actualTotalHeight = 0;

            float iconTitleHeight = Math.Max(buffTooltip.ObjectIcon.Height, titleSize.Y + 16);
            actualTotalHeight += iconTitleHeight;

            actualTotalHeight += 2;
            actualTotalHeight += 16;

            actualTotalHeight += subtitleSize.Y;
            actualTotalHeight += 8;

            actualTotalHeight += descriptionHeight;
            actualTotalHeight += 16;

            actualTotalHeight += 2;
            actualTotalHeight += 8;

            actualTotalHeight += buffStatsHeight;

            actualTotalHeight += TooltipConfiguration.BOTTOM_PADDING;

            containerPosition = AdjustPositionForOverflow(containerPosition, actualTotalHeight);

            Color titleColor = buffTooltip.Type == BuffTooltipType.Buff ?
                TooltipConfiguration.BuffColor : TooltipConfiguration.DebuffColor;

            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)actualTotalHeight),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            DrawIconAndTitle(spriteBatch, buffTooltip, containerPosition, titleSize, titleColor);

            var titleHeight = titleSize.Y + TooltipConfiguration.BOTTOM_OFFSET;

            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value,
                buffTooltip.Type.ToString(),
                new Vector2(containerPosition.X, containerPosition.Y + titleHeight + 16),
                TooltipConfiguration.SubtitleColor, 0f, Vector2.Zero, Vector2.One);

            var subtitleHeight = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Type.ToString(), Vector2.One).Y;
            var descriptionStartY = titleHeight + subtitleHeight + 24;

            float currentDescriptionY = descriptionStartY;
            foreach (string descriptionLine in buffTooltip.Description)
            {
                string parsedLine = ParseTooltipText(descriptionLine); // This handles {Keyword:} syntax
                parsedLine = ParseTooltipObjects(parsedLine);

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, parsedLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentDescriptionY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);

                var wrappedSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, parsedLine,
                    Vector2.One * 0.95f, TooltipConfiguration.CONTAINER_WIDTH - 40);
                currentDescriptionY += wrappedSize.Y;
            }

            DrawDivider(spriteBatch, containerPosition, (int)currentDescriptionY);
            DrawBuffStats(spriteBatch, buffTooltip, containerPosition, currentDescriptionY);

            return actualTotalHeight;
        }

        private static void DrawKeywordTooltip(SpriteBatch spriteBatch, string keyword, Vector2 position, float height)
        {
            var title = Language.GetTextValue(LocalizationPath.Keywords + keyword + ".DisplayName");
            Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, title, Vector2.One);

            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)position.X - 10, (int)position.Y - 10,
                TooltipConfiguration.KEYWORD_WIDTH + 40, (int)height + TooltipConfiguration.BOTTOM_PADDING),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, title,
                position + new Vector2(titleSize.X, 24), TooltipConfiguration.KeywordColor, 0f, titleSize, Vector2.One);

            var snippets = ChatManager.ParseMessage(TooltipParser.GetKeyword(keyword), Color.White).ToArray();
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, snippets,
                new Vector2(position.X, position.Y + titleSize.Y - 4), 0f, Color.White, Vector2.Zero,
                Vector2.One * 0.95f, out _, 225f);
        }

        #endregion

        #region Helper Drawing Methods

        private static void DrawSetItems(SpriteBatch spriteBatch, SetBonusTooltip setBonus, Vector2 containerPosition, Vector2 setBonusTitleLength, float yOffset)
        {
            int setCount = 0;
            int maxSetCount = setBonus.SetItems.Count;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, setBonus.SetName,
                new Vector2(containerPosition.X, containerPosition.Y + yOffset), Color.Orange, 0f, Vector2.Zero, Vector2.One);

            Color setCountColor = setCount == maxSetCount ? Color.Orange : Color.Gray;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, $"({setCount}/{maxSetCount})",
                new Vector2(containerPosition.X + setBonusTitleLength.X, containerPosition.Y + yOffset),
                setCountColor, 0f, Vector2.Zero, Vector2.One);

            float currentY = yOffset + setBonusTitleLength.Y + 4;
            foreach (int itemID in setBonus.SetItems)
            {
                var setItem = new Item();
                setItem.SetDefaults(itemID);

                Color drawColor = CheckEquippedItem(itemID) ? new Color(255, 255, 143) : Color.Gray;
                if (drawColor != Color.Gray) setCount++;

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, " > " + setItem.Name,
                    new Vector2(containerPosition.X, containerPosition.Y + currentY),
                    drawColor, 0f, Vector2.Zero, Vector2.One);

                currentY += setBonusTitleLength.Y;
            }

            setCountColor = setCount == maxSetCount ? Color.Orange : Color.Gray;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, $"({setCount}/{maxSetCount})",
                new Vector2(containerPosition.X + setBonusTitleLength.X, containerPosition.Y + yOffset),
                setCountColor, 0f, Vector2.Zero, Vector2.One);
        }

        private static void DrawProjectileStats(SpriteBatch spriteBatch, ProjectileTooltip projectileTooltip, Vector2 containerPosition, float height)
        {
            var damageClass = projectileTooltip.DamageClass.DisplayName;
            string damageText = (projectileTooltip.ProjectileDamage) + $"{damageClass}";
            var damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, Vector2.One * 1.25f);
            var damageTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Damage", Vector2.One * 0.8f);

            DrawDivider(spriteBatch, containerPosition, (int)height);

            float statsY = height + 8;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, damageText,
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - damageSize.X - 20,
                containerPosition.Y + statsY),
                Color.Orange, 0f, Vector2.Zero, Vector2.One * 1.25f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Damage",
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - damageTypeSize.X - damageSize.X - 28,
                containerPosition.Y + statsY + 3),
                Color.Gray, 0f, Vector2.Zero, Vector2.One * 0.8f);
        }

        private static void DrawBuffStats(SpriteBatch spriteBatch, BuffTooltip buffTooltip, Vector2 containerPosition, float height)
        {
            string durationText = buffTooltip.BuffTime + " seconds";
            var durationSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, durationText, Vector2.One * 1.25f);
            var timeTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Time", Vector2.One * 0.8f);

            float statsY = height + 8;

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, durationText,
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - durationSize.X - 20,
                containerPosition.Y + statsY),
                Color.Orange, 0f, Vector2.Zero, Vector2.One * 1.25f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Time",
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - timeTypeSize.X - durationSize.X - 28,
                containerPosition.Y + statsY + 3),
                Color.Gray, 0f, Vector2.Zero, Vector2.One * 0.8f);
        }

        private static void DrawIconAndTitle(SpriteBatch spriteBatch, TooltipEntity tooltipEntity, Vector2 containerPosition, Vector2 titleSize, Color? titleColor = null)
        {
            var texture = tooltipEntity.ObjectIcon;
            Color useTitleColor = titleColor ?? TooltipConfiguration.TitleColor;

            spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, Color.White, 0f,
                texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, tooltipEntity.Title,
                containerPosition + new Vector2(texture.Width + 12, 8), useTitleColor, 0f, Vector2.Zero, new Vector2(1.25f));
        }

        private static void DrawDivider(SpriteBatch spriteBatch, Vector2 containerPosition, int yOffset)
        {
            float dividerWidth = TooltipConfiguration.CONTAINER_WIDTH - 20;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle((int)containerPosition.X, (int)containerPosition.Y + yOffset, (int)dividerWidth, 2),
                Color.Black * 0.25f);
        }

        #endregion

        #region Calculation and Positioning Methods

        public static float CalculateTooltipHeight(TooltipEntity tooltip)
        {
            Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, tooltip.Title, new Vector2(1.25f));

            float height = titleSize.Y * 2 + TooltipConfiguration.BOTTOM_OFFSET;

            foreach (string descriptionLine in tooltip.Description)
            {
                string parsedLine = ParseTooltipObjects(descriptionLine);
                var snippets = ChatManager.ParseMessage(parsedLine, Color.White).ToArray();
                foreach (var snippet in snippets)
                {
                    height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text,
                        Vector2.One * 0.95f, TooltipConfiguration.MAXIMUM_TEXT_LENGTH).Y;
                }
            }

            int coloredTextCount = 0;
            int lineBreakCount = 0;
            foreach (string descriptionLine in tooltip.Description)
            {
                string parsedLine = ParseTooltipObjects(descriptionLine);
                coloredTextCount += parsedLine.Split('[').Length - 1;
                lineBreakCount += parsedLine.Split('\n').Length - 1;
            }
            height -= (28 * coloredTextCount + 26.6f * lineBreakCount);

            if (tooltip is SetBonusTooltip setBonus)
            {
                height += setBonus.SetItems.Count * 20;
                height += ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, Vector2.One).Y * 3;
            }
            else if (tooltip is ProjectileTooltip || tooltip is BuffTooltip)
            {
                height += 50;
            }

            return height + 8;
        }

        public static float CalculateKeywordHeight(string keyword)
        {
            Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, keyword, Vector2.One);
            float height = titleSize.Y + 4;

            var snippets = ChatManager.ParseMessage(TooltipParser.GetKeyword(keyword), Color.White).ToArray();
            foreach (var snippet in snippets)
            {
                height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, new Vector2(0.95f), 225f).Y;
            }

            return height;
        }

        public static Vector2 CalculateTooltipPosition(int x, int y, float yOffset, string widestLine, float height)
        {
            float containerOffset = ChatManager.GetStringSize(FontAssets.MouseText.Value, widestLine, Vector2.One).X;
            Vector2 position = new Vector2(x, y + yOffset) + new Vector2(containerOffset + 30, 0);

            if (Main.MouseScreen.X > Main.screenWidth / 2)
                position = new Vector2(x, y + yOffset) - new Vector2(360, 0);

            if (y + yOffset + height > Main.screenHeight)
            {
                float yOverflow = y + yOffset + height - Main.screenHeight;
                position -= new Vector2(0, yOverflow);
            }

            return position;
        }

        public static Vector2 AdjustForScreenBounds(Vector2 position, float height, int x, string widestLine)
        {
            if (position.Y + height > Main.screenHeight)
            {
                float yOverflow = position.Y + height - Main.screenHeight;
                position -= new Vector2(0, yOverflow);
            }

            if (Main.MouseScreen.X > Main.screenWidth / 2)
            {
                position = new Vector2(
                    x + TooltipConfiguration.RIGHT_OFFSET - TooltipConfiguration.CONTAINER_WIDTH -
                    ChatManager.GetStringSize(FontAssets.MouseText.Value, widestLine, Vector2.One).X,
                    position.Y);
            }

            return position;
        }

        private static Vector2 AdjustPositionForOverflow(Vector2 containerPosition, float height)
        {
            if (containerPosition.Y + height > Main.screenHeight)
            {
                float yOverflow = containerPosition.Y + height - Main.screenHeight;
                containerPosition.Y -= yOverflow;
            }
            return containerPosition;
        }

        private static bool CheckEquippedItem(int id)
        {
            for (int i = 0; i < 8; i++)
            {
                if (Main.LocalPlayer.armor[i].type == id) return true;
            }
            return false;
        }

        #endregion

        #region Text Parsing Methods

        public static string ParseTooltipText(string text)
        {
            string convertedText = text;

            foreach (var highlight in TextHighlights)
            {
                string pattern = $@"({{{highlight.Key}:.*?}})";
                var matches = Regex.Matches(text, pattern);
                foreach (Match match in matches)
                {
                    string newValue = match.Value.Replace($"{{{highlight.Key}:", $"[c/{highlight.Value}:");
                    convertedText = convertedText.Replace(match.Value, newValue);
                }
            }

            return convertedText.Replace("}", "]");
        }

        public static string ParseTooltipObjects(string text)
        {
            string convertedText = text;
            foreach (var highlight in ObjectHighlights)
            {
                string pattern = $@"(<{highlight.Key}:.*?>)";
                var matches = Regex.Matches(text, pattern);
                foreach (Match match in matches)
                {
                    string content = match.Value.Replace($"<{highlight.Key}:", "").Replace(">", "");
                    string newValue = $"[c/{highlight.Value}:{content}]";
                    convertedText = convertedText.Replace(match.Value, newValue);
                }
            }

            if (convertedText.Contains("DeathsDoor"))
            {
                convertedText = convertedText.Replace("DeathsDoor", "Deaths Door");
            }

            if (convertedText.Contains("MindDown"))
            {
                convertedText = convertedText.Replace("MindDown", "Mind Down");
            }

            return convertedText;
        }

        #endregion
    }
}