using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Utilities;
using OvermorrowMod.Content.Particles;
using OvermorrowMod.Core.Particles;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Vanilla.Weapons.Ranged
{
    public class PhoenixBurst : ModProjectile
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Phoenix Burst");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 85;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            //Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(new SoundStyle($"{nameof(OvermorrowMod)}/Sounds/PhoenixBurst"));

            //Projectile.Center = Main.LocalPlayer.Center;

            float randomScale = Main.rand.NextFloat(0.35f, 0.5f);
            float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);

            //Particle.CreateParticle(Particle.ParticleType<Common.Particles.PhoenixBurst>(), Projectile.Center, Vector2.Zero, Color.Orange, 1);

            Texture2D sparkTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "trace_01", AssetRequestMode.ImmediateLoad).Value;
            for (int i = 0; i < 32; i++)
            {
                //randomScale = Main.rand.NextFloat(0.15f, 0.35f);
                randomScale = Main.rand.NextFloat(15f, 20f);

                float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(45), MathHelper.ToRadians(45));
                Vector2 angleTo = Projectile.DirectionTo(Main.LocalPlayer.Center);
                Vector2 RandomVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(9, 15);
                Color color = Color.Orange;

                var lightSpark = new Spark(sparkTexture, 0f, true, 0f);
                ParticleManager.CreateParticleDirect(lightSpark, Projectile.Center, RandomVelocity * 2, Color.Orange, 1f, randomScale, 0f, useAdditiveBlending: true);

                randomScale = Main.rand.NextFloat(20f, 30f);
                
                var rotatingEmber = new Spark(sparkTexture, Main.rand.Next(8, 10) * 10, false,  -1f);
                rotatingEmber.endColor = Color.Red;
                ParticleManager.CreateParticleDirect(rotatingEmber, Projectile.Center, Vector2.Normalize(RandomVelocity) * Main.rand.Next(9, 10), Color.Orange, 1f, randomScale, 0f, useAdditiveBlending: true);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!target.HasBuff<PhoenixMarkBuff>())
            {
                target.AddBuff(BuffID.OnFire, 360);
                target.AddBuff(ModContent.BuffType<PhoenixMarkBuff>(), 360);
                Projectile.NewProjectile(null, target.Center, Vector2.Zero, ModContent.ProjectileType<PhoenixMark>(), 0, 0f, Projectile.owner, target.whoAmI);
            }
        }

        public ref float AICounter => ref Projectile.ai[0];
        public override void AI()
        {
            AICounter++;
            Projectile.scale = MathHelper.Lerp(0.5f, 0, Projectile.timeLeft / 85f);

            Projectile.rotation += 0.08f;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Width = (int)MathHelper.Lerp(Projectile.width, 512, Utils.Clamp(AICounter, 0, 20) / 20f);
            hitbox.Height = (int)MathHelper.Lerp(Projectile.width, 512, Utils.Clamp(AICounter, 0, 20) / 20f);

            hitbox.X = (int)(Projectile.Center.X - hitbox.Width / 2f);
            hitbox.Y = (int)(Projectile.Center.Y - hitbox.Height / 2f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "circle_05").Value;
            float alpha = MathHelper.Lerp(0.5f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            float scale = MathHelper.Lerp(0f, 6f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Red * alpha, Projectile.rotation, texture.Size() / 2f, scale, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.Additive);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "light_03").Value;
            scale = MathHelper.Lerp(0f, 2f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * alpha, Projectile.rotation, texture.Size() / 2f, scale + 0.5f, SpriteEffects.None, 1);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "twirl_01").Value;
            alpha = MathHelper.Lerp(1f, 0f, Utils.Clamp(AICounter, 0f, 80f) / 80f);
            scale = MathHelper.Lerp(0f, 0.5f, Utils.Clamp(AICounter, 0f, 60f) / 60f);

            float iterations = 5;
            for (int i = 0; i < iterations; i++)
            {
                Color color = Color.Lerp(Color.Red, Color.Orange, i / iterations);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, (Projectile.rotation * 2 + MathHelper.ToRadians(10 * i)) * -1, texture.Size() / 2f, scale, SpriteEffects.None, 1);

                scale = MathHelper.Lerp(0f, 1f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
                float rotationSpeed = MathHelper.Lerp(3, 2, Utils.Clamp(AICounter - 10, 0f, 60f) / 60f);

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, Projectile.rotation * rotationSpeed + MathHelper.ToRadians(25 * i) + MathHelper.ToRadians(240), texture.Size() / 2f, scale, SpriteEffects.FlipHorizontally, 1);

                scale = MathHelper.Lerp(0f, 1.50f, Utils.Clamp(AICounter, 0f, 60f) / 60f);
                rotationSpeed = MathHelper.Lerp(4, 2, Utils.Clamp(AICounter - 20, 0f, 60f) / 60f);

                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, color * alpha, (Projectile.rotation * rotationSpeed + MathHelper.ToRadians(5 * i) + MathHelper.ToRadians(240)) * -1, texture.Size() / 2f, scale, SpriteEffects.None, 1);
            }

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "star_05").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange * alpha, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 1);

            Main.spriteBatch.Reload(BlendState.AlphaBlend);

            return false;
        }
    }
}