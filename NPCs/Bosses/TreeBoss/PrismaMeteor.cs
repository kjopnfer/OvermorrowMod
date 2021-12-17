using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Effects.Prim;
using OvermorrowMod.Effects.Prim.Trails;
using OvermorrowMod.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OvermorrowMod.NPCs.Bosses.TreeBoss
{
    public class PrismaMeteor : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Main.DiscoColor;
        public float TrailSize(float progress) => 40;
        public Type TrailType()
        {
            return typeof(TorchTrail);

        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public Color projectileColor = Main.DiscoColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prisma Meteor");
        }

        public override void SetDefaults()
        {
            projectile.width = 65;
            projectile.height = 65;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 900;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += projectile.ai[0];

            if (projectile.ai[1]++ == 0)
            {
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TreeWarning>(), 0, 1f, Main.myPlayer, 0, 1);
            }

            projectile.tileCollide = projectile.ai[1] < 180 ? false : true;

            if (Main.rand.NextBool(5))
            {
                Particle.CreateParticle(Particle.ParticleType<Glow>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(3, 6), Main.DiscoColor, 1, 1, 0, 1f);
            }
        }

        public override bool ShouldUpdatePosition() => projectile.ai[1] > 60 ? true : false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/test");

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.3f, 2f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Main.DiscoColor, 1, 1, 0, 1f);


            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 1050 && projectile.ai[0] > -1)
                {
                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 15;
                }
            }

            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 6;

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 3f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * 2;

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Main.DiscoColor, 1, 1, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }

            Main.PlaySound(SoundID.Item14);
        }
    }

    public class LesserPrismaMeteor : ModProjectile, ITrailEntity
    {
        // TODO: The system for the Torch Trails has changed where the color and width can be changed within the interface implementation
        // Therefore, remake this projectile by returning Color types, referencing the previous GitHub commit for this for the colors
        public Color TrailColor(float progress) => Main.DiscoColor;
        public float TrailSize(float progress) => 40;
        public Type TrailType()
        {
            return typeof(TorchTrail);
        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public Color projectileColor = Main.DiscoColor;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lesser Prisma Meteor");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 900;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += projectile.ai[0];

            if (projectile.ai[1]++ == 0)
            {
                Projectile.NewProjectile(projectile.Center, projectile.velocity, ModContent.ProjectileType<TreeWarning>(), 0, 1f, Main.myPlayer, 0, 1);
            }

            projectile.tileCollide = projectile.ai[1] < 180 ? false : true;

            if (Main.rand.NextBool(8))
            {
                Particle.CreateParticle(Particle.ParticleType<Glow>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(3, 6), Main.DiscoColor, 1, 0.25f, 0, 1f);
            }
        }

        public override bool ShouldUpdatePosition() => projectile.ai[1] > 60 ? true : false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/test2");

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, projectile.scale * 0.8f, SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(0.3f, 2f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, new Vector2(0.3f, 2f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            Particle.CreateParticle(Particle.ParticleType<Shockwave>(), projectile.Center, Vector2.Zero, Main.DiscoColor, 1, 0.5f, 0, 1f);

            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 6;

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 3f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i));

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Main.DiscoColor, 1, 0.5f, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }

            Main.PlaySound(SoundID.Item14);
        }
    }
}