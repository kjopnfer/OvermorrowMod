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
        Color Lerp3(Color a, Color b, Color c, float t)
        {
            if (t < 0.5f) // 0.0 to 0.5 goes to a -> b
                return Color.Lerp(a, b, t / 0.5f);
            else // 0.5 to 1.0 goes to b -> c
                return Color.Lerp(b, c, (t - 0.5f) / 0.5f);
        }

        public Color TrailColor(float progress) => Lerp3(new Color(216, 44, 4), new Color(254, 121, 2), new Color(253, 221, 3), progress) * progress;

        //public Color TrailColor(float progress) => Color.Lerp(new Color(216, 44, 4), new Color(254, 121, 2), progress) * progress;
        public float TrailSize(float progress) => 20;
        public Type TrailType() => typeof(TorchTrail);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;

            DrawOffsetX = -16;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X + Projectile.velocity.X, Projectile.position.Y + Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.RainbowMk2, Projectile.velocity.X, Projectile.velocity.Y, 100, new Color(253, 254, 255), Main.rand.NextFloat(0.5f, 0.7f));
                Main.dust[dust].noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 randomOffset = new Vector2(Main.rand.Next(-4, 0), Main.rand.Next(-2, 2));
                float randomRotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                //Particle.CreateParticle(Particle.ParticleType<Ember>(), Projectile.Center + randomOffset, -Vector2.UnitX.RotatedBy(randomRotation), new Color(216, 44, 4));
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Reload(BlendState.Additive);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(254, 121, 2) * 0.65f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 0.7f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(216, 44, 4) * 0.3f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0f);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Spotlight").Value;

            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            var fadeMult = 1f / trailLength;
            for (int i = 1; i < trailLength; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(254, 121, 2) * (1f - fadeMult * i) * 0.5f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 1.5f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(texture, Projectile.oldPos[i] - Main.screenPosition + offset, null, new Color(254, 121, 2) * (1f - fadeMult * i) * 0.2f, Projectile.oldRot[i], texture.Size() / 2f, Projectile.scale * (trailLength - i) / trailLength * 3f, SpriteEffects.None, 0);
            }

            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(254, 121, 2) * 0.5f, 0f, texture.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(253, 221, 3) * 0.2f, 0f, texture.Size() / 2, Projectile.scale * 3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}