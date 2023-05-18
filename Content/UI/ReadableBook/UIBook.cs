using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Core;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.UI.ReadableBook
{
    public abstract class UIBook
    {
        public virtual Texture2D BookTexture => ModContent.Request<Texture2D>(AssetDirectory.UI + "Books/TestBookBack", AssetRequestMode.ImmediateLoad).Value;
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
}
