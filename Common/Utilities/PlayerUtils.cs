using Terraria;

namespace OvermorrowMod.Common.Utilities
{
    public static class PlayerUtils
    {
        public static int GetActiveMinionCount(this Player player)
        {
            int count = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.minion && proj.owner == player.whoAmI)
                {
                    count++;
                }
            }

            return count;
        }
    }
}