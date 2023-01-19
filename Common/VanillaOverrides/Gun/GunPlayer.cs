using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class HeldGunInfo
    {
        public int shotsFired;
        public HeldGunInfo(int shotsFired)
        {
            this.shotsFired = shotsFired;
        }
    }

    public class GunPlayer : ModPlayer
    {
        /// <summary>
        /// Used to preserve data between guns whenever swapped to prevent reload skipping
        /// </summary>
        public Dictionary<int, HeldGunInfo> playerGunInfo = new Dictionary<int, HeldGunInfo>();
    }
}
