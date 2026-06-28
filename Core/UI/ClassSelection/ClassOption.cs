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
    public class ClassOption : UIElement
    {
        public CharacterClass characterClass;
        public bool selected;

        public override void LeftClick(UIMouseEvent evt)
        {
            if (Parent is ClassSelection state)
            {
                state.SelectClass(characterClass);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);
            bool hovering = IsMouseHovering;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Color bg = selected ? new Color(55, 50, 30) : (hovering ? new Color(45, 45, 50) : new Color(35, 35, 40));
            spriteBatch.Draw(pixel, rect, bg);

            Color border = selected ? new Color(220, 200, 130) : (hovering ? new Color(180, 160, 110) : new Color(110, 100, 70));
            DrawBorder(spriteBatch, rect, border, selected ? 2 : 1);

            int mainType = ClassSelection.MainWeaponType(characterClass);
            Main.instance.LoadItem(mainType);
            Texture2D itemTex = TextureAssets.Item[mainType].Value;
            Rectangle frame = Main.itemAnimations[mainType] != null ? Main.itemAnimations[mainType].GetFrame(itemTex) : itemTex.Frame();
            float maxDim = Math.Max(frame.Width, frame.Height);
            float scale = maxDim > 40f ? 40f / maxDim : 1f;
            Vector2 iconCenter = new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f - 8f);
            spriteBatch.Draw(itemTex, iconCenter, frame, Color.White, 0f, frame.Size() / 2f, scale, SpriteEffects.None, 0f);

            var font = FontAssets.MouseText.Value;
            string label = characterClass.ToString();
            Vector2 size = ChatManager.GetStringSize(font, label, Vector2.One) * 0.85f;
            Vector2 pos = new Vector2(rect.X + rect.Width / 2f - size.X / 2f, rect.Bottom - size.Y - 6f);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, font, label, pos, selected ? new Color(255, 235, 170) : Color.White, 0f, Vector2.Zero, new Vector2(0.85f));

            if (hovering) Main.LocalPlayer.mouseInterface = true;
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
