using System.Collections.Generic;

namespace OvermorrowMod.Common.Items.Guns
{
    public class HeldGunInfo
    {
        public List<int> loadedItemTypes;
        public List<int> loadedProjectileTypes;
        public int bonusBullets;
        public int bonusDamage;
        public int bonusAmmo;

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
