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
    public struct ArmorSet
    {
        public readonly int Head;
        public readonly int Body;
        public readonly int Legs;

        public ArmorSet(int Head, int Body, int Legs)
        {
            this.Head = Head;
            this.Body = Body;
            this.Legs = Legs;
        }
    }

    public class TooltipObject
    {
        public int Priority { get; protected set; }
        public Texture2D ObjectIcon { get; protected set; }
    }

    public class SetBonusTooltip : TooltipObject
    {
        public readonly string SetTitle;
        public readonly string SetName;
        public readonly string SetDescription;
        public readonly ArmorSet Set;

        public SetBonusTooltip(Texture2D SetIcon, string SetTitle, string SetName, string SetDescription, ArmorSet Set)
        {
            this.Priority = 1;

            this.ObjectIcon = SetIcon;
            this.SetTitle = SetTitle;
            this.SetName = SetName;
            this.SetDescription = SetDescription;
            this.Set = Set;
        }
    }

    public enum ProjectileTooltipType
    {
        Projectile,
        Minion
    }

    public class ProjectileTooltip : TooltipObject
    {
        public readonly string ProjectileTitle;
        public readonly string ProjectileDescription;
        public readonly float ProjectileDamage;
        public readonly ProjectileTooltipType Type;

        public ProjectileTooltip(Texture2D ProjectileIcon, string ProjectileTitle, string ProjectileDescription, float ProjectileDamage, ProjectileTooltipType Type)
        {
            this.Priority = 2;

            this.ObjectIcon = ProjectileIcon;
            this.ProjectileTitle = ProjectileTitle;
            this.ProjectileDescription = ProjectileDescription;
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

        public readonly string BuffTitle;
        public readonly string BuffDescription;
        public readonly float BuffTime;
        public readonly BuffTooltipType Type;

        public BuffTooltip(Texture2D BuffIcon, string BuffTitle, string BuffDescription, float BuffTime, BuffTooltipType Type)
        {
            this.Priority = 3;

            this.ObjectIcon = BuffIcon;
            this.BuffTitle = BuffTitle;
            this.BuffDescription = BuffDescription;
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
                    new ArmorSet(ItemID.WoodHelmet, ItemID.WoodBreastplate, ItemID.WoodGreaves)));
            }

            if (item.type == ItemID.CowboyHat || item.type == ItemID.CowboyJacket || item.type == ItemID.CowboyPants)
            {
                TooltipObjects.Add(new SetBonusTooltip(ModContent.Request<Texture2D>(AssetDirectory.UI + "Tooltips/WhiteHat").Value,
                    "Wild West Deadeye",
                    "Cowboy Armor",
                    " + Critical hits with [c/FAD5A5:Revolvers] rebound to the nearest enemy",
                    new ArmorSet(ItemID.CowboyHat, ItemID.CowboyJacket, ItemID.CowboyPants)));
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
            // Draw set bonuses, projectiles, and buffs first
            // Afterwards draw any keywords on the left or right side of those tooltips
            float keywordOffset = 0;

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

            string widest = lines.OrderBy(n => ChatManager.GetStringSize(FontAssets.MouseText.Value, n.Text, Vector2.One).X).Last().Text;
            if (orderedTooltips.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                float offset = 0;

                foreach (TooltipObject tooltipObject in TooltipObjects)
                {
                    float MAXIMUM_LENGTH = 330;
                    float containerOffset = ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X;
                    float containerWidth = 350;
                    float dividerWidth = containerWidth - 20;
                    //float width = 400;
                    float height = 14;

                    Vector2 containerPosition = new Vector2(x, y + offset) + new Vector2(containerOffset + 30, 0);

                    Color color = true ? new Color(28, 31, 77) : new Color(25, 27, 27);
                    Color titleColor = true ? new Color(139, 233, 253) : Color.Gray;
                    Color subtitleColor = /*new Color(178, 190, 181)*/new Color(241, 250, 140);
                    Color primaryColor = true ? Color.White : Color.Gray;

                    if (tooltipObject is SetBonusTooltip setBonus)
                    {
                        int dividerOffset = 48;
                        int bottomOffset = 20;

                        Vector2 baseTextSize = new Vector2(1f);
                        Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetTitle, new Vector2(1.25f));

                        // Offset the title and subtitles for the icon
                        Vector2 titleOffset = new Vector2(100, 14);
                        Vector2 subtitleOffset = new Vector2(24, 18);
                        Vector2 setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, baseTextSize) + new Vector2(8, 0);

                        //width = titleSize.X + titleOffset.X + 40; // set the width equal the height plus the offset

                        TextSnippet[] snippets = ChatManager.ParseMessage(setBonus.SetDescription, Color.White).ToArray();

                        int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;
                        /*if (maxSnippetLength > width) // update the width if the description is longer than the height                   
                            width = maxSnippetLength + 40;*/

                        height += titleSize.Y * 2 + bottomOffset; // this is the title/subtitle
                        float titleHeight = height;
                        float unoffsetColoredText = 28 * GetColoredTextCount(setBonus.SetDescription) + (26.6f * GetLineBreakCount(setBonus.SetDescription)); // for wood
      
                        float heightBefore = height;
                        foreach (TextSnippet snippet in snippets)
                        {
                            height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, Vector2.One * 0.95f).Y;
                        }

                        height -= unoffsetColoredText;

                        height += 8; // this is the set bonus name/counter
                        float descriptionHeight = height;

                        height += setBonusTitleLength.Y * 3;
                        height += 50; // final bottom padding

                        float yOverflow = 0;
                        if (y + height > Main.screenHeight) // y-Overflow check
                            yOverflow = y + height - Main.screenHeight;

                        if (Main.MouseScreen.X > Main.screenWidth / 2)
                            containerPosition = new Vector2(x, y) - new Vector2(containerOffset + 10, 0);

                        containerPosition -= new Vector2(0, yOverflow);

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)containerWidth, (int)height), color * 0.925f);

                        #region Title
                        Texture2D texture = setBonus.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, setBonus.SetTitle, containerPosition + titleSize + titleOffset, titleColor, 0f, titleSize, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + dividerOffset, (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Set Bonus
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Set Bonus", new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight), subtitleColor, 0f, titleSize, baseTextSize);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight), (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Set
                        int setCount = 0;

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, setBonus.SetName, new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + descriptionHeight + 36), Color.Orange, 0f, titleSize, baseTextSize);

                        Item headItem = new Item();
                        headItem.SetDefaults(setBonus.Set.Head);
                        Color headColor = Main.LocalPlayer.armor[0].type == headItem.type ? new Color(255, 255, 143) : Color.Gray;
                        if (Main.LocalPlayer.armor[0].type == headItem.type) setCount++;

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, " > " + headItem.Name, new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + descriptionHeight + setBonusTitleLength.Y + 36), headColor, 0f, titleSize, baseTextSize);

                        Item bodyItem = new Item();
                        bodyItem.SetDefaults(setBonus.Set.Body);
                        Color bodyColor = Main.LocalPlayer.armor[1].type == bodyItem.type ? new Color(255, 255, 143) : Color.Gray;
                        if (Main.LocalPlayer.armor[1].type == bodyItem.type) setCount++;

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, " > " + bodyItem.Name, new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + descriptionHeight + (setBonusTitleLength.Y * 2) + 36), bodyColor, 0f, titleSize, baseTextSize);

                        Item legItem = new Item();
                        legItem.SetDefaults(setBonus.Set.Legs);
                        Color legColor = Main.LocalPlayer.armor[2].type == legItem.type ? new Color(255, 255, 143) : Color.Gray;
                        if (Main.LocalPlayer.armor[2].type == legItem.type) setCount++;

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, " > " + legItem.Name, new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + descriptionHeight + (setBonusTitleLength.Y * 3) + 36), legColor, 0f, titleSize, baseTextSize);

                        // The set counter
                        Color setCountColor = setCount == 3 ? Color.Orange : Color.Gray;
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "(" + setCount + "/3)", new Vector2(containerPosition.X + titleSize.X + setBonusTitleLength.X, containerPosition.Y + descriptionHeight + 36), setCountColor, 0f, titleSize, baseTextSize);
                        #endregion

                        if (containerOffset > keywordOffset) keywordOffset = containerOffset;
                    }
                    
                    if (tooltipObject is ProjectileTooltip projectileTooltip)
                    {
                        int dividerOffset = 48;
                        int bottomOffset = 20;

                        Vector2 baseTextSize = new Vector2(1f);
                        Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, projectileTooltip.ProjectileTitle, new Vector2(1.25f));
                        Vector2 damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, "50%", new Vector2(1.25f));

                        // Offset the title and subtitles for the icon
                        Vector2 titleOffset = new Vector2(90, 14);
                        Vector2 subtitleOffset = new Vector2(24, 18);
                        Vector2 setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, "50%", baseTextSize);

                        //width = titleSize.X + titleOffset.X + 70; // set the width equal the height plus the offset

                        TextSnippet[] snippets = ChatManager.ParseMessage(projectileTooltip.ProjectileDescription, Color.White).ToArray();
                        int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;
                        /*if (maxSnippetLength > width) // update the width if the description is longer than the height                   
                            width = maxSnippetLength + 70;*/

                        height += titleSize.Y * 2 + bottomOffset; // this is the title/subtitle
                        float titleHeight = height;
                        float unoffsetColoredText = 28 * GetColoredTextCount(projectileTooltip.ProjectileDescription) + (26.6f * GetLineBreakCount(projectileTooltip.ProjectileDescription));

                        float heightBefore = height;
                        foreach (TextSnippet snippet in snippets)
                        {
                            height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, Vector2.One * 0.95f).Y;
                        }

                        height -= unoffsetColoredText;

                        height += 8; // this is the set bonus name/counter
                        float descriptionHeight = height + 10;

                        height += setBonusTitleLength.Y * 3;
                        height -= 15; // removes extra padding at the bottom, this might be unnecessary?

                        float yOverflow = 0;
                        if (y + height > Main.screenHeight) // y-Overflow check
                            yOverflow = y + height - Main.screenHeight;

                        if (Main.MouseScreen.X > Main.screenWidth / 2)
                            containerPosition = new Vector2(x, y) - new Vector2(containerOffset + 10, 0);

                        containerPosition -= new Vector2(0, yOverflow);

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)containerWidth, (int)height), color * 0.925f);

                        #region Title
                        Texture2D texture = projectileTooltip.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, projectileTooltip.ProjectileTitle, containerPosition + titleSize + titleOffset, titleColor, 0f, titleSize, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + dividerOffset, (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Description
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, projectileTooltip.Type.ToString(), new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight), subtitleColor, 0f, titleSize, baseTextSize);
                        
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight), (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Values
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "50%", new Vector2(containerPosition.X + containerWidth, containerPosition.Y + descriptionHeight + 46), Color.Orange, 0f, damageSize, baseTextSize * 1.25f);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Damage", new Vector2(containerPosition.X + containerWidth, containerPosition.Y + descriptionHeight + 40), Color.Gray, 0f, damageSize, baseTextSize * 0.8f);
                        #endregion

                        if (containerOffset > keywordOffset) keywordOffset = containerOffset;
                    }
                
                    if (tooltipObject is BuffTooltip buffTooltip)
                    {
                        int dividerOffset = 48;
                        int bottomOffset = 20;

                        string damageText = buffTooltip.BuffTime + " seconds";

                        Vector2 baseTextSize = new Vector2(1f);
                        Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, buffTooltip.BuffTitle, new Vector2(1.25f));
                        Vector2 damageSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, damageText, new Vector2(1.25f));

                        // Offset the title and subtitles for the icon
                        Vector2 titleOffset = new Vector2(90, 14);
                        Vector2 subtitleOffset = new Vector2(24, 18);

                        TextSnippet[] snippets = ChatManager.ParseMessage(buffTooltip.BuffDescription, Color.White).ToArray();
                        int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;
    
                        height += titleSize.Y * 2 + bottomOffset; // this is the title/subtitle
                        float titleHeight = height;
                        float unoffsetColoredText = 28 * GetColoredTextCount(buffTooltip.BuffDescription) + (26.6f * GetLineBreakCount(buffTooltip.BuffDescription));

                        float heightBefore = height;
                        foreach (TextSnippet snippet in snippets)
                        {
                            height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, Vector2.One * 0.95f).Y;
                        }

                        height -= unoffsetColoredText;

                        height += 8; // this is the set bonus name/counter
                        float descriptionHeight = height;

                        height += damageSize.Y * 3;
                        height -= 24; // removes extra padding at the bottom, this might be unnecessary?

                        float yOverflow = 0;
                        if (y + height > Main.screenHeight) // y-Overflow check
                            yOverflow = y + height - Main.screenHeight;

                        if (Main.MouseScreen.X > Main.screenWidth / 2)
                            containerPosition = new Vector2(x, y + offset) - new Vector2(containerOffset + 10, 0);

                        containerPosition -= new Vector2(0, yOverflow);

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)containerWidth, (int)height), color * 0.925f);

                        #region Title
                        Texture2D texture = buffTooltip.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, buffTooltip.BuffTitle, containerPosition + new Vector2(texture.Width + 12, 8), new Color(255, 85, 85), 0f, Vector2.Zero, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + dividerOffset, (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Description
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, buffTooltip.Type.ToString(), new Vector2(containerPosition.X, containerPosition.Y + texture.Height + 22), subtitleColor, 0f, Vector2.Zero, baseTextSize);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight), (int)dividerWidth, 2), Color.Black * 0.25f);

                        #region Values
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, damageText, new Vector2(containerPosition.X + containerWidth - damageSize.X - 30, containerPosition.Y + descriptionHeight + 12), Color.Orange, 0f, Vector2.Zero, baseTextSize * 1.25f);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Time", new Vector2(containerPosition.X + containerWidth - damageSize.X - 70, containerPosition.Y + descriptionHeight + 18), Color.Gray, 0f, Vector2.Zero, baseTextSize * 0.8f);
                        #endregion

                        if (containerOffset > keywordOffset) keywordOffset = containerWidth;
                    }

                    offset += height + 5;
                }
            }


            if (KeyWords.Count > 0 && Main.keyState.IsKeyDown(Keys.LeftShift))
            {
                float offset = 0;
                int bottomPadding = 14;

                foreach (string keyWord in KeyWords)
                {
                    Vector2 containerPosition = new Vector2(x, y + offset) + new Vector2(ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X + 38 + keywordOffset, 0);

                    Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, keyWord, new Vector2(1f));
                    float height = titleSize.Y + 4;

                    float titleHeight = height;

                    TextSnippet[] snippets = ChatManager.ParseMessage(GetKeyword(keyWord), Color.White).ToArray();
                    float MAXIMUM_LENGTH = 225;

                    float keywordWidth = 200;

                    foreach (TextSnippet snippet in snippets)
                    {
                        Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, new Vector2(0.95f), MAXIMUM_LENGTH);
                        height += textSize.Y;
                        //if (textSize.X > keywordWidth) keywordWidth = textSize.X;
                    }

                    float yOverflow = 0;
                    if (y + height > Main.screenHeight) // y-Overflow check
                        yOverflow = y + height - Main.screenHeight;

                    if (Main.MouseScreen.X > Main.screenWidth / 2)
                        containerPosition = new Vector2(x, y + offset) - new Vector2(keywordWidth + 58 + keywordOffset, 0);

                    containerPosition -= new Vector2(0, yOverflow);

                    Color color = new Color(28, 31, 77);
                    Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)keywordWidth + 40, (int)height + bottomPadding), color * 0.925f);

                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, keyWord, containerPosition + new Vector2(titleSize.X, 24), new Color(255, 121, 198), 0f, titleSize, new Vector2(1f));
                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight - 8), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);

                    offset += height + 5 + bottomPadding;
                }

                //Main.NewText(string.Join(",", KeyWords));
            }

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            // Probably a better way to do this?
            #region Gun Replacements
            if (item.type == ItemID.PhoenixBlaster)
            {
                tooltips.Add(new TooltipLine(Mod, "PhoenixBlaster0", "<Reload>: Release a burst of flame, inflicting [c/ff5555:[Phoenix Mark][c/ff5555:]]"));
                tooltips.Add(new TooltipLine(Mod, "PhoenixBlaster1", "<Fail>: Damage yourself, inflicting [c/ff5555:[On Fire!][c/ff5555:]]"));
            }

            if (item.type == ItemID.Musket)
            {
                tooltips.Add(new TooltipLine(Mod, "PhoenixBlaster0", "<Reload>: Increase accuracy for each block clicked"));
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