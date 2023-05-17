using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Accessories.CapturedMirage;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace OvermorrowMod.Common
{
    public class TooltipObject
    {
        public int Priority { get; protected set; }
        public Texture2D ObjectIcon { get; protected set; }
        public string Title { get; protected set; }
        public string Description { get; protected set; }
    }

    public class SetBonusTooltip : TooltipObject
    {
        public readonly string SetName;
        public readonly List<int> SetItems;

        public SetBonusTooltip(Texture2D SetIcon, string SetTitle, string SetName, string SetDescription, List<int> SetItems)
        {
            this.Priority = 1;

            this.ObjectIcon = SetIcon;
            this.Title = SetTitle;
            this.Description = SetDescription;
            this.SetName = SetName;
            this.SetItems = SetItems;
        }
    }

    public enum ProjectileTooltipType
    {
        Projectile,
        Minion
    }

    public class ProjectileTooltip : TooltipObject
    {
        public readonly float ProjectileDamage;
        public readonly ProjectileTooltipType Type;

        public ProjectileTooltip(Texture2D ProjectileIcon, string ProjectileTitle, string ProjectileDescription, float ProjectileDamage, ProjectileTooltipType Type)
        {
            this.Priority = 2;

            this.ObjectIcon = ProjectileIcon;
            this.Title = ProjectileTitle;
            this.Description = ProjectileDescription;
            this.ProjectileDamage = ProjectileDamage;
            this.Type = Type;
        }
    }


    public enum BuffTooltipType
    {
        Buff,
        Debuff
    }

    public class BuffTooltip : TooltipObject
    {
        public readonly float BuffTime;
        public readonly BuffTooltipType Type;

        public BuffTooltip(Texture2D BuffIcon, string BuffTitle, string BuffDescription, float BuffTime, BuffTooltipType Type)
        {
            this.Priority = 3;

            this.ObjectIcon = BuffIcon;
            this.Title = BuffTitle;
            this.Description = BuffDescription;
            this.BuffTime = BuffTime;
            this.Type = Type;
        }
    }

    public partial class OvermorrowGlobalItem : GlobalItem
    {
        public List<TooltipObject> TooltipObjects = new List<TooltipObject>();
        public override void SetDefaults(Item item)
        {
            if (item.type == ItemID.WoodHelmet || item.type == ItemID.WoodBreastplate || item.type == ItemID.WoodGreaves)
            {
                TooltipObjects.Add(new SetBonusTooltip(ModContent.Request<Texture2D>("OvermorrowMod/Assets/Unused/Buffs/Test").Value,
                    "Wooden Warrior",
                    "Wood Armor",
                    " + Increased defense by [c/58D68D:1]\n + Increased damage by [c/58D68D:5]\n + [c/58D68D:5%] chance to instantly kill all enemies",
                    new List<int>()
                    {
                        ItemID.WoodHelmet,
                        ItemID.WoodBreastplate,
                        ItemID.WoodGreaves
                    }));
            }

            if (item.type == ItemID.CowboyHat || item.type == ItemID.CowboyJacket || item.type == ItemID.CowboyPants)
            {
                TooltipObjects.Add(new SetBonusTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Wild West Deadeye",
                    "Cowboy Armor",
                    " + Critical hits with [c/FAD5A5:Revolvers] rebound to the nearest enemy",
                    new List<int>()
                    {
                        ItemID.CowboyHat,
                        ItemID.CowboyJacket,
                        ItemID.CowboyPants
                    }));
            }

            if (item.type == ModContent.ItemType<CapturedMirage>())
            {
                TooltipObjects.Add(new ProjectileTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Mirage Arrow",
                    " + Copies the effect of another random [c/FAD5A5:Arrow] from your ammo slots",
                    0.5f,
                    ProjectileTooltipType.Projectile));
            }

            if (item.type == ItemID.PhoenixBlaster)
            {
                TooltipObjects.Add(new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Phoenix Mark",
                    " + Increases all incoming damage by [c/58D68D:15%]",
                    6,
                    BuffTooltipType.Debuff));
                TooltipObjects.Add(new BuffTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "On Fire!",
                    " - Prevents health regeneration\n" +
                    " - Loses [c/ff5555:4] health per second",
                    4,
                    BuffTooltipType.Debuff));
            }

            base.SetDefaults(item);
        }

        private int GetColoredTextCount(string description)
        {
            string[] splitText = description.Split('[');

            return splitText.Length - 1;
        }

        private int GetLineBreakCount(string description)
        {
            string[] splitText = description.Split('\n');

            return splitText.Length - 1;
        }

        private string GetKeyword(string id)
        {
            XmlDocument xmlDoc = ModUtils.GetXML("Common/Tooltips/Keywords.xml");
            var keywordList = xmlDoc.GetElementsByTagName("Keyword");

            foreach (XmlNode node in keywordList)
            {
                string nodeID = node.Attributes["id"].Value;
                if (nodeID == id)
                {
                    foreach (XmlNode info in node.ChildNodes)
                    {
                        if (info.Name == "Description") return info.InnerText;
                    }
                }
            }

            return "";
        }

        private bool CheckEquippedItem(int id)
        {
            // Check armor slots (0 - 2) and accessory slots (3 - 7)
            for (int i = 0; i < 8; i++)
            {
                if (Main.LocalPlayer.armor[i].type == id) return true;
            }

            return false;
        }

        // From https://stackoverflow.com/a/12108823
        private string[] GetKeywords(string text)
        {
            string filtered = string.Join(";", Regex.Matches(text, @"\<(.+?)\>")
                                    .Cast<Match>()
                                    .Select(m => m.Groups[1].Value));
            return filtered.Split(';');
        }


        HashSet<string> KeyWords = new HashSet<string>();
        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            // Sort the tooltips based on priority
            var orderedTooltips = TooltipObjects.OrderBy(x => x.Priority).ToList();

            foreach (TooltipLine line in lines)
            {
                string[] lineKeywords = GetKeywords(line.Text);
                if (lineKeywords.Length > 0)
                {
                    foreach (string lineKeyword in lineKeywords)
                        if (lineKeyword.Length > 0) KeyWords.Add(lineKeyword);
                }
            }

            float CONTAINER_WIDTH = 350;

            string widest = lines.OrderBy(n => ChatManager.GetStringSize(FontAssets.MouseText.Value, n.Text, Vector2.One).X).Last().Text;
            if (orderedTooltips.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                float yOffset = 0;

                foreach (TooltipObject tooltipObject in TooltipObjects)
                {
                    float MAXIMUM_LENGTH = 330;
                    int BOTTOM_OFFSET = 20;
                    int DIVIDER_OFFSET = 48;

                    float containerOffset = ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X;
                    float dividerWidth = CONTAINER_WIDTH - 20;

                    float height = 14;

                    Vector2 baseTextSize = new Vector2(1f);
                    Vector2 containerPosition = new Vector2(x, y + yOffset) + new Vector2(containerOffset + 30, 0);
                    Vector2 titleOffset = new Vector2(100, 14);

                    Color color = true ? new Color(28, 31, 77) : new Color(25, 27, 27);
                    Color titleColor = true ? new Color(139, 233, 253) : Color.Gray;
                    Color subtitleColor = /*new Color(178, 190, 181)*/new Color(241, 250, 140);
                    Color primaryColor = true ? Color.White : Color.Gray;

                    Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, tooltipObject.Title, new Vector2(1.25f));
                    TextSnippet[] snippets = ChatManager.ParseMessage(tooltipObject.Description, Color.White).ToArray();
                    int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;

                    height += titleSize.Y * 2 + BOTTOM_OFFSET; // this is the title/subtitle
                    float titleHeight = height;

                    foreach (TextSnippet snippet in snippets)
                        height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, Vector2.One * 0.95f).Y;

                    float unoffsetColoredText = 28 * GetColoredTextCount(tooltipObject.Description) + (26.6f * GetLineBreakCount(tooltipObject.Description));
                    height -= unoffsetColoredText;
                    height += 8;

                    if (tooltipObject is SetBonusTooltip setBonus)
                    {
                        // Offset the title and subtitles for the icon
                        Vector2 setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, baseTextSize) + new Vector2(8, 0);
                        float descriptionHeight = height;

                        height += setBonusTitleLength.Y * 3;
                        height += setBonus.SetItems.Count * 20;

                        if (Main.MouseScreen.X > Main.screenWidth / 2)
                            containerPosition = new Vector2(x, y) - new Vector2(360, 0);

                        if (y + height > Main.screenHeight) // y-Overflow check
                        {
                            float yOverflow = y + height - Main.screenHeight;
                            containerPosition -= new Vector2(0, yOverflow);
                        }

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)CONTAINER_WIDTH, (int)height), color * 0.925f);

                        #region Title
                        Texture2D texture = setBonus.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, tooltipObject.Title, containerPosition + new Vector2(texture.Width + 12, 8), titleColor, 0f, Vector2.Zero, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + DIVIDER_OFFSET, (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Set Bonus
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Set Bonus", new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight), subtitleColor, 0f, titleSize, baseTextSize);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight), (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Set
                        int setCount = 0;
                        int maxSetCount = setBonus.SetItems.Count;

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, setBonus.SetName, new Vector2(containerPosition.X, containerPosition.Y + descriptionHeight + 8), Color.Orange, 0f, Vector2.Zero, baseTextSize);

                        int itemOffsetCount = 1;
                        foreach (int itemID in setBonus.SetItems)
                        {
                            Item setItem = new Item();
                            setItem.SetDefaults(itemID);
                            Color drawColor = Color.Gray;
                            if (CheckEquippedItem(itemID))
                            {
                                drawColor = new Color(255, 255, 143);
                                setCount++;
                            }

                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, " > " + setItem.Name, new Vector2(containerPosition.X, containerPosition.Y + descriptionHeight + (setBonusTitleLength.Y * itemOffsetCount) + 8), drawColor, 0f, Vector2.Zero, baseTextSize);
                            itemOffsetCount++;
                        }

                        Color setCountColor = setCount == maxSetCount ? Color.Orange : Color.Gray;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "(" + setCount + "/" + maxSetCount + ")", new Vector2(containerPosition.X + setBonusTitleLength.X, containerPosition.Y + descriptionHeight + 8), setCountColor, 0f, Vector2.Zero, baseTextSize);
                        #endregion
                    }

                    // I don't know what I was doing aaaaaaaaaaaah
                    if (tooltipObject is ProjectileTooltip || tooltipObject is BuffTooltip)
                    {
                        string damageText;
                        Vector2 damageSize;
                        Vector2 damageType;

                        float descriptionHeight = height;
                        height -= 24; // removes extra padding at the bottom, this might be unnecessary?

                        float yOverflow = 0;

                        // I didn't know how to even generalize this so its literally the same shit
                        if (tooltipObject is ProjectileTooltip projectileTooltip)
                        {
                            damageText = (projectileTooltip.ProjectileDamage * 100) + "%";
                            damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, new Vector2(1.25f));
                            damageType = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Damage", new Vector2(0.8f));

                            height += damageSize.Y * 3;
                            if (y + height > Main.screenHeight) // y-Overflow check
                            {
                                yOverflow = y + height - Main.screenHeight;
                            }

                            if (Main.MouseScreen.X > Main.screenWidth / 2)
                                containerPosition = new Vector2(x, y) - new Vector2(containerOffset + 30, 0);

                            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)(containerPosition.Y - yOverflow - 10), (int)CONTAINER_WIDTH, (int)height), color * 0.925f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, projectileTooltip.Type.ToString(), new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight - yOverflow), subtitleColor, 0f, titleSize, baseTextSize);

                            #region Values
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, damageText, new Vector2(containerPosition.X + CONTAINER_WIDTH - damageSize.X - 30, containerPosition.Y + descriptionHeight - yOverflow + 12), Color.Orange, 0f, Vector2.Zero, baseTextSize * 1.25f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Damage", new Vector2(containerPosition.X + CONTAINER_WIDTH - damageType.X - damageSize.X - 35, containerPosition.Y + descriptionHeight - yOverflow + 18), Color.Gray, 0f, Vector2.Zero, baseTextSize * 0.8f);
                            #endregion
                        }
                        else if (tooltipObject is BuffTooltip buffTooltip)
                        {
                            damageText = buffTooltip.BuffTime + " seconds";
                            damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, new Vector2(1.25f));
                            damageType = ChatManager.GetStringSize(FontAssets.MouseText.Value, "Time", new Vector2(0.8f));

                            height += damageSize.Y * 3;

                            if (y + height > Main.screenHeight) // y-Overflow check
                            {
                                yOverflow = y + height - Main.screenHeight;
                            }

                            if (Main.MouseScreen.X > Main.screenWidth / 2)
                                containerPosition = new Vector2(x, y) - new Vector2(containerOffset + 10, 0);

                            Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)(containerPosition.Y - yOverflow - 10), (int)CONTAINER_WIDTH, (int)height), color * 0.925f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, buffTooltip.Type.ToString(), new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight - yOverflow), subtitleColor, 0f, titleSize, baseTextSize);

                            #region Values
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, damageText, new Vector2(containerPosition.X + CONTAINER_WIDTH - damageSize.X - 30, containerPosition.Y + descriptionHeight - yOverflow + 12), Color.Orange, 0f, Vector2.Zero, baseTextSize * 1.25f);
                            ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Time", new Vector2(containerPosition.X + CONTAINER_WIDTH - damageType.X - damageSize.X - 35, containerPosition.Y + descriptionHeight - yOverflow + 18), Color.Gray, 0f, Vector2.Zero, baseTextSize * 0.8f);
                            #endregion
                        }

                        #region Title
                        Texture2D texture = tooltipObject.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f - new Vector2(0, yOverflow), null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, tooltipObject.Title, containerPosition + new Vector2(texture.Width + 12, 8) - new Vector2(0, yOverflow), titleColor, 0f, Vector2.Zero, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + DIVIDER_OFFSET - yOverflow), (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Description
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight - yOverflow), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight - yOverflow), (int)dividerWidth, 2), Color.Black * 0.25f);
                    }

                    //if (containerOffset > keywordOffset) keywordOffset = containerOffset;
                    yOffset += height + 5;
                }
            }

            if (KeyWords.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                float offset = 0;
                int BOTTOM_PADDING = 14;
                int CONTAINER_OFFSET = 35;

                if (orderedTooltips.Count == 0) CONTAINER_WIDTH = 0;

                foreach (string keyWord in KeyWords)
                {
                    //Vector2 containerPosition = new Vector2(x, y + offset) + new Vector2(ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X, 0);
                    Vector2 containerPosition = new Vector2(x + CONTAINER_WIDTH + CONTAINER_OFFSET + ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X, y + offset);

                    Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, keyWord, new Vector2(1f));
                    float height = titleSize.Y + 4;

                    float titleHeight = height;

                    TextSnippet[] snippets = ChatManager.ParseMessage(GetKeyword(keyWord), Color.White).ToArray();
                    float MAXIMUM_LENGTH = 225;
                    float KEYWORD_WIDTH = 200;

                    foreach (TextSnippet snippet in snippets)
                    {
                        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, new Vector2(0.95f), MAXIMUM_LENGTH);
                        height += textSize.Y;
                        //if (textSize.X > keywordWidth) keywordWidth = textSize.X;
                    }

                    if (y + height > Main.screenHeight)// y-Overflow check
                    {
                        float yOverflow = y + height - Main.screenHeight;
                        containerPosition -= new Vector2(0, yOverflow);
                    }

                    int RIGHT_OFFSET = 75;
                    if (Main.MouseScreen.X > Main.screenWidth / 2)
                        containerPosition = new Vector2(x + RIGHT_OFFSET - CONTAINER_WIDTH - ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X, y + offset);

                    Color color = new Color(28, 31, 77);
                    Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)KEYWORD_WIDTH + 40, (int)height + BOTTOM_PADDING), color * 0.925f);

                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, keyWord, containerPosition + new Vector2(titleSize.X, 24), new Color(255, 121, 198), 0f, titleSize, new Vector2(1f));
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight - 8), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);

                    offset += height + 5 + BOTTOM_PADDING;
                }

                //Main.NewText(string.Join(",", KeyWords));
            }

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }

        /// <summary>
        /// Searches for all buffs. If isBuff is set to false, searches for debuffs instead.
        /// </summary>
        /// <returns>An array of all buffs/debuffs found within the string.</returns>
        private string[] GetBuff(string text, bool isBuff = true)
        {
            string pattern = isBuff ? @"\<Buff:(.+?)\>" : @"\<Debuff:(.+?)\>";
            string filtered = string.Join(";", Regex.Matches(text, pattern)
                                    .Cast<Match>()
                                    .Select(m => m.Groups[1].Value));
            return filtered.Split(';');
        }

        // TODO: Add a thing for buffs later
        private string ConvertBuffWords(string text)
        {
            string convertedText = text;

            string pattern = @"(<Debuff:.*>)";
            string filtered = string.Join(";", Regex.Matches(text, pattern)
                                               .Cast<Match>()
                                               .Select(m => m.Groups[1].Value));

            MatchCollection matches = Regex.Matches(text, pattern);
            foreach (Match match in matches)
            {
                string newValue = match.Value.Replace("<Debuff:", "[c/ff5555:[");
                newValue = newValue.Replace(">", "][c/ff5555:]]");

                convertedText = convertedText.Replace(match.Value, newValue);
            }

            return convertedText;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Probably a better way to do this?
            #region Gun Replacements
            if (item.type == ItemID.PhoenixBlaster)
            {
                tooltips.Add(new TooltipLine(Mod, "PhoenixBlaster0", "<Reload>: Release a burst of flame, inflicting <Debuff:Phoenix Mark>"));
                tooltips.Add(new TooltipLine(Mod, "PhoenixBlaster1", "<Fail>: Damage yourself, inflicting <Debuff:On Fire!>"));
            }

            if (item.type == ItemID.Musket)
            {
                tooltips.Add(new TooltipLine(Mod, "Musket0", "<Reload>: Increase accuracy for each block clicked"));
            }

            if (item.type == ItemID.Boomstick)
            {
                tooltips.Add(new TooltipLine(Mod, "Boomstick0", "<Reload>: Increase recoil and bullets fired for each block clicked"));
            }

            if (item.type == ItemID.Revolver)
            {
                tooltips.Add(new TooltipLine(Mod, "Revolver0", "<Reload>: Reload instantly. Your next clip has increased damage"));
            }

            if (item.type == ItemID.Handgun)
            {
                tooltips.Add(new TooltipLine(Mod, "Handgun0", "<Reload>: Your next clip has increased firing speed"));
            }

            if (item.type == ItemID.TheUndertaker)
            {
                tooltips.Add(new TooltipLine(Mod, "Undertaker0", "<Reload>: Your next clip has 6 bullets and increased firing speed"));
                tooltips.Add(new TooltipLine(Mod, "Undertaker1", "<Fail>: Your next clip has 2 bullets"));
                tooltips.Add(new TooltipLine(Mod, "Undertaker2", "Bullets deal more damage at point blank range"));
            }

            if (item.type == ItemID.QuadBarrelShotgun)
            {
                tooltips.Add(new TooltipLine(Mod, "Quadbarrel0", "<Reload>: Increase number of bullets and recoil for each block clicked"));
            }
            #endregion

            if (TooltipObjects.Count > 0 || KeyWords.Count > 0)
            {
                if (!Main.keyState.IsKeyDown(Keys.LeftShift))
                    tooltips.Add(new TooltipLine(Mod, "SetBonusKey", "[c/808080:Hold {SHIFT} for more info]"));
            }

            foreach (TooltipLine tooltip in tooltips)
            {
                string newText = tooltip.Text;
                newText = ConvertBuffWords(newText);

                foreach (string keyword in KeyWords)
                {
                    if (newText.Contains($"<{keyword}>"))
                    {
                        newText = newText.Replace($"<{keyword}>", $"[c/ff79c6:{keyword}]");
                    }
                }

                tooltip.Text = newText;
            }

            base.ModifyTooltips(item, tooltips);
        }
    }
}