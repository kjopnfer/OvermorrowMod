using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using ReLogic.Graphics;
using ReLogic.Content;
using OvermorrowMod.Core;
using Terraria.UI.Chat;
using OvermorrowMod.Content.UI.AmmoSwap;
using OvermorrowMod.Common.Cutscenes;
using Terraria.Audio;

namespace OvermorrowMod.Content.UI.ReadableBook
{
    public class UIBookPanel : UIPanel
    {
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BookBack").Value;
            spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1f, 0, 0);
        }
    }

    public class UIReadableBookState : UIState
    {
        private UIBookPanel drawSpace = new UIBookPanel();

        public virtual int PageNum => 1;
        public virtual Asset<Texture2D> PageTexture => ModContent.Request<Texture2D>(AssetDirectory.UI + "BookBack");

        public virtual Color TextColor => new Color(50, 50, 50);

        internal UIBookCloseButton closeButton = new UIBookCloseButton();

        internal int pageIndex = 0;

        public virtual BlendState BGBlendState => BlendState.AlphaBlend;

        public bool showBook = false;
        public static bool AnyShowing = false;

        private UIBook currentBook;
        public void ShowBook(UIBook currentBook)
        {
            this.currentBook = currentBook;

            showBook = true;
            pageIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (!showBook) return;

            //if (!AnyShowing) Showing = false;
            //ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - WIDTH, Main.screenHeight / 2 - HEIGHT, WIDTH * 2, HEIGHT * 2, this);
            this.RemoveAllChildren();
            ModUtils.AddElement(drawSpace, Main.screenWidth / 2 - 375, Main.screenHeight / 2 - 250, 750, 500, this);

            drawSpace.RemoveAllChildren();
            ModUtils.AddElement(closeButton, 250, 250, 22, 22, drawSpace);

            foreach (var pageElement in currentBook.bookPages[pageIndex].pageElements)
            {
                ModUtils.AddElement(pageElement, pageElement.drawRectangle.X, pageElement.drawRectangle.Y, pageElement.drawRectangle.Width, pageElement.drawRectangle.Height, drawSpace);
            }

            Main.LocalPlayer.mouseInterface = true;
        }
    }

    public class UIBookCloseButton : UIElement
    {
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 pos = GetDimensions().ToRectangle().TopLeft();
            bool isHovering = ContainsPoint(Main.MouseScreen);

            if (Parent.Parent is UIReadableBookState parent)
            {
                Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.UI + "BookClose").Value;
                Color color = isHovering ? Color.White * 0.75f : Color.White;
                spriteBatch.Draw(texture, pos + new Vector2(texture.Width / 2f, texture.Height / 2f), null, color, 0f, texture.Size() / 2f, 1f, 0, 0);
            }
        }

        public override void MouseDown(UIMouseEvent evt)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            if (Parent.Parent is UIReadableBookState parent)
            {
                parent.showBook = false;
            }
        }
    }

    public abstract class UIBook
    {
        public abstract List<UIBookPage> bookPages { get; }
    }

    public abstract class UIBookPage
    {
        public List<UIBookElement> pageElements = new List<UIBookElement>();

        public abstract void SetPageContents();

        public UIBookPage()
        {
            SetPageContents();
        }

        public void AddElement(UIBookElement bookElement)
        {
            pageElements.Add(bookElement);
        }
    }

    public class UIBookElement : UIElement
    {
        public Rectangle drawRectangle { get; protected set; }
        public float scale { get; protected set; }
        public bool drawBorder { get; protected set; }
    }

    public class UIBookText : UIBookElement
    {
        public string text { get; }
        public Color color { get; }
        public UIBookText(Rectangle drawRectangle, string text, Color color, float scale = 1f, bool drawBorder = false)
        {
            this.drawRectangle = drawRectangle;
            this.text = text;
            this.color = color;
            this.scale = scale;
            this.drawBorder = drawBorder;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = new Vector2(GetDimensions().X, GetDimensions().Y);
            float maxWidth = drawRectangle.Width;

            ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, text, position, color, 0f, Vector2.Zero, Vector2.One * scale, maxWidth);
        }
    }

    public class UIBookImage : UIBookElement
    {
        public Texture2D texture;

        public UIBookImage(Rectangle drawRectangle, Texture2D texture, float scale = 1f, bool drawBorder = false)
        {
            this.drawRectangle = drawRectangle;
            this.texture = texture;
            this.scale = scale;
            this.drawBorder = drawBorder;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, GetDimensions().Center(), null, Color.White, 0, texture.Size() / 2f, 1f, 0, 0);
        }
    }
}