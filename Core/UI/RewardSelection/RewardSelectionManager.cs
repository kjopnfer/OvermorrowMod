using Terraria;

namespace OvermorrowMod.Core.UI
{
    public static class RewardSelectionManager
    {
        private static RewardSelection UI;

        internal static void Initialize(RewardSelection state) => UI = state;

        public static void ShowFor(IRewardSource source, int[] itemIds) => UI?.Show(source, itemIds);
        public static void Hide() => UI?.Hide();
        public static bool IsVisible() => RewardSelection.visible;
        public static float Opacity => UI?.Opacity ?? 0f;
    }
}
