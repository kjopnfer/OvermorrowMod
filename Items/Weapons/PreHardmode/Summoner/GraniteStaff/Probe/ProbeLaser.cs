using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace OvermorrowMod.Items.Weapons.PreHardmode.Summoner.GraniteStaff.Probe
{
    public class ProbeLaser : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.EyeBeam);
            aiType = ProjectileID.EyeBeam;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 150;
        }
    }
}