using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Piercing
{
    public class FungiSpore : ModProjectile
    {
        public override string Texture => "Terraria/NPC_" + NPCID.FungiSpore;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungi Spore");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            // Projectile gravity
            projectile.velocity.Y += 0.02f;

            if (projectile.velocity.Y > 1f) // Terminal velocity
            {
                projectile.velocity.Y = 1f;
            }

            // Make projectiles gradually disappear
            if (projectile.timeLeft <= 60)
            {
                projectile.alpha += 5;
            }

            projectile.ai[1]++;
        }
    }
}