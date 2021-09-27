using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class StickyWeb : ModProjectile
    {

        public override bool CanDamage() => true;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.StickyGlowstick);
            aiType = ProjectileID.StickyGlowstick;
            projectile.friendly = false;
            projectile.timeLeft = 700;
            projectile.hostile = true;
            projectile.width = 26;
            projectile.height = 26;
        }

        public override void AI()
        {
            if (projectile.timeLeft < 650)
            {
                projectile.velocity.X = 0;
                projectile.velocity.Y = 0;
            }


            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Fire, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 5, new Color(), 1f);
            }
        }
    }
}