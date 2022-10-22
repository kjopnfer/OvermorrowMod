using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Cutscenes;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using System;
using System.Xml;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles
{
    public class TestFire : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("FIRE TEST");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.timeLeft = 420;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.ai[0]++ % 3 == 0)
                Particle.CreateParticle(Particle.ParticleType<Flames>(), Projectile.Center + Vector2.UnitX * Main.rand.Next(-6, 6), -Vector2.UnitY * Main.rand.Next(6, 9), Color.Orange, 1f, Main.rand.NextFloat(0.35f, 0.4f), Main.rand.NextFloat(0, MathHelper.PiOver2), Main.rand.NextFloat(0.01f, 0.015f));
        }
    }
}