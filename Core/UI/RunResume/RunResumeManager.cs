namespace OvermorrowMod.Core.UI.RunResume
{
    public static class RunResumeManager
    {
        private static RunResumePrompt UI;

        internal static void Initialize(RunResumePrompt state) => UI = state;

        public static void Show() => UI?.Show();
        public static void Hide() => UI?.Hide();
    }
}
