using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.Bandits
{
    public class FlameArrow : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Color.Lerp(new Color(216, 44, 4), new Color(254, 121, 2), progress) * progress;
        public float TrailSize(float progress) => 16;
        public Type TrailType() => typeof(TorchTrail);

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;

            DrawOffsetX = -16;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(253, 254, 255), Main.rand.NextFloat(0.5f, 0.7f));
                Main.dust[dust].noGravity = true;
            }

            Vector2 randomOffset = new Vector2(Main.rand.Next(-4, 0), Main.rand.Next(-2, 2));
            float randomRotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            Particle.CreateParticle(Particle.ParticleType<Ember>(), Projectile.Center + randomOffset, -Vector2.UnitX.RotatedBy(randomRotation), new Color(216, 44, 4));

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D tex = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(254, 121, 2) * 0.5f, 0f, tex.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(253, 221, 3) * 0.2f, 0f, tex.Size() / 2, Projectile.scale * 3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}