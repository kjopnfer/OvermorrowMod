using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace OvermorrowMod.NPCs.Sky
{
    public class SkyScythe : ModProjectile
    {
        private int ProjTimer = 0;
        private float speed = 0.22f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Scythe");     //The English name of the projectile
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.FlamingScythe);
            projectile.width = 48;
            projectile.height = 48;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 70;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            ProjTimer++;
            if (ProjTimer == 60)
            {
                speed += 0.1f;
                ProjTimer = 0;
            }

            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.ManaRegeneration, projectile.oldVelocity.X * 0f, projectile.oldVelocity.Y * 0f, 1, new Color(), 0.8f);
            }
            if (projectile.velocity.X > 0)
            {
                projectile.rotation += speed;
            }
            else
            {
                projectile.rotation -= speed;
            }
        }
    }
}