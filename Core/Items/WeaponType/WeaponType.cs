using System;

namespace OvermorrowMod.Core.Items
{
    [Flags]
    public enum WeaponType
    {
        None = 0,
        Rapier = 1 << 0,
        Greatsword = 1 << 1,
        Dagger = 1 << 2,
        Bow = 1 << 3,
        Staff = 1 << 4,
        Whip = 1 << 5,
    }
}