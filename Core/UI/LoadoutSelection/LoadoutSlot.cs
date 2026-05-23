using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.LoadoutSelection
{
    public enum LoadoutSlotKind { Weapon, Misc }

    public class LoadoutSlot : UIElement
    {
        public LoadoutSlotKind kind;
        public Item storedItem = new Item();

        public string errorMessage;
        public int errorTimer;
        private const int ErrorDuration = 150;

        public bool Validate(Item item, out string error)
        {
            error = null;
            if (item == null || item.IsAir) { error = "No item"; return false; }
            if (kind == LoadoutSlotKind.Weapon)
            {
                if (item.damage <= 0) { error = "Not a weapon"; return false; }
                if (item.rare > ItemRarityID.Blue) { error = "Rarity too high"; return false; }
                return true;
            }
            if (item.damage > 0) { error = "No weapons in misc"; return false; }
            return true;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (!Main.mouseItem.IsAir)
            {
                if (Validate(Main.mouseItem, out string err))
                {
                    if (!storedItem.IsAir && storedItem.type == Main.mouseItem.type)
                    {
                        int room = Math.Max(0, storedItem.maxStack - storedItem.stack);
                        int add = Math.Min(room, Main.mouseItem.stack);
                        if (add > 0) storedItem.stack += add;
                    }
                    else
                    {
                        storedItem = Main.mouseItem.Clone();
                    }
                    errorMessage = null;
                    errorTimer = 0;
                    SoundEngine.PlaySound(SoundID.Grab);
                }
                else
                {
                    errorMessage = err;
                    errorTimer = ErrorDuration;
                    SoundEngine.PlaySound(SoundID.MenuClose);
                }
            }
            else if (!storedItem.IsAir)
            {
                storedItem = new Item();
                errorMessage = null;
                errorTimer = 0;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (errorTimer > 0) errorTimer--;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);
            bool hovering = IsMouseHovering;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Color bg = hovering ? new Color(55, 50, 30) : new Color(40, 40, 40);
            spriteBatch.Draw(pixel, rect, bg);

            Color border = !storedItem.IsAir ? GetRarityColor(storedItem.rare) : (hovering ? new Color(220, 200, 130) : new Color(160, 140, 90));
            int thickness = !storedItem.IsAir ? 2 : 1;
            DrawBorder(spriteBatch, rect, border, thickness);

            if (!storedItem.IsAir)
            {
                Main.instance.LoadItem(storedItem.type);
                Texture2D itemTex = TextureAssets.Item[storedItem.type].Value;
                Rectangle frame = Main.itemAnimations[storedItem.type] != null ? Main.itemAnimations[storedItem.type].GetFrame(itemTex) : itemTex.Frame();
                float maxDim = Math.Max(frame.Width, frame.Height);
                float scale = maxDim > 32f ? 32f / maxDim : 1f;
                spriteBatch.Draw(itemTex, dims.Center(), frame, Color.White, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

                if (storedItem.stack > 1)
                {
                    var stackFont = FontAssets.ItemStack.Value;
                    string stackText = storedItem.stack.ToString();
                    Vector2 stackPos = new Vector2(rect.X + 6f, rect.Bottom - 18f);
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, stackFont, stackText, stackPos, Color.White, 0f, Vector2.Zero, new Vector2(0.75f));
                }
            }

            if (errorTimer > 0 && !string.IsNullOrEmpty(errorMessage))
            {
                float alpha = MathHelper.Clamp(errorTimer / (float)ErrorDuration, 0f, 1f);
                var font = FontAssets.MouseText.Value;
                Vector2 size = ChatManager.GetStringSize(font, errorMessage, Vector2.One);
                Vector2 pos = new Vector2(rect.X + rect.Width / 2f - size.X / 2f, rect.Bottom + 6f);
                ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, errorMessage, pos, new Color(220, 80, 80) * alpha, 0f, Vector2.Zero, Vector2.One);
            }

            if (hovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (!storedItem.IsAir)
                {
                    Main.HoverItem = storedItem.Clone();
                    Main.hoverItemName = storedItem.Name;
                }
            }
        }

        private static Color GetRarityColor(int rare)
        {
            return rare switch
            {
                ItemRarityID.Gray => new Color(130, 130, 130),
                ItemRarityID.White => Color.White,
                ItemRarityID.Blue => new Color(150, 150, 255),
                ItemRarityID.Green => new Color(150, 255, 150),
                ItemRarityID.Orange => new Color(255, 200, 150),
                ItemRarityID.LightRed => new Color(255, 150, 150),
                ItemRarityID.Pink => new Color(255, 150, 150),
                ItemRarityID.LightPurple => new Color(210, 160, 255),
                ItemRarityID.Lime => new Color(150, 255, 10),
                ItemRarityID.Yellow => new Color(255, 255, 10),
                ItemRarityID.Cyan => new Color(5, 200, 255),
                ItemRarityID.Red => new Color(255, 40, 100),
                ItemRarityID.Purple => new Color(180, 40, 255),
                _ => new Color(160, 140, 90)
            };
        }

        private static void DrawBorder(SpriteBatch sb, Rectangle r, Color c, int t)
        {
            Texture2D px = TextureAssets.MagicPixel.Value;
            sb.Draw(px, new Rectangle(r.X, r.Y, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Bottom - t, r.Width, t), c);
            sb.Draw(px, new Rectangle(r.X, r.Y, t, r.Height), c);
            sb.Draw(px, new Rectangle(r.Right - t, r.Y, t, r.Height), c);
        }
    }
}
