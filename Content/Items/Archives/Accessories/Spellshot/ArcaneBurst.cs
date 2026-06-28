using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles.Modular;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives.Accessories
{
    public class ArcaneBurst : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Empty;

        private const int DetonationTime = 30;
        private const int FlashDuration = 10;
        private const int SparkDelay = 8;
        private const int BurstRadius = 220;

        private static readonly Color CorePurple = new Color(200, 120, 255);
        private static readonly Color EdgePink = new Color(255, 120, 220);

        public ref float Timer => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = DetonationTime + 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.2f }, Projectile.Center);

            var spec2 = new ParticleSpec
            {
                Count = 30,
                ConeSpread = 134.308f,
                Angle = 90f,
                SpeedMin = 2.308f,
                SpeedMax = 6.154f,
                Drag = 0.023f,
                AngularVelMin = -0.074f,
                AngularVelMax = 0.023f,
                Orientation = ParticleOrientation.Spin,
                RotationOffsetDeg = -90f,
                FlipVertical = true,
                LifetimeMax = 84,
                StartScaleMin = 0.058f,
                StartScaleMax = 0.096f,
                ScaleEasing = ParticleEasing.EaseIn,
                AlphaFadeOutFrac = 0.262f,
                EndColor = new Color(200, 50, 255),
                Texture = "trace_01",
            };
            ParticleEmitter.EmitWorld(spec2, Projectile.Center);

            var smoke = new ParticleSpec
            {
                Count = 7,
                ConeSpread = 134.308f,
                Angle = 90f,
                SpeedMin = 2.308f,
                SpeedMax = 6.154f,
                Drag = 0.023f,
                AngularVelMin = -0.074f,
                AngularVelMax = 0.023f,
                Orientation = ParticleOrientation.Spin,
                RotationOffsetDeg = -90f,
                FlipVertical = true,
                LifetimeMin = 19,
                LifetimeMax = 44,
                StartScaleMin = 0.058f,
                StartScaleMax = 0.577f,
                ScaleEasing = ParticleEasing.EaseIn,
                AlphaFadeInFrac = 0.046f,
                AlphaFadeOutFrac = 0.923f,
                StartColor = new Color(249, 94, 255),
                EndColor = new Color(255, 50, 214),
                Texture = "flame_04",
                Additive = false,
            };
            ParticleEmitter.EmitWorld(smoke, Projectile.Center);
        }

        public override void AI()
        {
            float lightFade = MathHelper.Clamp(1f - Timer / DetonationTime, 0f, 1f);
            Lighting.AddLight(Projectile.Center, EdgePink.ToVector3() * 1.2f * lightFade);

            // Sparks arrive once the opening flash is nearly spent.
            if (Timer == SparkDelay) SpawnBurstParticles();

            Timer++;
            if (Timer >= DetonationTime) Projectile.Kill();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)MathHelper.Lerp(Projectile.width, BurstRadius, MathHelper.Clamp(Timer / 18f, 0f, 1f));
            hitbox.Width = size;
            hitbox.Height = size;
            hitbox.X = (int)(Projectile.Center.X - size / 2f);
            hitbox.Y = (int)(Projectile.Center.Y - size / 2f);
        }

        private void SpawnBurstParticles()
        {
            var spec = new ParticleSpec
            {
                Count = 20,
                SpeedMin = 5.769f, SpeedMax = 9.615f, Drag = 0.054f,
                AngularVelMin = 0.014f, AngularVelMax = 0.023f,
                LifetimeMax = 62,
                StartScaleMin = 0.135f, StartScaleMax = 0.404f,
                StartColor = new Color(255, 28, 198), EndColor = new Color(255, 227, 250),
                Texture = "trace_01", RotationOffsetDeg = -90f, FlipVertical = true,
            };
            ParticleEmitter.EmitWorld(spec, Projectile.Center);

           

        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            float t = MathHelper.Clamp(Timer / DetonationTime, 0f, 1f);
            float grow = (float)Math.Sqrt(t);
            float alpha = MathHelper.Clamp((1f - t) * 1.6f, 0f, 1f);

            Texture2D outer = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_circle_01").Value;
            Texture2D inner = ModContent.Request<Texture2D>(AssetDirectory.Textures + "magic_circle_02").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float spin = Timer * 0.1f;

            // Opening flash, front-loaded so it pops before the sparks arrive.
            float flashT = MathHelper.Clamp(Timer / (float)FlashDuration, 0f, 1f);
            if (flashT < 1f)
            {
                Texture2D flash = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
                float flashAlpha = 1f - flashT;
                float flashScale = MathHelper.Lerp(1f, 3f, flashT) * (BurstRadius / (float)flash.Width);
                spriteBatch.Draw(flash, drawPos, null, EdgePink * flashAlpha, 0f, flash.Size() / 2f, flashScale * 1.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(flash, drawPos, null, Color.White * flashAlpha, 0f, flash.Size() / 2f, flashScale, SpriteEffects.None, 0f);
            }

            float outerScale = BurstRadius / (float)outer.Width * grow * 1.6f;
            float innerScale = BurstRadius / (float)inner.Width * grow * 1.0f;

            spriteBatch.Draw(outer, drawPos, null, EdgePink * alpha, spin, outer.Size() / 2f, outerScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(outer, drawPos, null, Color.White * alpha * 0.45f, spin, outer.Size() / 2f, outerScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(inner, drawPos, null, CorePurple * alpha, -spin, inner.Size() / 2f, innerScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(inner, drawPos, null, Color.White * alpha * 0.35f, -spin, inner.Size() / 2f, innerScale, SpriteEffects.None, 0f);
        }
    }
}
