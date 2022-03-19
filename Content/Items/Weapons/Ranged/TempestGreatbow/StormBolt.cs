using OvermorrowMod.Common;
using OvermorrowMod.Content.NPCs.Bosses.StormDrake;
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
            projectile.width = 10;
            projectile.friendly = true;
            projectile.timeLeft = (int)maxTime;
            Length = 1f;
            Sine = true;
        }
        public override void AI()
        {
            Length = TRay.CastLength(projectile.Center, projectile.velocity, 2000f);
            Positions = CreateLightning(projectile.Center, projectile.Center + projectile.velocity * Length, projectile.width/*, Sine*/);
            float progress = (maxTime - (float)projectile.timeLeft) / maxTime;
            float mult = (float)Math.Sin(progress * Math.PI);
            for (int i = 0; i < Positions.Count; i++)
            {
                Positions[i].Size = Positions[i].DefSize * mult;
            }
        }
    }
}