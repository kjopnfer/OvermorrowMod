using System;

namespace OvermorrowMod.Core.Loot
{
    [Flags]
    public enum ItemType
    {
        None    = 0,
        Generic = 1 << 0,
        Melee   = 1 << 1,
        Ranged  = 1 << 2,
        Magic   = 1 << 3,
        Summon  = 1 << 4,
    }

    public static class AffinityFlagsExtensions
    {
        public static int BitCount(this ItemType flags)
        {
            int value = (int)flags;
            int count = 0;
            while (value != 0)
            {
                value &= value - 1;
                count++;
            }
            return count;
        }
    }
}
