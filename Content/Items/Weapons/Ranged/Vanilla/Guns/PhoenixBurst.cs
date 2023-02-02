using Microsoft.Xna.Framework;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.Vanilla.Guns
{
    public class PhoenixBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phoenix Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PhoenixBurst"));

            Projectile.Center = Main.LocalPlayer.Center;

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            Particle.CreateParticle(Particle.ParticleType<Common.Particles.PhoenixBurst>(), Projectile.Center, Vector2.Zero, Color.Orange, 1);

            for (int i = 0; i < 32; i++)
            {
                //randomScale = Main.rand.NextFloat(0.15f, 0.35f);
                randomScale = Main.rand.NextFloat(15f, 20f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(9, 15);
                Color color = Color.Orange;

                if (Main.rand.NextBool())
                    Particle.CreateParticle(Particle.ParticleType<LightSpark>(), Projectile.Center, RandomVelocity * 2, color, 1, randomScale);
                //Particle.CreateParticle(Particle.ParticleType<Orb>(), Projectile.Center, RandomVelocity, color, 1, randomScale, 0, 25);

                randomScale = Main.rand.NextFloat(20f, 30f);
                Particle.CreateParticle(Particle.ParticleType<RotatingEmber>(), Projectile.Center, Vector2.Normalize(RandomVelocity) * Main.rand.Next(5, 7), Color.Orange, 1f, randomScale, 0f, 0f, -1f, randomScale);
            }
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;
            Projectile.scale = MathHelper.Lerp(0.5f, 0, Projectile.timeLeft / 120f);
            Projectile.rotation += 0.08f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;

            /*Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_03").Value;
            float alpha = MathHelper.Lerp(1f, 0f, AICounter / 80f);
            float scale = MathHelper.Lerp(0, 1.5f, AICounter / 80f);
            Color color = Color.Lerp(Color.DarkOrange, Color.DarkRed, AICounter / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_03").Value;
            alpha = MathHelper.SmoothStep(1f, 0f, AICounter / 80f);
            scale = MathHelper.Lerp(0, 1f, AICounter / 60f);

            for (int i = 1; i <= 4; i++)
            {
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + (MathHelper.PiOver2 * i), texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            alpha = MathHelper.Lerp(1f, 0f, AICounter / 60f);
            scale = MathHelper.Lerp(0, 2f, AICounter / 100f);
            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "flame_01").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.DarkOrange * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);*/

            return false;
        }
    }
}