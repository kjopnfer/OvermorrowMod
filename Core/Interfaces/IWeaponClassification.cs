using OvermorrowMod.Core.Items;

namespace OvermorrowMod.Core.Interfaces
{
    public interface IWeaponClassification
    {
        WeaponType WeaponType { get; }
    }

    public interface IProjectileClassification
    {
        WeaponType WeaponType { get; }
    }
}