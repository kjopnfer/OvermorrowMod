using OvermorrowMod.Common;
using System;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.TempestGreatbow
{
    public class StormBolt : Lightning
    {
        public float maxTime = 15;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Bolt");
        }
        public override void SafeSetDefaults()
        {
            Projectile.width = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(Projectile.Center, Projectile.velocity, 2000f);
            Positions = CreateLightning(Projectile.Center, Projectile.Center + Projectile.velocity * Length, Projectile.width/*, Sine*/);
            float progress = (maxTime - (float)Projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}