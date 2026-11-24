using Terraria;

namespace OvermorrowMod.Core.UI.DeathScreen
{
    public static class DeathScreenManager
    {
        private static DeathScreen UI;

        internal static void Initialize(DeathScreen state) => UI = state;

        public static void ShowDeath(Player player) => UI?.ShowDeath(player);
        public static void ShowAbandoned(Player player) => UI?.ShowAbandoned(player);
        public static void Hide() => UI?.Hide();
    }
}
