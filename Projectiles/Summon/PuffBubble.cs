using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Summon
{
    public class PuffBubble : ModProjectile
    {
        private bool release = false;

        public override string Texture => "Terraria/Projectile_" + ProjectileID.FlaironBubble;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.timeLeft = 75;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (!Main.player[projectile.owner].channel)
            {
                release = true;
            }
            if (release)
            {
                projectile.velocity.Y -= 0.5f;
            }
            if (!release)
            {
                projectile.velocity.X *= 0.90f;
                projectile.velocity.Y *= 0.90f;
            }
        }



        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item, projectile.position, 54);
        }
    }
}
