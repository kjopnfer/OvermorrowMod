
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
    public class CandleGain : ModProjectile
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
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            Player player = Main.player[Projectile.owner];
            if (!player.active) return;

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            randomScale = Main.rand.NextFloat(10f, 20f);
            Color color = new Color(108, 108, 224);

            /*Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_02", AssetRequestMode.ImmediateLoad).Value;

            var lightOrb = new Circle(texture, ModUtils.SecondsToTicks(0.7f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;

            float orbScale = 0.5f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, orbScale, 0.2f);

            lightOrb = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_03", AssetRequestMode.ImmediateLoad).Value, ModUtils.SecondsToTicks(0.6f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, scale: 0.6f, 0.2f);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_04", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 16; i++)
            {
                randomScale = Main.rand.NextFloat(2f, 7f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 7);

                var lightSpark = new Spark(sparkTexture, 0f, true, 0f);
                lightSpark.endColor = Color.Purple;
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity * 2, color, 1f, randomScale, 0f);
            }

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);*/

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_04", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 32; i++)
            {
                //randomScale = Main.rand.NextFloat(0.15f, 0.35f);
                randomScale = Main.rand.NextFloat(0.1f, 0.2f);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(13, 18);

                /*float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);

                //Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity, color, 1, randomScale, 0f, 0f, 1f);
                var lightSpark = new Spark(sparkTexture, 0f, true, 0f);
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity, color, 1f, randomScale, 0f);
                */
                randomScale = Main.rand.NextFloat(3f, 6f);
                RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(9, 12);
                //Particle.CreateParticle(Particle.ParticleType<RotatingEmber>(), Projectile.Center, Vector2.Normalize(RandomVelocity), Color.Orange, 1f, randomScale, 0f, 0f, -1f);
                var rotatingEmber = new Spark(sparkTexture, Main.rand.Next(8, 10) * 10, false, -1f);
                rotatingEmber.endColor = Color.Purple;
                rotatingEmber.AnchorEntity = player;

                ParticleManager.CreateParticleDirect(rotatingEmber, Projectile.Center, Vector2.Normalize(RandomVelocity), color, 1f, randomScale, 0f);
            }


            //Particle.CreateParticle(Particle.ParticleType<Circle>(), Projectile.Center, Vector2.Zero, color, 1, randomScale, 0f, 0f, 1f);
        }

        public ref float AICounter => ref Projectile.ai[0];

        public override void AI()
        {
            AICounter++;
            base.AI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}