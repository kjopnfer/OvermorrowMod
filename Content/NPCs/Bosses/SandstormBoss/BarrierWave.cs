using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class BarrierWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 96;
            Projectile.timeLeft = 69420;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
        }

        public override void AI()
        {
            if (++Projectile.ai[0] % 60 == 0)
            {
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, Color.Orange, 1, 0.5f, Projectile.velocity.ToRotation());
            }

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<DharuudArena>())
                {
                    if (Projectile.Hitbox.Intersects(proj.Hitbox))
                    {
                        Particle.CreateParticle(Particle.ParticleType<Shockwave>(), proj.Center, Vector2.Zero, Color.Yellow, 1, 2f);

                        foreach (Player player in Main.player)
                        {
                            if (player.active && Projectile.Distance(player.Center) < 300 && player.immuneTime == 0)
                            {
                                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was flattened by shock waves."), 50, 0);
                            }

                            if (player.active && Projectile.Distance(player.Center) < 1200)
                            {
                                var modPlayer = player.Overmorrow();
                                modPlayer.AddScreenShake(45, 10);
                            }
                        }

                        Projectile.Kill();
                    }
                }
            }
        }
    }
}