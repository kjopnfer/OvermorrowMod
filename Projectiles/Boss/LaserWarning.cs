using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OvermorrowMod.Projectiles.Boss
{
    public class LaserWarning : Deathray
    {
        // I added a temporary texture because we didn't have the Projectiles/Boss/LaserWarning texture
        public override string Texture => "OvermorrowMod/Projectiles/Boss/ElectricBall";
        public LaserWarning() : base(60, 500f, 0f, Color.White, "OvermorrowMod/Projectiles/Boss/ElectricBall") {}
        public override bool CanHitPlayer(Player target) => false;
        public override bool? CanHitNPC(NPC target) => false;
        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center, projectile.velocity, ProjectileID.CultistBossLightningOrbArc, projectile.damage, projectile.knockBack, projectile.owner);
        }
    }
}