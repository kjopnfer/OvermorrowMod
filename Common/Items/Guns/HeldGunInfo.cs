using System.Collections.Generic;

namespace OvermorrowMod.Common.Items.Guns
{
    public class HeldGunInfo
    {
        public readonly List<int> loadedItemTypes;
        public readonly List<int> loadedProjectileTypes;
        public readonly int bonusBullets;
        public readonly int bonusDamage;
        public readonly int bonusAmmo;

        public HeldGunInfo(List<int> loadedItemTypes, List<int> loadedProjectileTypes, int bonusBullets, int bonusDamage, int bonusAmmo)
        {
            this.loadedItemTypes = loadedItemTypes;
            this.loadedProjectileTypes = loadedProjectileTypes;
            this.bonusBullets = bonusBullets;
            this.bonusDamage = bonusDamage;
            this.bonusAmmo = bonusAmmo;
        }
    }
}
