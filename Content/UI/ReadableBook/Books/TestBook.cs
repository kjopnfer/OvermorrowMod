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
        public override List<UIBookPage> bookPages => new List<UIBookPage>() { 
            new TestBookPage(), new TestBookPage2(), new TestBookPage3(), new TestBookPage4(), new TestBookPage5(),
        };
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

    public class TestBookPage2 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "second page", Color.Black));
            AddElement(new UIBookImage(new Rectangle(10, 50, 1, 1), ModContent.Request<Texture2D>(AssetDirectory.Resprites + "ChainKnife").Value));
        }
    }

    public class TestBookPage3 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "third page", Color.Black));
            AddElement(new UIBookImage(new Rectangle(10, 50, 1, 1), ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Minishark").Value));
        }
    }

    public class TestBookPage4 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "fourth page", Color.Black));
            AddElement(new UIBookImage(new Rectangle(50, 100, 1, 1), ModContent.Request<Texture2D>(AssetDirectory.Resprites + "Handgun").Value, 2.5f));
        }
    }

    public class TestBookPage5 : UIBookPage
    {
        public override void SetPageContents()
        {
            AddElement(new UIBookText(new Rectangle(1, 1, 1, 1), "bruh", Color.Black));
        }
    }
}