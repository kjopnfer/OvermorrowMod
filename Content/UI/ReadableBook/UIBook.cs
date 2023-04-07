using System.Collections.Generic;

namespace OvermorrowMod.Content.UI.ReadableBook
{
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
}
