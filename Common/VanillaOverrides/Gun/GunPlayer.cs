using System.Collections.Generic;
using Terraria.ModLoader;

namespace OvermorrowMod.Common.VanillaOverrides.Gun
{
    public class HeldGunInfo
    {
        public int shotsFired;
        public int bonusBullets;
        public int bonusDamage;
        public int bonusAmmo;

        public HeldGunInfo(int shotsFired, int bonusBullets, int bonusDamage, int bonusAmmo)
        {
            this.shotsFired = shotsFired;
            this.bonusBullets = bonusBullets;
            this.bonusDamage = bonusDamage;
            this.bonusAmmo = bonusAmmo;
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
