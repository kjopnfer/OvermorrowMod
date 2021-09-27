using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.SpiderBoss
{
    public class StickyWeb2 : ModProjectile
    {

        public override bool CanDamage() => true;

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.StickyGlowstick);
            aiType = ProjectileID.StickyGlowstick;
            projectile.friendly = false;
            projectile.timeLeft = 1000;
            projectile.tileCollide = false;
            projectile.hostile = true;
            projectile.width = 76;
            projectile.height = 78;
        }

        public override void AI()
        {

            if(projectile.timeLeft > 970)
            {
                projectile.damage = 0;
            }


            if(projectile.timeLeft < 970)
            {
                projectile.velocity.X = 0;
                projectile.velocity.Y = 0;
            }


            if(projectile.timeLeft < 930)
            {
                projectile.damage = 30;
            }

            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.Fire, projectile.oldVelocity.X * 0.2f, projectile.oldVelocity.Y * 0.2f, 5, new Color(), 2.1f);
            }
        }
    }
}