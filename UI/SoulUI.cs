using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using static Terraria.ModLoader.ModContent;
using System.Drawing.Drawing2D;
using WardenClass;

namespace OvermorrowMod.UI
{
    class SoulUI : UIState
    {
        private int soulCount;
        private UIText showSouls;

        public override void OnInitialize()
        {
            UIElement panel = new UIElement();
            panel.Width.Set(Main.screenWidth, 0);
            panel.Height.Set(Main.screenHeight, 0);
            panel.HAlign = panel.VAlign = 0.5f;
            Append(panel);

            if (!Main.gameMenu)
            {
                soulCount = Main.LocalPlayer.GetModPlayer<WardenDamagePlayer>().soulResourceCurrent;
            }

            showSouls = new UIText(soulCount.ToString());
            showSouls.HAlign = 0.499f;
            showSouls.VAlign = 0.57f;
            panel.Append(showSouls);
        }

        public override void Update(GameTime gameTime)
        {
            soulCount = Main.LocalPlayer.GetModPlayer<WardenDamagePlayer>().soulResourceCurrent;
            showSouls.SetText(soulCount.ToString());
            base.Update(gameTime);
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