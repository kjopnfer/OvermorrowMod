using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Interfaces;
using OvermorrowMod.Core.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Projectiles.Archives
{
    public class RestLight : ModProjectile, IDrawAdditive
    {
        public override string Texture => AssetDirectory.Textures + "light_01";

        private const int MaxLife = 45;

        private Player Owner => Main.player[Projectile.owner];
        private float Progress => 1f - Projectile.timeLeft / (float)MaxLife;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = MaxLife;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
        }

        public override void AI()
        {
            if (Owner.active && !Owner.dead)
                Projectile.Center = Owner.Center;

            if (Projectile.timeLeft > MaxLife - 24 && !Main.gamePaused)
            {
                Texture2D streak = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_05").Value;
                Color warmStart = new Color(255, 214, 120);
                Color warmEnd = new Color(255, 120, 20);

                if (Main.rand.NextBool())
                {
                    var ember = new Spark(streak, ModUtils.SecondsToTicks(Main.rand.NextFloat(0.7f, 1.1f)), rotateWithVelocity: false)
                    {
                        endColor = warmEnd,
                        slowModifier = 0.97f,
                        squashHeight = false
                    };

                    Vector2 pos = Owner.Bottom + new Vector2(Main.rand.Next(-Owner.width - 16, Owner.width + 16), Main.rand.Next(-4, 6));
                    Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(2f, 4f) + Vector2.UnitX * Main.rand.NextFloat(-0.6f, 0.6f);
                    float scale = Main.rand.NextFloat(0.1f, 0.22f);
                    ParticleManager.CreateParticleDirect(ember, pos, velocity, warmStart, 1f, scale, 0f, ParticleDrawLayer.AboveAll, useAdditiveBlending: true);
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.6f, 0.25f) * (1f - Progress));
        }

        public void DrawAdditive(SpriteBatch spriteBatch)
        {
            Texture2D glow = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_01").Value;

            float p = Progress;
            Vector2 center = Projectile.Center - Main.screenPosition;
            Color gold = new Color(255, 190, 90);
            Color warmWhite = new Color(255, 240, 205);
            Vector2 glowOrigin = glow.Size() / 2f;

            float rise = Utils.GetLerpValue(0f, 0.12f, p, true);
            float decay = Utils.GetLerpValue(1f, 0.2f, p, true);
            float intensity = rise * decay;
            float burst = EasingUtils.EaseOutQuad(p);

            float rotation = Projectile.rotation;

            float waveScale = 0.14f + burst * 0.6f;
            float waveAlpha = decay * (1f - burst) * 0.6f;
            spriteBatch.Draw(glow, center, null, gold * waveAlpha, rotation, glowOrigin, waveScale, SpriteEffects.None, 0f);

            float coreScale = 1f + burst * 0.45f;
            (float scale, float alpha, Color color)[] flashLayers = new (float, float, Color)[]
            {
                (0.30f, 0.40f, gold),
                (0.21f, 0.60f, warmWhite),
                (0.13f, 0.85f, warmWhite),
                (0.07f, 1.00f, Color.White),
            };

            for (int l = 0; l < flashLayers.Length; l++)
            {
                var (scale, alpha, color) = flashLayers[l];
                spriteBatch.Draw(glow, center, null, color * (alpha * intensity), rotation + l * 0.6f, glowOrigin, scale * coreScale, SpriteEffects.None, 0f);
            }
        }
    }
}
