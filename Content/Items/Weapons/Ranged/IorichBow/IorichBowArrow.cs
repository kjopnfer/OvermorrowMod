using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.Items.Weapons.Ranged.IorichBow
{
    public class IorichBowArrow : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Main.DiscoColor;
        public float TrailSize(float progress) => 10;
        //public float TrailSize(float progress) => 20;

        public Type TrailType() => typeof(TorchTrail);
   
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = 2;
            projectile.timeLeft = 900;
            projectile.alpha = 255;
            projectile.tileCollide = true;
            projectile.ranged = true;
            projectile.extraUpdates = 10;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(3, 6);
                Dust dust = Terraria.Dust.NewDustDirect(projectile.Center, 2, 2, DustID.RainbowMk2, velocity.X, velocity.Y, 0, Main.DiscoColor, 1f);
                dust.noGravity = true;
            }

            projectile.rotation = projectile.velocity.ToRotation();
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("Terraria/Projectile_644");
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor * 0.5f, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.4f, 2f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor * 0.5f, projectile.rotation, drawOrigin, new Vector2(0.4f, 1f), SpriteEffects.None, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D SoulTexture = ModContent.GetTexture("OvermorrowMod/Textures/Extra_89");
            Main.spriteBatch.Draw(SoulTexture, projectile.Center - Main.screenPosition, null, Main.DiscoColor, projectile.rotation + MathHelper.PiOver2, SoulTexture.Size() / 2, new Vector2(0.5f, 1), SpriteEffects.None, 0f);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust projectiled on tile
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Main.DiscoColor, 0.5f, 0.25f);

            for (int i = 0; i < 15; i++)
            {
                Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(5, 15);
                Particle.CreateParticle(Particle.ParticleType<Spark>(), projectile.Center, RandomVelocity, Main.DiscoColor, 1, 1f);
            }
        }
    }
}