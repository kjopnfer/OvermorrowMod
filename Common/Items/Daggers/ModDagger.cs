using OvermorrowMod.Common.Items.Daggers;
using Terraria.ModLoader;

/// <summary>
/// Generic ModDagger class that inherits from the non-generic base.
/// This provides the type-safe generic interface while allowing access to the base class.
/// </summary>
public abstract class ModDagger<HeldProjectile, ThrownProjectile> : ModDaggerBase
    where HeldProjectile : HeldDagger
    where ThrownProjectile : ThrownDagger
{
    protected override int HeldProjectileType => ModContent.ProjectileType<HeldProjectile>();
    protected override int ThrownProjectileType => ModContent.ProjectileType<ThrownProjectile>();
}