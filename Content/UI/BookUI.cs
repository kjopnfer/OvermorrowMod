using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    public class BookUI : UIState
    {
        public static bool Visible => Main.playerInventory && Main.LocalPlayer.chest == -1 && Main.npcShop == 0;

        private UIImageButton QuestBook = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));
        public override void OnInitialize()
        {
            ModUtils.AddElement(QuestBook, 570, 274, 34, 38, this);

            base.OnInitialize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                QuestBook.Draw(spriteBatch);
                QuestBook.SetVisibility(1, 1);

                if (QuestBook.IsMouseHovering)
                {
                    Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Outline").Value;
                    spriteBatch.Draw(outline, QuestBook.GetDimensions().Position() + new Vector2(-2, -2), Color.White);

                    Utils.DrawBorderString(spriteBatch, "ahh", Main.MouseScreen + Vector2.One * 16, Main.MouseTextColorReal, 1f);
                }
            }

            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            QuestBook.SetImage(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));
            base.Update(gameTime);
        }
    }
}