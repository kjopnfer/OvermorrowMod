using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using Terraria;
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
            projectile.width = projectile.height = 12;
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
                Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Color.Orange, 1, 0.5f, projectile.velocity.ToRotation());
            }

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.whoAmI != projectile.whoAmI && proj.active && proj.type == ModContent.ProjectileType<BarrierWave>())
                {
                    if (projectile.Hitbox.Intersects(proj.Hitbox))
                    {
                        Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), projectile.Center, Vector2.Zero, Color.Yellow, 1, 2f);

                        foreach (Player player in Main.player)
                        {
                            if (player.active && projectile.Distance(player.Center) < 420)
                            {
                                var modPlayer = player.Overmorrow();
                                modPlayer.ScreenShake = 45;
                                modPlayer.ShakeOffset = 10;
                            }
                        }

                        proj.Kill();
                        projectile.Kill();
                    }
                }
            }
        }
    }
}