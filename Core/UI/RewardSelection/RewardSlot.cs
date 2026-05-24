using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Core.UI
{
    public class RewardSlot : UIElement
    {
        public int slotIndex;
        public Item displayItem = new Item();
        public bool selected = false;
        public float animOpacity = 0f;

        public void SetItem(int itemId)
        {
            displayItem = new Item();
            if (itemId > 0) displayItem.SetDefaults(itemId);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (animOpacity < 1f) return;
            if (Parent is RewardSelection state)
                state.SelectSlot(slotIndex);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);

            if (animOpacity >= 1f && !selected && !displayItem.IsAir)
                SoundEngine.PlaySound(SoundID.MenuTick);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var dims = GetDimensions();
            Rectangle rect = new Rectangle((int)dims.X, (int)dims.Y, (int)dims.Width, (int)dims.Height);

            float opacity = MathHelper.Clamp(animOpacity, 0f, 1f);
            if (opacity <= 0f) 
                return;

            Texture2D pixel = TextureAssets.MagicPixel.Value;
            Texture2D glowTex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05", AssetRequestMode.ImmediateLoad).Value;

            bool isHovering = IsMouseHovering && opacity >= 1f && !selected && !displayItem.IsAir;

            float pulse = 0.7f + 0.3f * (float)Math.Sin(Main.GameUpdateCount * 0.06f + slotIndex * 0.7f);
            float haloAlpha = (selected ? 0.95f : isHovering ? 0.7f : 0.5f) * opacity * pulse;
            float haloScale = rect.Width * (selected ? 2.1f : isHovering ? 1.85f : 1.6f) / glowTex.Width;
            spriteBatch.Draw(glowTex, dims.Center(), null, Color.Gold * haloAlpha, 0f, glowTex.Size() / 2f, haloScale, SpriteEffects.None, 0f);

            Color backdropColor = selected ? new Color(70, 55, 25) : isHovering ? new Color(55, 50, 30) : new Color(40, 40, 40);
            spriteBatch.Draw(pixel, rect, backdropColor * opacity * 0.9f);

            Color borderColor = selected ? Color.Gold : isHovering ? new Color(220, 200, 130) : new Color(160, 140, 90);
            int thickness = selected ? 2 : 1;
            DrawBorder(spriteBatch, rect, borderColor * opacity, thickness);

            // Item icon
            if (!displayItem.IsAir)
            {
                Main.instance.LoadItem(displayItem.type);
                Texture2D itemTex = TextureAssets.Item[displayItem.type].Value;
                Rectangle frame = Main.itemAnimations[displayItem.type] != null ? Main.itemAnimations[displayItem.type].GetFrame(itemTex) : itemTex.Frame();
                float maxDim = Math.Max(frame.Width, frame.Height);
                float scale = maxDim > 32f ? 32f / maxDim : 1f;
                Vector2 origin = frame.Size() / 2f;
                spriteBatch.Draw(itemTex, dims.Center(), frame, Color.White * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }

            if (opacity >= 1f && !displayItem.IsAir && ContainsPoint(Main.MouseScreen))
            {
                Main.HoverItem = displayItem.Clone();
                Main.hoverItemName = displayItem.Name;
                Main.LocalPlayer.mouseInterface = true;
            }
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
