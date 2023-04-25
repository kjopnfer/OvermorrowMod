using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Players;
using OvermorrowMod.Content.Items.Consumable;
using OvermorrowMod.Core;
using System;
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
    public partial class OvermorrowGlobalItem : GlobalItem
    {
        public override bool PreDrawTooltip(Item item, ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y)
        {
            ReLogic.Graphics.DynamicSpriteFont font = FontAssets.MouseText.Value;
            //float height = -16;
            float height = 190;

            if (item.type == ItemID.WoodHelmet /*&& Main.keyState.IsKeyDown(Keys.LeftShift)*/)
            {
                string widest = lines.OrderBy(n => ChatManager.GetStringSize(font, n.Text, Vector2.One).X).Last().Text;
                float width = ChatManager.GetStringSize(font, widest, Vector2.One).X;

                Vector2 containerPosition = new Vector2(x, y) + new Vector2(width + 30, 0);

                int tempWidth = 0;

                Color color = true ? new Color(38, 72, 89) : new Color(25, 27, 27);

                Color titleColor = true ? Color.YellowGreen : Color.Gray;
                Color primaryColor = true ? Color.White : Color.Gray;
                Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), color * 0.925f);
                Texture2D texture = ModContent.Request<Texture2D>("OvermorrowMod/Assets/Unused/Buffs/VineRune").Value;

                Vector2 temp = new Vector2(2, 5);
                Main.spriteBatch.Draw(texture, temp + containerPosition + texture.Size() / 2f, null, primaryColor, 0f, texture.Size() / 2f, 1.25f, SpriteEffects.None, 0f);
                Utils.DrawBorderString(Main.spriteBatch, "Wooden Warrior", containerPosition + new Vector2(48, 0), titleColor);
                Utils.DrawBorderString(Main.spriteBatch, "Set Bonus", containerPosition + new Vector2(48, 24), new Color(178, 190, 181));

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + 48, (int)width + 78, 2), Color.Black * 0.5f);

                //Main.spriteBatch.Draw(TextureAssets.MagicPixel, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), Color.Red);
                //Utils.DrawLine(Main.spriteBatch, Main.screenPosition / 2f, Main.screenPosition / 2f + new Vector2(48, 0), Color.Red, Color.Red, 5);
                Utils.DrawBorderString(Main.spriteBatch, "Increased defense by 1", containerPosition + new Vector2(0, 60), primaryColor);

                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)containerPosition.X, (int)containerPosition.Y + 88, (int)width + 78, 2), Color.Black * 0.5f);

                Utils.DrawBorderString(Main.spriteBatch, "Cowboy Armor (2/3)", containerPosition + new Vector2(0, 96), Color.YellowGreen);
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Hat", containerPosition + new Vector2(0, 120), Color.LightGreen);
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Jacket", containerPosition + new Vector2(0, 120 + 24), Color.LightGreen);
                Utils.DrawBorderString(Main.spriteBatch, "- Cowboy Pants", containerPosition + new Vector2(0, 120 + 48), Color.Gray);

                //Utils.DrawInvBG(Main.spriteBatch, new Rectangle((int)containerPosition.X - 10, (int)containerPosition.Y - 10, (int)width + 96 + tempWidth, (int)height + 20), new Color(25, 27, 27) * 0.5f);
            }

            return base.PreDrawTooltip(item, lines, ref x, ref y);
        }
    }
}