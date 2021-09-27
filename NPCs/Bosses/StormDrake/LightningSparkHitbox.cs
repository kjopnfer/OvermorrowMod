using Microsoft.Xna.Framework;
using OvermorrowMod.Particles;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Projectiles.Boss
{
    public class LightningSparkHitbox : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetDefaults()
        {
            projectile.width = 128;
            projectile.height = 128;
            projectile.tileCollide = false;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.timeLeft = 200;
        }
        public override void AI()
        {
            if (++projectile.ai[1] == 1)
            {
                Particle.CreateParticle(Particle.ParticleType<LightningSpark>(), projectile.Center, Vector2.Zero, Color.DarkCyan, 1, 1, 0, 1f);
            }
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0);
        }
    }
}