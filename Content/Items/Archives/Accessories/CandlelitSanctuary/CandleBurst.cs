
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Globals;
using OvermorrowMod.Core.Particles;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
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
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            randomScale = Main.rand.NextFloat(10f, 20f);

            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_02").Value;

            Color color = new Color(108, 108, 224);
            var lightOrb = new Circle(texture, ModUtils.SecondsToTicks(0.7f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;

            float orbScale = 0.5f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, orbScale, 0.2f);

            lightOrb = new Circle(ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_03").Value, ModUtils.SecondsToTicks(0.6f), canGrow: true, useSineFade: true);
            lightOrb.rotationAmount = 0.05f;
            ParticleManager.CreateParticleDirect(lightOrb, Projectile.Center, Vector2.Zero, color, 1f, scale: 0.6f, 0.2f);

            for (int i = 0; i < 16; i++)
            {
                //randomScale = Main.rand.NextFloat(0.15f, 0.35f);
                randomScale = Main.rand.NextFloat(2f, 7f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(4, 7);

                var lightSpark = new Spark(randomScale, true, 0f);
                lightSpark.endColor = Color.Purple;
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity * 2, color, 1f, 1f, 0f);
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
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "scorch_01").Value;
            float alpha = MathHelper.Lerp(0.5f, 0f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            float scale = MathHelper.Lerp(0f, 1f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            for (int i = 0; i < 4; i++)
            {
                float rotation = MathHelper.PiOver4 * i;
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(211, 122, 255) * alpha, Projectile.rotation + rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(BlendState.Additive);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_03").Value;
            scale = MathHelper.Lerp(0f, 1f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
            for (int i = 0; i < 3; i++)
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, new Color(171, 3, 255) * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            //texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            //Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return base.PreDraw(ref lightColor);
        }
    }
}