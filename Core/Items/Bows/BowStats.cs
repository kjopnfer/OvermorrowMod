using Microsoft.Xna.Framework;
using Terraria.ID;

namespace OvermorrowMod.Core.Items.Bows
{
    public class BowStats
    {
        public float ChargeSpeed = 1f;
        public float MaxChargeTime = 60f;
        public float ShootDelay = 30f;
        public float MaxSpeed = 12f;
        public float DamageMultiplier = 1f;
        public float KnockbackMultiplier = 1f;
        public int ArrowType = ProjectileID.None;
        public int ConvertArrow = ItemID.None;
        public bool CanConsumeAmmo = true;
        public Color StringColor = Color.White;
        public bool StringGlow = false;
        public Vector2 PositionOffset = new Vector2(15, 0);
        public (Vector2, Vector2) StringPositions = (new Vector2(-5, 14), new Vector2(-5, -14));

        public BowStats Clone()
        {
            return (BowStats)this.MemberwiseClone();
        }
    }
}