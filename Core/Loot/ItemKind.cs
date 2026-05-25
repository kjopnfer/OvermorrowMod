using System;

namespace OvermorrowMod.Core.Loot
{
    [Flags]
    public enum ItemKind
    {
        None       = 0,
        Weapon     = 1 << 0,
        Accessory  = 1 << 1,
        Armor      = 1 << 2,
        Consumable = 1 << 3,
    }
}
