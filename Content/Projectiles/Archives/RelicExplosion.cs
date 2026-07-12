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

namespace OvermorrowMod.Content.Projectiles.Archives
{
    public class RelicExplosion : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Empty;

        private const int DetonationTime = 30;
        private const int FlashDuration = 10;
        private const int SparkDelay = 8;
        private const int BurstRadius = 220;

        private Color coreColor = new Color(254, 254, 255);
        private Color edgeColor = new Color(255, 120, 220);
        private Color smokeStart = new Color(117, 100, 255);
        private Color smokeEnd = new Color(12, 24, 55);

        public ref float Timer => ref Projectile.ai[0];
        public ref float Palette => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = DetonationTime + 2;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Palette == 0f)
            {
                coreColor = new Color(239, 245, 255);
                edgeColor = new Color(31, 44, 255);
                smokeStart = new Color(26, 117, 255);
                smokeEnd = new Color(13, 7, 153);
            }
            else
            {
                coreColor = new Color(254, 254, 255);
                edgeColor = new Color(255, 82, 119);
                smokeStart = new Color(189, 74, 210);
                smokeEnd = new Color(104, 5, 164);
            }

            SoundEngine.PlaySound(SoundID.Item74 with { Pitch = -0.2f }, Projectile.Center);

            var spec = new ParticleSpec
            {
                Count = 1,
                ConeSpread = 180f,
                Angle = -90f,
                SpeedMin = 0f,
                SpeedMax = 0f,
                Drag = 0.058f,
                AngularVelMin = 0.044f,
                AngularVelMax = 0.053f,
                RotationOffsetDeg = -90f,
                FlipVertical = true,
                LifetimeMin = 33,
                LifetimeMax = 41,
                StartScaleMin = 0.154f,
                StartScaleMax = 0.327f,
                EndScale = 0.5f,
                ScaleEasing = ParticleEasing.EaseOut,
                AlphaFadeOutFrac = 0.854f,
                StartColor = coreColor,
                EndColor = smokeEnd,
                Texture = "pulse",
            };

            ParticleEmitter.EmitWorld(spec, Projectile.Center);

            var spec2 = new ParticleSpec
            {
                Shape = EmitShape.Cone,
                Count = 28,
                Rate = 85.846f,
                ConeSpread = 180f,
                InitialVelocity = new Vector2(0f, -3.077f),
                Drag = 0.1f,
                Gravity = new Vector2(0f, -0.308f),
                Turbulence = 0.115f,
                AngularVelMin = -0.032f,
                AngularVelMax = 0.046f,
                Orientation = ParticleOrientation.Spin,
                LifetimeMin = 45,
                LifetimeMax = 67,
                StartScaleMin = 0.192f,
                StartScaleMax = 0.404f,
                EndScale = 0.477f,
                AlphaFadeOutFrac = 0.646f,
                StartColor = smokeStart,
                EndColor = smokeStart,
                Texture = "smoke_07",
                Textures = new() { "smoke_07", "smoke_08", "smoke_06", "smoke_05", "smoke_04", "smoke_03", "smoke_02", "smoke_01" },
            };
            int puffs = Main.rand.Next(8, 12);
            for (int i = 0; i < puffs; i++)
            {
                float driftX = Main.rand.NextFloat(-10f, 10f);
                float riseY = Main.rand.NextFloat(-3f, -2f);
                float sizeScale = Main.rand.NextFloat(0.4f, 0.75f);
                var puff = spec2 with
                {
                    Count = Math.Max(6, spec2.Count / puffs),
                    InitialVelocity = new Vector2(driftX, riseY),
                    StartScaleMin = spec2.StartScaleMin * sizeScale,
                    StartScaleMax = spec2.StartScaleMax * sizeScale,
                    EndScale = spec2.EndScale * sizeScale,
                };
                ParticleEmitter.EmitWorld(puff, Projectile.Center);
            }

            var smoke = new ParticleSpec
            {
                Shape = EmitShape.Cone,
                Count = 23,
                ConeSpread = 180f,
                Angle = -90f,
                SpeedMin = 5.769f,
                SpeedMax = 9.615f,
                Drag = 0.058f,
                AngularVelMin = 0.044f,
                AngularVelMax = 0.053f,
                RotationOffsetDeg = -90f,
                FlipVertical = true,
                LifetimeMin = 25,
                LifetimeMax = 53,
                StartScaleMin = 0.231f,
                StartScaleMax = 0.423f,
                EndScale = 0.481f,
                StartColor = smokeStart,
                EndColor = smokeEnd,
                Texture = "flame_01",
                Textures = new() { "flame_01", "flame_02", "flame_03", "flame_04" },
            };

            ParticleEmitter.EmitWorld(smoke, Projectile.Center);
        }

        public override void AI()
        {
            float lightFade = MathHelper.Clamp(1f - Timer / DetonationTime, 0f, 1f);
            Lighting.AddLight(Projectile.Center, edgeColor.ToVector3() * 1.2f * lightFade);

            if (Timer == SparkDelay) SpawnBurstParticles();

            Timer++;
            if (Timer >= DetonationTime) Projectile.Kill();
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            int size = (int)MathHelper.Lerp(Projectile.width, BurstRadius, MathHelper.Clamp(Timer / 18f, 0f, 1f));
            hitbox.Width = size;
            hitbox.Height = size ;
            hitbox.X = (int)(Projectile.Center.X - size / 2f);
            hitbox.Y = (int)(Projectile.Center.Y - size / 2f);
        }

        private void SpawnBurstParticles()
        {
            var spec2 = new ParticleSpec
            {
                Shape = EmitShape.Cone,
                Count = 12,
                Rate = 85.846f,
                ConeSpread = 180f,
                InitialVelocity = new Vector2(0f, -3.077f),
                Drag = 0.088f,
                Gravity = new Vector2(0f, -0.308f),
                Turbulence = 0.115f,
                AngularVelMin = -0.032f,
                AngularVelMax = 0.046f,
                Orientation = ParticleOrientation.Spin,
                LifetimeMax = 60,
                StartScaleMin = 0.192f,
                StartScaleMax = 0.404f,
                EndScale = 0.377f,
                AlphaFadeOutFrac = 0.646f,
                StartColor = smokeEnd,
                EndColor = smokeStart,
                Texture = "smoke_07",
                Textures = new() { "smoke_07", "smoke_08", "smoke_06", "smoke_05", "smoke_04", "smoke_03", "smoke_02", "smoke_01" },
                Additive = false,
            };

            int puffs = Main.rand.Next(4, 8);
            for (int i = 0; i < puffs; i++)
            {
                float driftX = Main.rand.NextFloat(-2f, 2f);
                float riseY = Main.rand.NextFloat(4, 6f);
                float sizeScale = Main.rand.NextFloat(0.35f, 0.75f);
                var puff = spec2 with
                {
                    Count = Math.Max(6, spec2.Count / puffs),
                    InitialVelocity = new Vector2(driftX, riseY),
                    StartScaleMin = spec2.StartScaleMin * sizeScale,
                    StartScaleMax = spec2.StartScaleMax * sizeScale,
                    EndScale = spec2.EndScale * sizeScale,
                };
                //ParticleEmitter.EmitWorld(puff, Projectile.Center);
            }


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

            // Opening flash
            float flashT = MathHelper.Clamp(Timer / (float)FlashDuration, 0f, 1f);
            if (flashT < 1f)
            {
                Texture2D flash = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
                float flashAlpha = 1f - flashT;
                float flashScale = MathHelper.Lerp(1f, 3f, flashT) * (BurstRadius / (float)flash.Width);
                spriteBatch.Draw(flash, drawPos, null, edgeColor * flashAlpha, 0f, flash.Size() / 2f, flashScale * 1.3f, SpriteEffects.None, 0f);
                spriteBatch.Draw(flash, drawPos, null, Color.White * flashAlpha, 0f, flash.Size() / 2f, flashScale, SpriteEffects.None, 0f);
            }

            float outerScale = BurstRadius / (float)outer.Width * grow * 1.6f;
            float innerScale = BurstRadius / (float)inner.Width * grow * 1.0f;

            //spriteBatch.Draw(outer, drawPos, null, edgeColor * alpha, spin, outer.Size() / 2f, outerScale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(outer, drawPos, null, Color.White * alpha * 0.45f, spin, outer.Size() / 2f, outerScale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(inner, drawPos, null, coreColor * alpha, -spin, inner.Size() / 2f, innerScale, SpriteEffects.None, 0f);
            //spriteBatch.Draw(inner, drawPos, null, Color.White * alpha * 0.35f, -spin, inner.Size() / 2f, innerScale, SpriteEffects.None, 0f);
        }
    }
}
