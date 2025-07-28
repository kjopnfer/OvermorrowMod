
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Archives
{
    public class CandleBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = ModUtils.SecondsToTicks(0.75f);
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            randomScale = Main.rand.NextFloat(10f, 20f);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_02", AssetRequestMode.ImmediateLoad).Value;

            Color color = new(108, 108, 224);
            var lightOrb = new Circle(texture, ModUtils.SecondsToTicks(0.7f), canGrow: true, useSineFade: true)
            {
                rotationAmount = 0.05f
            };

            float orbScale = 0.5f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, orbScale, 0.2f, useAdditiveBlending: true);

            lightOrb = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_03", AssetRequestMode.ImmediateLoad).Value, ModUtils.SecondsToTicks(0.6f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, scale: 0.6f, 0.2f, useAdditiveBlending: true);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_04", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 16; i++)
            {
                randomScale = Main.rand.NextFloat(2f, 7f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 7);

                var lightSpark = new Spark(sparkTexture, 0f, true, 0f);
                lightSpark.endColor = Color.Purple;
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity * 2, color, 1f, randomScale, 0f, useAdditiveBlending: true);
            }

            
            //Particle.CreateParticle(Particle.ParticleType<Circle>(), Projectile.Center, Vector2.Zero, color, 1, randomScale, 0f, 0f, 1f);
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}