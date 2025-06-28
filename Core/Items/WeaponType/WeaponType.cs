using System;

namespace OvermorrowMod.Core.Items
{
    [Flags]
    public enum WeaponType
    {
        None = 0,
        Sword = 1 << 0,
        Broadsword = 1 << 1,
        Rapier = 1 << 2,
        Greatsword = 1 << 3,
        Shortsword = 1 << 4,
        Dagger = 1 << 5,
        Bow = 1 << 6,
        Staff = 1 << 7,
        Whip = 1 << 8,
        Revolver = 1 << 9,
        Handgun = 1 << 10,
        Shotgun = 1 << 11,
        Musket = 1 << 12,
        Rifle = 1 << 13,
        SubMachineGun = 1 << 14,
        MachineGun = 1 << 15,
        Launcher = 1 << 16,
        Sniper = 1 << 17,
    }
}