using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace OvermorrowMod.Content.UI
{
    public class BookUI : UIState
    {
        public static bool Open = false;
        public static bool Dragging = false;
        public static bool Visible => Main.playerInventory && Main.LocalPlayer.chest == -1 && Main.npcShop == 0;

        private QuestLog Back = new QuestLog();
        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));
        private UIImageButton BookButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));
        public override void OnInitialize()
        {
            ModUtils.AddElement(BookButton, 570, 274, 34, 38, this);
            BookButton.OnClick += BookClicked;
            BookButton.SetVisibility(1, 1);

            //ModUtils.AddElement(Back, Main.screenWidth / 2, Main.screenHeight / 2, 500, 500, this);
            //
            //ModUtils.AddElement(ExitButton, 454, 4, 32, 32, Back);
            //ExitButton.OnClick += Exit;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Visible)
            {
                BookButton.Draw(spriteBatch);
                BookButton.SetVisibility(1, 1);

                if (BookButton.IsMouseHovering)
                {
                    Texture2D outline = ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Outline").Value;
                    spriteBatch.Draw(outline, BookButton.GetDimensions().Position() + new Vector2(-2, -2), Color.White);

                    Utils.DrawBorderString(spriteBatch, "Journal", Main.MouseScreen + Vector2.One * 16, Main.MouseTextColorReal, 1f);
                }
            }

            if (Open) base.Draw(spriteBatch);
        }

        private void BookClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            var state = ModContent.GetInstance<OvermorrowModSystem>().QuestLog;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);

            //Open = true;
        }

        private void Exit(UIMouseEvent evt, UIElement listeningElement)
        {
            Open = false;
        }

        public override void Update(GameTime gameTime)
        {
            BookButton.SetImage(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook"));

            if (!Main.mouseLeft) Dragging = false;

            Recalculate();
            base.Update(gameTime);
        }
    }

    /*internal class QuestLog : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White);
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            base.Draw(spriteBatch);
        }
    }*/

    internal class QuestLog : UIState
    {
        private BookPanel Panel = new BookPanel();
        private UIPanel panel = new UIPanel();
        private UIImageButton ExitButton = new UIImageButton(ModContent.Request<Texture2D>(AssetDirectory.UI + "QuestBook_Exit"));

        public override void OnInitialize()
        {
            ModUtils.AddElement(Panel, Main.screenWidth / 2, Main.screenHeight / 2, 750, 500, this);
            ModUtils.AddElement(ExitButton, 454, 4, 32, 32, Panel);
            ExitButton.OnClick += Exit;
        }

        private void Exit(UIMouseEvent evt, UIElement listeningElement)
        {
            var state = ModContent.GetInstance<OvermorrowModSystem>().BookUI;
            ModContent.GetInstance<OvermorrowModSystem>().BookInterface?.SetState(state);

            //ModContent.GetInstance<OvermorrowModSystem>().HideMyUI();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(TextureAssets.MagicPixel.Value, GetDimensions().ToRectangle(), TextureAssets.MagicPixel.Value.Frame(), Color.White);
            //Vector2 pos = GetDimensions().ToRectangle().TopLeft();

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
    }
}