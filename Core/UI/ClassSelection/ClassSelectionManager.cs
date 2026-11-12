namespace OvermorrowMod.Core.UI.ClassSelection
{
    public static class ClassSelectionManager
    {
        private static ClassSelection UI;

        internal static void Initialize(ClassSelection state) => UI = state;

        public static void Show() => UI?.Show();
        public static void Hide() => UI?.Hide();
        public static bool IsVisible() => ClassSelection.visible;
    }
}
