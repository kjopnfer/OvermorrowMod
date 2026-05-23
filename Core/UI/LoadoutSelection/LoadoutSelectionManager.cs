namespace OvermorrowMod.Core.UI.LoadoutSelection
{
    public static class LoadoutSelectionManager
    {
        private static LoadoutSelection UI;

        internal static void Initialize(LoadoutSelection state) => UI = state;

        public static void Show() => UI?.Show();
        public static void Hide() => UI?.Hide();
        public static bool IsVisible() => LoadoutSelection.visible;
    }
}
