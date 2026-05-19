using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace OvermorrowMod.Core.UI
{
    public enum RewardButtonType { Confirm, Deny }

    public class RewardActionButton : UIElement
    {
        public RewardButtonType type;
        public float animOpacity = 0f;

        private Color BaseColor => type == RewardButtonType.Confirm ? new Color(60, 180, 60) : new Color(180, 60, 60);
        private Color DisabledColor => type == RewardButtonType.Confirm ? new Color(80, 100, 80) : new Color(120, 80, 80);

        public bool IsEnabled
        {
            get
            {
                if (type == RewardButtonType.Deny) return true;
                return Parent is RewardSelection s && s.selectedIndex >= 0;
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
