using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            this.ObjectIcon = SetIcon;
            this.SetTitle = SetTitle;
            this.SetName = SetName;
            this.SetDescription = SetDescription;
            this.Set = Set;
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
                    " + Increased defense by 1\n + Increased damage by 5\n + 5% chance to instantly kill all enemies",
                    new ArmorSet(ItemID.WoodHelmet, ItemID.WoodBreastplate, ItemID.WoodGreaves)));
            }

            if (item.type == ItemID.CowboyHat || item.type == ItemID.CowboyJacket || item.type == ItemID.CowboyPants)
            {
                TooltipObjects.Add(new SetBonusTooltip(ModContent.Request<Texture2D>("OvermorrowMod/Assets/Unused/Buffs/Test").Value,
                    "Wild West Deadeye",
                    "Cowboy Armor",
                    " + Critical hits with [c/BF40BF:Revolvers] rebound to the nearest enemy",
                    new ArmorSet(ItemID.CowboyHat, ItemID.CowboyJacket, ItemID.CowboyPants)));
            }

            base.SetDefaults(item);
        }

        private int GetColoredTextCount(string description)
        {
            string[] splitText = description.Split('[');

            return splitText.Length - 1;
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            // Main.keyState.IsKeyDown(Keys.LeftShift)

            if (TooltipObjects.Count > 0)
            {
                float height = 14;
                string widest = lines.OrderBy(n => ChatManager.GetStringSize(FontAssets.MouseText.Value, n.Text, Vector2.One).X).Last().Text;
                float width = ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X;

                Vector2 containerPosition = new Vector2(x, y) + new Vector2(width + 30, 0);

                Color color = true ? new Color(28, 31, 77) : new Color(25, 27, 27);
                Color titleColor = true ? Color.YellowGreen : Color.Gray;
                Color subtitleColor = /*new Color(178, 190, 181)*/new Color(52, 201, 235);
                Color primaryColor = true ? Color.White : Color.Gray;

                foreach (TooltipObject tooltipObject in TooltipObjects)
                {
                    if (tooltipObject is SetBonusTooltip setBonus)
                    {
                        int dividerOffset = 48;
                        int bottomOffset = 20;

                        Vector2 baseTextSize = new Vector2(1f);
                        Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetTitle, new Vector2(1.25f));

                        // Offset the title and subtitles for the icon
                        Vector2 titleOffset = new Vector2(90, 14); 
                        Vector2 subtitleOffset = new Vector2(24, 18);
                        Vector2 setBonusTitleLength = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, baseTextSize) + new Vector2(8, 0);

                        width = titleSize.X + titleOffset.X + 40; // set the width equal the height plus the offset

                        float MAXIMUM_LENGTH = 300;
                        TextSnippet[] snippets = ChatManager.ParseMessage(setBonus.SetDescription, Color.White).ToArray();

                        int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;
                        if (maxSnippetLength > width) // update the width if the description is longer than the height                   
                            width = maxSnippetLength + 40;
                        

                        height += titleSize.Y * 2 + bottomOffset; // this is the title/subtitle
                        float titleHeight = height;

                        float unoffsetColoredText = 16 * GetColoredTextCount(setBonus.SetDescription);
                        foreach (TextSnippet snippet in snippets)
                        {
                            height += ChatManager.GetStringSize(FontAssets.MouseText.Value, snippet.Text, Vector2.One * 0.95f).Y;
                            height -= unoffsetColoredText;
                        }

                        height += 30; // this is the set bonus name/counter
                        float descriptionHeight = height;

                        height += setBonusTitleLength.Y * 3;
                        height += 50; // final bottom padding

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width, (int)height), color * 0.925f);

                        #region Title
                        Texture2D texture = setBonus.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, setBonus.SetTitle, containerPosition + titleSize + titleOffset, titleColor, 0f, titleSize, new Vector2(1.25f));
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + dividerOffset, (int)width - 18, 2), Color.Black * 0.25f);

                        #region Set Bonus
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Set Bonus", new Vector2(containerPosition.X + titleSize.X, containerPosition.Y + titleHeight), subtitleColor, 0f, titleSize, baseTextSize);

                        Vector2 descriptionOffset = new Vector2(0, titleOffset.Y + subtitleOffset.Y + dividerOffset);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, new Vector2(containerPosition.X, containerPosition.Y + titleHeight), 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);
                        #endregion

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)(containerPosition.Y + descriptionHeight), (int)width - 18, 2), Color.Black * 0.25f);

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
                    }
                }
            }
            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}