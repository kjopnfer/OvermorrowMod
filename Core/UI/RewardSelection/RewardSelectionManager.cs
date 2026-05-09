using Terraria;

namespace OvermorrowMod.Core.UI
{
    public static class RewardSelectionManager
    {
        private static RewardSelection UI;

        internal static void Initialize(RewardSelection state) => UI = state;

        public static void ShowFor(NPC chest, int[] itemIds) => UI?.Show(chest, itemIds);
        public static void Hide() => UI?.Hide();
        public static bool IsVisible() => RewardSelection.visible;
    }
}
