using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class BarrierWave : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shockwave");
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 96;
            projectile.timeLeft = 69420;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
        }

        public override void AI()
        {
            if (++projectile.ai[0] % 60 == 0)
            {
                Particle.CreateParticle(Particle.ParticleType<Pulse>(), projectile.Center, Vector2.Zero, Color.Orange, 1, 0.5f, projectile.velocity.ToRotation());
            }

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<DharuudArena>())
                {
                    if (projectile.Hitbox.Intersects(proj.Hitbox))
                    {
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), proj.Center, Vector2.Zero, Color.Yellow, 1, 2f);

                        foreach (Player player in Main.player)
                        {
                            if (player.active && projectile.Distance(player.Center) < 300 && player.immuneTime == 0)
                            {
                                player.Hurt(PlayerDeathReason.ByCustomReason(player.name + " was flattened by shock waves."), 50, 0);
                            }

                            if (player.active && projectile.Distance(player.Center) < 1200)
                            {
                                var modPlayer = player.Overmorrow();
                                modPlayer.AddScreenShake(45, 10);
                            }
                        }

                        projectile.Kill();
                    }
                }
            }
        }
    }
}