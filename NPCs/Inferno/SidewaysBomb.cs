using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Inferno
{
    public class SidewaysBomb : ModProjectile
    {
        private int timer = 0;

        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.scale = 3f;
            projectile.timeLeft = 500;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosion");
        }
        public override void AI()
        {
            timer++;
            if (timer == 10)
            {
                Vector2 value1 = new Vector2(0f, 0f);
                Projectile.NewProjectile(projectile.position.X, projectile.position.Y, value1.X, value1.Y, mod.ProjectileType("InfernoExplosion"), projectile.damage + 30, 1f, projectile.owner, 0f);
                timer = 0;
            }
        }
    }
}