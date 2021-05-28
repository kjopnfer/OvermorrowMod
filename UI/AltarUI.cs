using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using System.Drawing.Drawing2D;

namespace OvermorrowMod.UI
{
    class AltarUI : UIState
    {
        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(Main.screenWidth / 2, 0);
            panel.Height.Set(Main.screenHeight / 2, 0);
            panel.BackgroundColor = Color.Black;
            panel.HAlign = panel.VAlign = 0.5f;
            Append(panel);

            UIText header = new UIText("hhhhhhhhhhhh");
            header.HAlign = 0.5f; // 1
            header.Top.Set(2, 0); // 2
            panel.Append(header);

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Texture2D background = ModContent.GetTexture("OvermorrowMod/UI/SoulBG");
            // 47 - > 40 - > 54 -> 72
            spriteBatch.Draw(background, new Vector2(Main.screenWidth / 2 - 69, Main.screenHeight / 2 + 20), Color.White);

        }
    }
}