using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI.ClassSelection
{
    public enum ClassButtonKind { Enter, Regenerate, Close }

    public class ClassSelectionButton : UIElement
    {
        public ClassButtonKind kind;
        public Action onClick;

        public string Label => kind switch
        {
            ClassButtonKind.Enter => "Enter",
            ClassButtonKind.Regenerate => "Regenerate",
            _ => "Close"
        };

        public Color BaseColor => kind == ClassButtonKind.Close ? new Color(180, 60, 60) : new Color(60, 140, 200);

        public override void LeftClick(UIMouseEvent evt)
        {
            onClick?.Invoke();
            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);
            bool hovering = IsMouseHovering;
            if (hovering) Main.LocalPlayer.mouseInterface = true;

            Color fill = hovering ? Color.Lerp(BaseColor, Color.White, 0.3f) : BaseColor;
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, rect, fill);
            DrawBorder(spriteBatch, rect, new Color(20, 20, 20), 1);

            var font = FontAssets.MouseText.Value;
            Vector2 size = ChatManager.GetStringSize(font, Label, Vector2.One);
            Vector2 pos = new Vector2(rect.X + rect.Width / 2f - size.X / 2f, rect.Y + rect.Height / 2f - size.Y / 2f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, Label, pos, Color.White, 0f, Vector2.Zero, Vector2.One);
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
