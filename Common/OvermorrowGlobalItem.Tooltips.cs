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
        public readonly string SetName;
        public readonly string SetDescription;
        public readonly ArmorSet Set;

        public SetBonusTooltip(Texture2D SetIcon, string SetName, string SetDescription, ArmorSet Set)
        {
            this.ObjectIcon = SetIcon;
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
                TooltipObjects.Add(new SetBonusTooltip(ModContent.Request<Texture2D>("OvermorrowMod/Assets/Unused/Buffs/VineRune").Value,
                    "Wooden Warrior",
                    "Increased defense by 1\nIncreased damage by 5\n5% chance to instantly kill all enemies",
                    new ArmorSet(ItemID.WoodHelmet, ItemID.WoodBreastplate, ItemID.WoodGreaves)));
            }

            base.SetDefaults(item);
        }

        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            // Main.keyState.IsKeyDown(Keys.LeftShift)

            float height = 14;
            //float height = 190;

            if (TooltipObjects.Count > 0)
            {
                string widest = lines.OrderBy(n => ChatManager.GetStringSize(FontAssets.MouseText.Value, n.Text, Vector2.One).X).Last().Text;
                float width = ChatManager.GetStringSize(FontAssets.MouseText.Value, widest, Vector2.One).X;

                Vector2 containerPosition = new Vector2(x, y) + new Vector2(width + 30, 0);

                Color color = true ? new Color(28, 31, 77) : new Color(25, 27, 27);
                Color titleColor = true ? Color.YellowGreen : Color.Gray;
                Color subtitleColor = new Color(178, 190, 181);
                Color primaryColor = true ? Color.White : Color.Gray;

                foreach (TooltipObject tooltipObject in TooltipObjects)
                {
                    if (tooltipObject is SetBonusTooltip setBonus)
                    {
                        int dividerOffset = 48;
                        int bottomOffset = 24;

                        Vector2 textSize = new Vector2(1f);
                        Vector2 titleSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, setBonus.SetName, textSize);

                        // Offset the title and subtitles for the icon
                        Vector2 titleOffset = new Vector2(48, 0); 
                        Vector2 subtitleOffset = new Vector2(24, 18);

                        width = titleSize.X + titleOffset.X + 22;

                        float MAXIMUM_LENGTH = 300;
                        TextSnippet[] snippets = ChatManager.ParseMessage(setBonus.SetDescription, Color.White).ToArray();

                        int maxSnippetLength = (int)ChatManager.GetStringSize(FontAssets.MouseText.Value, snippets[0].Text, Vector2.One * 0.95f, MAXIMUM_LENGTH).X;
                        if (maxSnippetLength > width)
                        {
                            width = maxSnippetLength + 22;
                        }

                        //Main.NewText(maxSnippetLength + " / " + width);

                        height += titleSize.Y * 2 + bottomOffset;
                        height += snippets.Length * 24 + dividerOffset + 24;

                        Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width, (int)height), color * 0.925f);

                        Texture2D texture = setBonus.ObjectIcon;
                        Main.spriteBatch.Draw(texture, containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1f, SpriteEffects.None, 0f);

                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, setBonus.SetName, containerPosition + titleSize + titleOffset, titleColor, 0f, titleSize, textSize);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, "Set Bonus", containerPosition + titleSize + subtitleOffset, subtitleColor, 0f, titleSize, textSize * 0.8f);

                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + dividerOffset, (int)width - 18, 2), Color.Black * 0.25f);

                        Vector2 descriptionOffset = new Vector2(0, titleOffset.Y + subtitleOffset.Y + dividerOffset);
                        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.MouseText.Value, snippets, containerPosition + descriptionOffset, 0f, Color.White, Vector2.Zero, Vector2.One * 0.95f, out _, MAXIMUM_LENGTH);

                        int secondDividerPosition = (int)(containerPosition.Y + descriptionOffset.Y) + snippets.Length * 24 + dividerOffset + 20;
                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, secondDividerPosition, (int)width - 18, 2), Color.Black * 0.25f);

                        //ChatManager.DrawColorCodedString(Main.spriteBatch, FontAssets.MouseText.Value, snippets, containerPosition + descriptionOffset, Color.White, 0f, Vector2.Zero, Vector2.One * 0.9f, out var hoveredSnippet, MAXIMUM_LENGTH);
                    }
                    //ChatManager.draw
                }
            }

            /*if (item.type == ItemID.WoodHelmet)
            {
                string widest = lines.OrderBy(n => ChatManager.GetStringSize(font, n.Text, Vector2.One).X).Last().Text;
                float width = ChatManager.GetStringSize(font, widest, Vector2.One).X;

                Vector2 containerPosition = new Vector2(x, y) + new Vector2(width + 30, 0);

                int tempWidth = 0;

                Color color = true ? new Color(28, 31, 77) : new Color(25, 27, 27);

                Color titleColor = true ? Color.YellowGreen : Color.Gray;
                Color primaryColor = true ? Color.White : Color.Gray;
                Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), color * 0.925f);
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Unused/Buffs/VineRune").Value;

                Vector2 temp = new Vector2(2, 5);
                Main.spriteBatch.Draw(texture, temp + containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1.25f, SpriteEffects.None, 0f);
                Utils.DrawBorderString(Main.spriteBatch, "Wooden Warrior", containerPosition + new Vector2(48, 0), titleColor);
                Utils.DrawBorderString(Main.spriteBatch, "Set Bonus", containerPosition + new Vector2(48, 24), new Color(178, 190, 181));

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + 48, (int)width + 78, 2), Color.Black * 0.25f);

                //Main.spriteBatch.Draw(TextureAssets.MagicPixel, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), Color.Red);
                //Utils.DrawLine(Main.spriteBatch, Main.screenPosition / 2f, Main.screenPosition / 2f + new Vector2(48, 0), Color.Red, Color.Red, 5);
                Utils.DrawBorderString(Main.spriteBatch, "Increased defense by 1", containerPosition + new Vector2(0, 60), primaryColor);

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + 88, (int)width + 78, 2), Color.Black * 0.25f);

                Utils.DrawBorderString(Main.spriteBatch, "Cowboy Armor (2/3)", containerPosition + new Vector2(0, 96), Color.Orange);
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Hat", containerPosition + new Vector2(0, 120), new Color(255, 255, 143));
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Jacket", containerPosition + new Vector2(0, 120 + 24), new Color(255, 255, 143));
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Pants", containerPosition + new Vector2(0, 120 + 48), Color.Gray);

                //Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), new Color(25, 27, 27) * 0.5f);
            }*/

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}