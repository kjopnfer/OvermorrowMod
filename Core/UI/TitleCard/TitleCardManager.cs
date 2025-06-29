namespace OvermorrowMod.Core.UI
{
    public static class TitleCardManager
    {
        private static TitleCard titleCard;

        internal static void Initialize(TitleCard card)
        {
            titleCard = card;
        }

        public static void ShowTitle(string text)
        {
            titleCard?.ShowTitle(text);
        }

        public static bool IsVisible()
        {
            return TitleCard.visible;
        }

        public static void Hide()
        {
            if (titleCard != null)
            {
                TitleCard.visible = false;
            }
        }
    }
}