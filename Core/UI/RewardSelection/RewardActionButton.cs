using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Loot;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace OvermorrowMod.Core.UI
{
    public enum RewardButtonType { Confirm, Deny, Reroll }

    public class RewardActionButton : UIElement
    {
        public RewardButtonType type;
        public float animOpacity = 0f;

        private Color BaseColor => type switch
        {
            RewardButtonType.Confirm => new Color(60, 180, 60),
            RewardButtonType.Deny => new Color(180, 60, 60),
            _ => new Color(60, 120, 190),
        };

        private Color DisabledColor => type switch
        {
            RewardButtonType.Confirm => new Color(80, 100, 80),
            RewardButtonType.Deny => new Color(120, 80, 80),
            _ => new Color(80, 90, 110),
        };

        private static int Rerolls => Main.LocalPlayer.GetModPlayer<RerollPlayer>().Rerolls;

        public bool IsEnabled
        {
            get
            {
                if (Parent is not RewardSelection s || !s.AcceptsInput) return false;
                return type switch
                {
                    RewardButtonType.Confirm => s.selectedIndex >= 0,
                    RewardButtonType.Reroll => Rerolls > 0,
                    _ => true,
                };
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (animOpacity < 1f) return;
            if (!IsEnabled) return;
            if (Parent is RewardSelection state)
                state.HandleAction(type);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            if (animOpacity < 1f) return;
            if (!IsEnabled) return;

            SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            float opacity = MathHelper.Clamp(animOpacity, 0f, 1f);
            if (opacity <= 0f)
                return;

            var dims = GetDimensions();
            Rectangle rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);

            bool enabled = IsEnabled;
            bool isHovering = IsMouseHovering && opacity >= 1f && enabled;

            Color fill = enabled ? BaseColor : DisabledColor;
            if (isHovering) fill = Color.Lerp(fill, Color.White, 0.3f);

            // Block world clicks under the button.
            if (isHovering)
                Main.LocalPlayer.mouseInterface = true;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, rect, fill * opacity);
            DrawBorder(spriteBatch, rect, new Color(20, 20, 20) * opacity, 1);

            if (type == RewardButtonType.Reroll)
                DrawRerollCount(spriteBatch, rect, opacity);
        }

        private void DrawRerollCount(SpriteBatch spriteBatch, Rectangle rect, float opacity)
        {
            int rerolls = Rerolls;
            var font = FontAssets.ItemStack.Value;
            string text = rerolls.ToString();
            Vector2 size = ChatManager.GetStringSize(font, text, Vector2.One);
            Vector2 position = new Vector2(rect.Right - size.X - 2f, rect.Bottom - size.Y + 2f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, text, position, Color.White * opacity, 0f, Vector2.Zero, Vector2.One);

            if (IsMouseHovering && opacity >= 1f)
                Main.hoverItemName = Language.GetTextValue(LocalizationPath.UI + "RewardSelection.Reroll", rerolls);
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
