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
                float height = CalculateTooltipHeight(tooltip);
                Vector2 position = CalculateTooltipPosition(x, y, yOffset, widestLine, height);

                // Draw based on tooltip type
                if (tooltip is SetBonusTooltip setBonus)
                {
                    DrawSetBonusTooltip(spriteBatch, setBonus, position, height, Color.White);
                }
                else if (tooltip is ProjectileTooltip projectileTooltip)
                {
                    DrawProjectileTooltip(spriteBatch, projectileTooltip, position, height);
                }
                else if (tooltip is BuffTooltip buffTooltip)
                {
                    DrawBuffTooltip(spriteBatch, buffTooltip, position, height);

                    // uhh i dont know how to fix it in the other code and im too lazy to
                    yOffset += 11;
                }

                yOffset += height + 5;
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

        public static void DrawSetBonusTooltip(SpriteBatch spriteBatch, SetBonusTooltip setBonus, Vector2 containerPosition, float height, Color primaryColor)
        {
            // Pre-calculate all text sizes
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.Title, new Vector2(1.25f));
            var setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, Vector2.One) + new Vector2(8, 0);
            var subtitleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Set Bonus", Vector2.One);

            // Calculate description height - now works with string array
            float descriptionHeight = 0;
            foreach (string descriptionLine in setBonus.Description)
            {
                descriptionHeight += ChatManager.GetStringSize(FontAssets.MouseText.Value, descriptionLine, Vector2.One * 0.95f).Y;
            }

            // Calculate set items height
            float setItemsHeight = setBonusTitleLength.Y + 8; // Set name + padding
            setItemsHeight += setBonus.SetItems.Count * 20; // Each item is 20px tall

            // Calculate ACTUAL total height of all elements
            float actualTotalHeight = 0;

            // 1. Icon/Title section height (use the larger of icon or title)
            float iconTitleHeight = Math.Max(setBonus.ObjectIcon.Height, titleSize.Y + 16); // +16 for positioning offset
            actualTotalHeight += iconTitleHeight;

            // 2. First divider
            actualTotalHeight += 2; // Divider height
            actualTotalHeight += 16; // Padding after divider

            // 3. "Set Bonus" subtitle
            actualTotalHeight += subtitleSize.Y;
            actualTotalHeight += 8; // Padding after subtitle

            // 4. Description text
            actualTotalHeight += descriptionHeight;
            actualTotalHeight += 16; // Padding after description

            // 5. Second divider
            actualTotalHeight += 2; // Divider height
            actualTotalHeight += 8; // Padding after divider

            // 6. Set items section
            actualTotalHeight += setItemsHeight;

            // 7. Bottom padding
            actualTotalHeight += TooltipConfiguration.BOTTOM_PADDING;

            // Draw background with the ACTUAL calculated height
            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)actualTotalHeight),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            // Draw icon and title
            DrawIconAndTitle(spriteBatch, setBonus, containerPosition, titleSize, primaryColor);

            // Draw divider
            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            // Calculate title section height
            var titleHeight = titleSize.Y + TooltipConfiguration.BOTTOM_OFFSET;

            // Draw "Set Bonus" subtitle
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Set Bonus",
                new Vector2(containerPosition.X, containerPosition.Y + titleHeight + 16),
                TooltipConfiguration.SubtitleColor, 0f, Vector2.Zero, Vector2.One);

            // Calculate where the description starts (after subtitle)
            var subtitleHeight = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Set Bonus", Vector2.One).Y;
            var descriptionStartY = titleHeight + subtitleHeight + 16; // 8 padding above + 8 padding below subtitle

            // Draw set bonus description - now handles string array
            float currentDescriptionY = descriptionStartY;
            foreach (string descriptionLine in setBonus.Description)
            {
                var snippets = ChatManager.ParseMessage(descriptionLine, Color.White);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, descriptionLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentDescriptionY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f);

                currentDescriptionY += ChatManager.GetStringSize(FontAssets.MouseText.Value, descriptionLine, Vector2.One * 0.95f).Y;
            }

            // Calculate where set items should start (right after description ends)
            var setItemsStartY = currentDescriptionY; // No extra padding yet

            // Draw another divider before set items
            DrawDivider(spriteBatch, containerPosition, (int)setItemsStartY);
            // Draw set items starting right after the divider
            DrawSetItems(spriteBatch, setBonus, containerPosition, setBonusTitleLength, setItemsStartY + 8);
        }

        public static void DrawProjectileTooltip(SpriteBatch spriteBatch, ProjectileTooltip projectileTooltip, Vector2 containerPosition, float height)
        {
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, projectileTooltip.Title, new Vector2(1.25f));

            // Adjust for screen overflow
            containerPosition = AdjustPositionForOverflow(containerPosition, height);

            // Draw background
            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)height),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            // Draw icon and title
            DrawIconAndTitle(spriteBatch, projectileTooltip, containerPosition, titleSize);

            // Draw subtitle (projectile type)
            var titleHeight = titleSize.Y * 2 + TooltipConfiguration.BOTTOM_OFFSET;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value,
                projectileTooltip.Type.ToString(),
                new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight),
                TooltipConfiguration.SubtitleColor, 0f, titleSize, Vector2.One);

            // Draw divider
            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            // Draw description - handle string array
            float currentY = titleHeight;
            foreach (string descriptionLine in projectileTooltip.Description)
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, descriptionLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f);
                //currentY += lineSize.Y;
            }

            // Draw damage value
            DrawProjectileStats(spriteBatch, projectileTooltip, containerPosition, height);
        }

        public static void DrawBuffTooltip(SpriteBatch spriteBatch, BuffTooltip buffTooltip, Vector2 containerPosition, float height)
        {
            // Pre-calculate all text sizes
            var titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Title, new Vector2(1.25f));
            var subtitleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Type.ToString(), Vector2.One);

            // Calculate description height - now works with string array
            float descriptionHeight = 0;
            foreach (string descriptionLine in buffTooltip.Description)
            {
                descriptionHeight += ChatManager.GetStringSize(FontAssets.MouseText.Value, descriptionLine, Vector2.One * 0.95f).Y;
            }

            // Calculate ACTUAL total height of all elements
            float actualTotalHeight = 0;

            // 1. Icon/Title section height (use the larger of icon or title)
            float iconTitleHeight = Math.Max(buffTooltip.ObjectIcon.Height, titleSize.Y + 16); // +16 for positioning offset
            actualTotalHeight += iconTitleHeight;

            // 2. Subtitle (buff/debuff type)
            actualTotalHeight += subtitleSize.Y;
            actualTotalHeight += 8; // Padding after subtitle

            // 3. First divider
            actualTotalHeight += 2; // Divider height
            actualTotalHeight += 16; // Padding after divider

            // 4. Description text
            actualTotalHeight += descriptionHeight;
            actualTotalHeight += 16; // Padding after description

            // 5. Duration/stats section (if applicable)
            // Add height calculation for buff stats here if needed
            // actualTotalHeight += buffStatsHeight;

            // 6. Bottom padding
            actualTotalHeight += TooltipConfiguration.BOTTOM_PADDING;

            // Adjust for screen overflow
            containerPosition = AdjustPositionForOverflow(containerPosition, actualTotalHeight);

            // Set title color based on buff/debuff type
            Color titleColor = buffTooltip.Type == BuffTooltipType.Buff ?
                TooltipConfiguration.BuffColor : TooltipConfiguration.DebuffColor;

            // Draw background with the calculated height
            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10,
                (int)TooltipConfiguration.CONTAINER_WIDTH, (int)actualTotalHeight + 16),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            // Draw icon and title with appropriate color
            DrawIconAndTitle(spriteBatch, buffTooltip, containerPosition, titleSize, titleColor);

            // Calculate title section height
            var titleHeight = titleSize.Y + TooltipConfiguration.BOTTOM_OFFSET;

            // Draw divider
            DrawDivider(spriteBatch, containerPosition, TooltipConfiguration.DIVIDER_OFFSET);

            // Draw subtitle (buff/debuff type)
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value,
                buffTooltip.Type.ToString(),
                new Vector2(containerPosition.X, containerPosition.Y + titleHeight + 16),
                TooltipConfiguration.SubtitleColor, 0f, Vector2.Zero, Vector2.One);

            // Calculate where the description starts (after subtitle)
            var subtitleHeight = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.Type.ToString(), Vector2.One).Y;
            var descriptionStartY = titleHeight + subtitleHeight + 16;

            // Draw description - handle string array
            float currentDescriptionY = descriptionStartY;
            foreach (string descriptionLine in buffTooltip.Description)
            {
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, descriptionLine,
                    new Vector2(containerPosition.X, containerPosition.Y + currentDescriptionY), Color.White, 0f, Vector2.Zero,
                    Vector2.One * 0.95f);

                // Move to next line
                currentDescriptionY += ChatManager.GetStringSize(FontAssets.MouseText.Value, descriptionLine, Vector2.One * 0.95f).Y;
            }

            DrawDivider(spriteBatch, containerPosition, (int)currentDescriptionY);
            DrawBuffStats(spriteBatch, buffTooltip, containerPosition, currentDescriptionY);
        }

        private static void DrawKeywordTooltip(SpriteBatch spriteBatch, string keyword, Vector2 position, float height)
        {
            var title = Language.GetTextValue(LocalizationPath.Keywords + keyword + ".DisplayName");
            Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, title, Vector2.One);

            // Draw background
            Utils.DrawInvBG(spriteBatch,
                new Rectangle((int)position.X - 10, (int)position.Y - 10,
                TooltipConfiguration.KEYWORD_WIDTH + 40, (int)height + TooltipConfiguration.BOTTOM_PADDING),
                TooltipConfiguration.PrimaryBackgroundColor * 0.925f);

            // Draw keyword title
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, title,
                position + new Vector2(titleSize.X, 24), TooltipConfiguration.KeywordColor, 0f, titleSize, Vector2.One);

            // Draw keyword description
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

            // Draw set name
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, setBonus.SetName,
                new Vector2(containerPosition.X, containerPosition.Y + yOffset), Color.Orange, 0f, Vector2.Zero, Vector2.One);

            // Draw set count next to set name
            Color setCountColor = setCount == maxSetCount ? Color.Orange : Color.Gray;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, $"({setCount}/{maxSetCount})",
                new Vector2(containerPosition.X + setBonusTitleLength.X, containerPosition.Y + yOffset),
                setCountColor, 0f, Vector2.Zero, Vector2.One);

            // Draw each set item
            float currentY = yOffset + setBonusTitleLength.Y + 4; // Small padding after set name
            foreach (int itemID in setBonus.SetItems)
            {
                var setItem = new Item();
                setItem.SetDefaults(itemID);

                Color drawColor = CheckEquippedItem(itemID) ? new Color(255, 255, 143) : Color.Gray;
                if (drawColor != Color.Gray) setCount++;

                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, " > " + setItem.Name,
                    new Vector2(containerPosition.X, containerPosition.Y + currentY),
                    drawColor, 0f, Vector2.Zero, Vector2.One);

                // Move to next line
                currentY += setBonusTitleLength.Y;
            }

            // Update the set count now that we've counted equipped items
            setCountColor = setCount == maxSetCount ? Color.Orange : Color.Gray;
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, $"({setCount}/{maxSetCount})",
                new Vector2(containerPosition.X + setBonusTitleLength.X, containerPosition.Y + yOffset),
                setCountColor, 0f, Vector2.Zero, Vector2.One);
        }

        private static void DrawProjectileStats(SpriteBatch spriteBatch, ProjectileTooltip projectileTooltip, Vector2 containerPosition, float height)
        {
            string damageText = (projectileTooltip.ProjectileDamage * 100) + "%";
            var damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, new Vector2(1.25f));
            var damageTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Damage", new Vector2(0.8f));

            float descriptionHeight = height - 24;

            // Draw bottom divider
            DrawDivider(spriteBatch, containerPosition, (int)descriptionHeight);

            // Draw damage percentage
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, damageText,
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - damageSize.X - 30,
                containerPosition.Y + descriptionHeight + 12),
                Color.Orange, 0f, Vector2.Zero, Vector2.One * 1.25f);

            // Draw "Damage" label
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Damage",
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - damageTypeSize.X - damageSize.X - 35,
                containerPosition.Y + descriptionHeight + 18),
                Color.Gray, 0f, Vector2.Zero, Vector2.One * 0.8f);
        }

        private static void DrawBuffStats(SpriteBatch spriteBatch, BuffTooltip buffTooltip, Vector2 containerPosition, float height)
        {
            string durationText = buffTooltip.BuffTime + " seconds";
            var durationSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, durationText, Vector2.One * 1.25f);
            var timeTypeSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Time", Vector2.One * 0.8f);

            // Calculate positioning for right-aligned stats
            float statsY = height + 8; // Add some padding after divider

            // Draw duration value (right-aligned)
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, durationText,
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - durationSize.X - 20,
                containerPosition.Y + statsY),
                Color.Orange, 0f, Vector2.Zero, Vector2.One * 1.25f);

            // Draw "Time" label to the left of duration value
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, "Time",
                new Vector2(containerPosition.X + TooltipConfiguration.CONTAINER_WIDTH - timeTypeSize.X - durationSize.X - 28,
                containerPosition.Y + statsY + 3), // +3 to align baselines of different sized text
                Color.Gray, 0f, Vector2.Zero, Vector2.One * 0.8f);
        }

        private static void DrawIconAndTitle(SpriteBatch spriteBatch, TooltipEntity tooltipEntity, Vector2 containerPosition, Vector2 titleSize, Color? titleColor = null)
        {
            var texture = tooltipEntity.ObjectIcon;
            Color useTitleColor = titleColor ?? TooltipConfiguration.TitleColor;

            // Draw icon
            spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, Color.White, 0f,
                texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

            // Draw title
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

            // Handle string array description
            foreach (string descriptionLine in tooltip.Description)
            {
                var snippets = ChatManager.ParseMessage(descriptionLine, Color.White).ToArray();
                foreach (var snippet in snippets)
                {
                    height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text,
                        Vector2.One * 0.95f, TooltipConfiguration.MAXIMUM_TEXT_LENGTH).Y;
                }
            }

            // Adjust for colored text and line breaks - now check each line
            int coloredTextCount = 0;
            int lineBreakCount = 0;
            foreach (string descriptionLine in tooltip.Description)
            {
                coloredTextCount += descriptionLine.Split('[').Length - 1;
                lineBreakCount += descriptionLine.Split('\n').Length - 1;
            }
            height -= (28 * coloredTextCount + 26.6f * lineBreakCount);

            // Add extra space for specific tooltip types
            if (tooltip is SetBonusTooltip setBonus)
            {
                height += setBonus.SetItems.Count * 20;
                height += ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, Vector2.One).Y * 3;
            }
            else if (tooltip is ProjectileTooltip || tooltip is BuffTooltip)
            {
                height += 50; // Space for damage/duration display
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

            // Adjust for screen bounds
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
                string pattern = $@"({{{highlight.Key}:.*}})";
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
                string pattern = $@"(<{highlight.Key}:.*>)";
                var matches = Regex.Matches(text, pattern);
                foreach (Match match in matches)
                {
                    string newValue = match.Value.Replace($"<{highlight.Key}:", $"[c/{highlight.Value}:{{");
                    convertedText = convertedText.Replace(match.Value, newValue);
                }
            }
            return convertedText.Replace(">", "}]");
        }

        #endregion
    }
}