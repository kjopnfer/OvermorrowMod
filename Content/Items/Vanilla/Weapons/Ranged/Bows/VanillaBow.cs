using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Items.Bows;
using OvermorrowMod.Core.Items.Bows;
using Terraria.ID;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged.Bows
{
    /// <summary>
    /// Shared held projectile for resprited vanilla bows. Subclasses supply the bow's
    /// resprite texture, the vanilla item it replaces, and its string color.
    /// </summary>
    public abstract class VanillaBow_Held : HeldBow
    {
        protected abstract Color BowStringColor { get; }
        protected virtual int ConvertArrowItem => ItemID.None;
        protected virtual int ForcedArrowProjectile => ProjectileID.None;

        protected virtual float ChargeRate => 1.5f;
        protected virtual float ChargeTime => 36f;
        protected virtual float FireDelay => 12f;
        protected virtual float ArrowSpeed => 13f;

        public override BowStats GetBaseBowStats()
        {
            return new BowStats
            {
                ChargeSpeed = ChargeRate,
                MaxChargeTime = ChargeTime,
                ShootDelay = FireDelay,
                MaxSpeed = ArrowSpeed,
                StringColor = BowStringColor,
                ConvertArrow = ConvertArrowItem,
                ArrowType = ForcedArrowProjectile
            };
        }
    }
}
