using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Common.Primitives.Trails;
using OvermorrowMod.Core;
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
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 900;
            Projectile.alpha = 255;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(3, 6);
                Dust dust = Terraria.Dust.NewDustDirect(Projectile.Center, 2, 2, DustID.RainbowMk2, velocity.X, velocity.Y, 0, Main.DiscoColor, 1f);
                dust.noGravity = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_644").Value;
            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor * 0.5f, Projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.4f, 2f), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor * 0.5f, Projectile.rotation, drawOrigin, new Vector2(0.4f, 1f), SpriteEffects.None, 0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D SoulTexture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Extra_89").Value;
            Main.EntitySpriteDraw(SoulTexture, Projectile.Center - Main.screenPosition, null, Main.DiscoColor, Projectile.rotation + MathHelper.PiOver2, SoulTexture.Size() / 2, new Vector2(0.5f, 1), SpriteEffects.None, 0);

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Makes dust Projectiled on tile
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Pulse>(), Projectile.Center, Vector2.Zero, Main.DiscoColor, 0.5f, 0.25f);

            for (int i = 0; i < 15; i++)
            {
                Vector2 RandomVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(5, 15);
                Particle.CreateParticle(Particle.ParticleType<Spark>(), Projectile.Center, RandomVelocity, Main.DiscoColor, 1, 1f);
            }
        }
    }
}