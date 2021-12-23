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

    public class RotatingPrismaMeteor : ModProjectile, ITrailEntity
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
            DisplayName.SetDefault("Prism Burst");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 360;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            projectile.rotation += projectile.ai[0];

            projectile.velocity = projectile.velocity.RotatedBy(MathHelper.ToRadians(1f)) * projectile.ai[1];

            if (Main.rand.NextBool(8))
            {
                Particle.CreateParticle(Particle.ParticleType<Glow>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(3, 6), Main.DiscoColor, 1, 0.25f, 0, 1f);
            }
        }

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

    public class MeteoricBurst : ModProjectile, ITrailEntity
    {
        public Color TrailColor(float progress) => Main.DiscoColor;
        public float TrailSize(float progress) => 240;
        public Type TrailType()
        {
            return typeof(MeteorTrail);

        }

        public override string Texture => "Terraria/Item_" + ProjectileID.LostSoulFriendly;
        public Color projectileColor = Main.DiscoColor;
        public NPC ParentNPC;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteoric Burst");
        }

        public override void SetDefaults()
        {
            projectile.width = 260;
            projectile.height = 260;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            ParentNPC = Main.npc[(int)projectile.ai[0]];
            if (ParentNPC.active)
            {
                projectile.Center = ParentNPC.Center;
                projectile.timeLeft = 5;
            }

            projectile.rotation += 0.08f;

            // Tile collide literally won't work properly so that's why I added the Collision check
            if (projectile.ai[1]++ > 480 && !Collision.CanHit(projectile.Center, 260, 260, projectile.Center + Vector2.UnitY, 2, 2))
            {
                int MAX_PROJECTILES = 8;
                for (int i = 1; i <= MAX_PROJECTILES; i++)
                {
                    //int RADIUS = 480;
                    Vector2 ProjectileVelocity = Vector2.One.RotatedBy(MathHelper.ToRadians(360 / MAX_PROJECTILES) * i) * 6;
                    Projectile.NewProjectile(projectile.Center, ProjectileVelocity, ModContent.ProjectileType<RotatingPrismaMeteor>(), projectile.damage / 3, 5f, Main.myPlayer, Main.rand.NextFloat(0.04f, 0.085f), 1);
                    Projectile.NewProjectile(projectile.Center, ProjectileVelocity * 2, ModContent.ProjectileType<RotatingPrismaMeteor>(), projectile.damage / 3, 5f, Main.myPlayer, Main.rand.NextFloat(0.04f, 0.085f), 1);

                    Projectile.NewProjectile(projectile.Center + new Vector2(-4,  480).RotatedBy(MathHelper.ToRadians(360 / MAX_PROJECTILES) * i), Vector2.Zero, ModContent.ProjectileType<CurvedTelegraph>(), 0, 0f, Main.myPlayer, (360 / MAX_PROJECTILES) * i);
                    Projectile.NewProjectile(projectile.Center + new Vector2(-4, 976).RotatedBy(MathHelper.ToRadians(360 / MAX_PROJECTILES) * i), Vector2.Zero, ModContent.ProjectileType<CurvedTelegraph2>(), 0, 0f, Main.myPlayer, (360 / MAX_PROJECTILES) * i);

                }

                projectile.Kill();
            }

            if (Main.rand.NextBool(5))
            {
                Particle.CreateParticle(Particle.ParticleType<Glow>(), projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.Next(6, 12), Main.DiscoColor, 1, 2, 0, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/test");

            Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, MathHelper.Lerp(0, 4, Utils.Clamp(projectile.ai[1], 0, 60) / 60f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, MathHelper.Lerp(0, 4, Utils.Clamp(projectile.ai[1], 0, 60) / 60f), SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation + MathHelper.PiOver2, drawOrigin, new Vector2(MathHelper.Lerp(0, 1.2f, Utils.Clamp(projectile.ai[1], 0, 60) / 60f)/*0.3f*/, MathHelper.Lerp(0, 8, Utils.Clamp(projectile.ai[1], 0, 60) / 60f)/*2f*/), SpriteEffects.None, 0);


            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, MathHelper.Lerp(0, 4, Utils.Clamp(projectile.ai[1], 0, 60) / 60f), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation, drawOrigin, MathHelper.Lerp(0, 4, Utils.Clamp(projectile.ai[1], 0, 60) / 60f), SpriteEffects.None, 0);

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Rectangle?(rect), Main.DiscoColor, projectile.rotation + MathHelper.Pi, drawOrigin, new Vector2(MathHelper.Lerp(0, 1.2f, Utils.Clamp(projectile.ai[1], 0, 60) / 60f)/*0.3f*/, MathHelper.Lerp(0, 8, Utils.Clamp(projectile.ai[1], 0, 60) / 60f)/*2f*/), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void Kill(int timeLeft)
        {
            ParentNPC.Center = projectile.Center;
            ParentNPC.alpha = 0;
            ParentNPC.velocity = Vector2.Zero;
            ((TreeBossP2)ParentNPC.modNPC).MeteorLanded = true;

            Particle.CreateParticle(Particle.ParticleType<Shockwave2>(), projectile.Center, Vector2.Zero, Main.DiscoColor, 1, 8f, 0, 1f);

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                float distance = Vector2.Distance(projectile.Center, Main.player[i].Center);
                if (distance <= 2000)
                {
                    Main.player[i].GetModPlayer<OvermorrowModPlayer>().ScreenShake = 25;
                }
            }

            Vector2 origin = projectile.Center;
            float radius = 15;
            int numLocations = 6;

            for (int i = 0; i < 6; i++)
            {
                Vector2 position = origin + Vector2.UnitX.RotatedByRandom(MathHelper.ToRadians(360f / numLocations * i)) * radius;
                Vector2 dustvelocity = new Vector2(0f, 12f).RotatedBy(MathHelper.ToRadians(360f / numLocations * i)) * 8;

                Particle.CreateParticle(Particle.ParticleType<Glow>(), position, dustvelocity, Main.DiscoColor, 1, 3, MathHelper.ToRadians(360f / numLocations * i), 1f);
            }

            Main.PlaySound(SoundID.Item14);
        }
    }
}