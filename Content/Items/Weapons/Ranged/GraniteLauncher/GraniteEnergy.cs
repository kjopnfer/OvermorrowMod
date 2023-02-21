using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Common.VanillaOverrides.Gun;
using OvermorrowMod.Core;
using OvermorrowMod.Core.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.GraniteLauncher
{
    public class GraniteEnergy : ModProjectile, ITrailEntity
    {
        public override string Texture => AssetDirectory.Empty;
        public Color TrailColor(float progress) => Color.Lerp(new Color(0, 137, 255), new Color(122, 232, 246), progress) * progress;
        public float TrailSize(float progress) => 24;
        public Type TrailType() => typeof(TorchTrail);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.extraUpdates = 2;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public ref float AICounter => ref Projectile.ai[0];
        Player player => Main.player[Projectile.owner];

        public override void AI()
        {
            if (!player.active) Projectile.Kill();

            if (AICounter > 45)
            {
                Vector2 move = Vector2.Zero;
                if (player.active)
                {
                    Vector2 newMove = player.Center - Projectile.Center;
                    move = newMove;

                    AdjustMagnitude(ref move);
                    Projectile.velocity = (10 * Projectile.velocity + move) / 5f;
                    AdjustMagnitude(ref Projectile.velocity);
                }
            }

            AICounter++;

            if (Projectile.Hitbox.Intersects(player.Hitbox) && AICounter > 120)
            {

                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    float randomScale = Main.rand.NextFloat(0.25f, 0.35f);
                    float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(80), MathHelper.ToRadians(80));

                    Color color = Color.Cyan;
                    //Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, color, 1, randomScale);
                }

                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<LightBurst>(), Projectile.Center, Vector2.Zero, Color.Cyan * 0.8f, 1, 0.75f, MathHelper.ToRadians(Main.rand.Next(0, 360)));
            player.GetModPlayer<GunPlayer>().GraniteEnergyCount++;
        }

        private void AdjustMagnitude(ref Vector2 vector)
        {
            float magnitude = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
            if (magnitude > 6f)
            {
                vector *= 6f / magnitude;
            }
        }
    }
}