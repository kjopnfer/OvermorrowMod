namespace OvermorrowMod.Common.Weapons.Guns
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
}