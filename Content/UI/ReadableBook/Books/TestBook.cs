using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using OvermorrowMod.Core;

namespace OvermorrowMod.Content.UI.ReadableBook
{
    public class TestBook : UIBook
    {
        public override List<UIBookPage> bookPages => new List<UIBookPage>() { new TestBookPage() };
    }

    public class TestBookPage : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "test text for the page", Color.Black));
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "new text", Color.Black));
            AddElement(new UIBookImage(new Rectangle(50, 50, 1, 1), ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Boomstick").Value));
        }
    }
}