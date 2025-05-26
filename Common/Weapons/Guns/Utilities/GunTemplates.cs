using Terraria.Audio;
using Terraria.ID;

namespace OvermorrowMod.Common.Weapons.Guns
{
    public static class GunTemplates
    {
        public static void ApplyRevolverDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 60;
            gun.MaxShots = 6;
            gun.RecoilAmount = 10;
            gun.ShootSound = SoundID.Item41;
        }

        public static void ApplyShotgunDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 90;
            gun.MaxShots = 2;
            gun.RecoilAmount = 25;
            gun.ShootSound = SoundID.Item36;
            gun.ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/ShotgunReload");
        }

        public static void ApplyHandgunDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 40;
            gun.MaxShots = 10;
            gun.RecoilAmount = 15;
            gun.ShootSound = SoundID.Item41;
            gun.ReloadFinishSound = new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/HandgunReload");
        }

        public static void ApplySubMachineGunDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 60;
            gun.MaxShots = 50;
            gun.RecoilAmount = 10;
            gun.ShootSound = SoundID.Item11;
        }

        public static void ApplyMachineGunDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 60;
            gun.MaxShots = 120;
            gun.RecoilAmount = 5;
            gun.ShootSound = SoundID.Item11;
        }

        public static void ApplySniperDefaults(HeldGun gun)
        {
            gun.MaxReloadTime = 120;
            gun.MaxShots = 1;
            gun.RecoilAmount = 20;
            gun.ShootSound = SoundID.Item40;
        }
    }
}