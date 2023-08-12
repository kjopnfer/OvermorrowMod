using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OvermorrowMod.Common;
using OvermorrowMod.Common.Particles;
using OvermorrowMod.Common.Primitives;
using OvermorrowMod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace OvermorrowMod.Content.NPCs.Bosses.SandstormBoss
{
    public class ForbiddenBeam : ModProjectile
    {
        protected bool RunOnce = true;
        protected int RotationDirection = 1;
        public Player Target;

        protected const float MAX_TIME = 240;
        public override bool? CanDamage() => false;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 5000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        // Cross Product: Where a = line point 1; b = line point 2; c = point to check against.
        public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                Target = Main.player[(int)Projectile.ai[0]];
                RunOnce = false;
            }

            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000);
            RotationDirection = isLeft(Projectile.Center, end, Target.Center) ? 1 : -1;

            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(0.5f, 0, Projectile.timeLeft / MAX_TIME)) * RotationDirection);

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.height, Projectile.width, ref a);
        }
        public Color BeamColor = Color.Yellow;

        public Texture2D TrailTexture1 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0").Value;
        public Texture2D TrailTexture2 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value;
        public Texture2D TrailTexture3 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail1").Value;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = Projectile.scale * 4 * mult;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0);

            //float scale = projectile.scale * 2 * mult;
            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000);
            float width = Projectile.width * Projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            BeamPacket.SetTexture(0, TrailTexture1);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture2);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            BeamPacket packet3 = new BeamPacket();
            packet3.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture3);
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;
            for (int i = 0; i < 5; i++)
                Main.EntitySpriteDraw(texture, end - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (!(Projectile.ModProjectile is SandFall))
            {
                DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
                Terraria.Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000), Projectile.width * Projectile.scale, new Terraria.Utils.TileActionAttempt(DelegateMethods.CastLight));
            }
        }
    }

    public class ForbiddenBeamFriendly : ForbiddenBeam
    {
        public Vector2 CrossHairTarget;
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 5000;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                CrossHairTarget = new Vector2(Projectile.ai[0], Projectile.ai[1]);
                RunOnce = false;
            }

            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000);
            RotationDirection = isLeft(Projectile.Center, end, CrossHairTarget) ? 1 : -1;

            Projectile.velocity = Projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(0.5f, 0, Projectile.timeLeft / MAX_TIME)) * RotationDirection);

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }
    }

    public class ForbiddenBurst : ForbiddenBeam
    {
        public new Projectile Target;

        private new const float MAX_TIME = 20;

        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 5000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 1)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }

            if (RunOnce)
            {
                Target = Main.projectile[(int)Projectile.ai[0]];
                RunOnce = false;
            }

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);
            float scale = Projectile.scale * 4 * mult;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0);

            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            Vector2 end = Target.Center;

            float width = Projectile.width * Projectile.scale;
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0").Value);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            BeamPacket packet3 = new BeamPacket();
            packet3.Pass = "Texture";
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail1").Value);
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;
            for (int i = 0; i < 5; i++)
                Main.EntitySpriteDraw(texture, end - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class SandFall : ModProjectile
    {
        protected bool RunOnce = true;
        protected int RotationDirection = 1;
        public Player Target;

        protected const float MAX_TIME = 240;

        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SANDFALL2");
        }
        public override void SetDefaults()
        {
            Projectile.width = 25;
            Projectile.height = 5000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)MAX_TIME;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        // Cross Product: Where a = line point 1; b = line point 2; c = point to check against.
        public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                Target = Main.player[(int)Projectile.ai[0]];
                RunOnce = false;
            }

            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000);
            RotationDirection = isLeft(Projectile.Center, end, Target.Center) ? 1 : -1;

            //projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(0.5f, 0, projectile.timeLeft / MAX_TIME)) * RotationDirection);
            Projectile.velocity = Vector2.UnitY;

            float progress = Utils.GetLerpValue(0, MAX_TIME, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.height, Projectile.width, ref a);
        }
        public Color BeamColor = Color.Yellow;

        public Texture2D TrailTexture1 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0v2").Value;
        public Texture2D TrailTexture2 = ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value;
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // make the beam slightly change scale with time
            float mult = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly/* * 2*/) * 0.1f;

            //float scale = projectile.scale * 2 * mult;
            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, 5000);
            float width = Projectile.width * Projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            //BeamColor = Lighting.GetColor((int)projectile.Center.X, (int)projectile.Center.Y, new Color(95, 73, 50));
            BeamColor = new Color(95, 73, 50);
            BeamPacket.SetTexture(0, TrailTexture1);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            //BeamColor = Lighting.GetColor((int)projectile.Center.X, (int)projectile.Center.Y, new Color(180, 128, 70));
            BeamColor = new Color(180, 128, 70);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, TrailTexture2);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++)
                {
                    Vector2 RandomPosition = end + new Vector2(Main.rand.Next(-10, 10), 0);
                    Vector2 RandomVelocity = -Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.Next(1, 3);
                    Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.15f, 0.25f), Main.rand.NextFloat(0.6f, 1f), 0, 0, Main.rand.Next(30, 60));
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

    public class SandFall2 : ForbiddenBeam
    {
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SANDFALL");
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 250;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.hide = true; // Prevents projectile from being drawn normally. Use in conjunction with DrawBehind.
            Projectile.localNPCHitCooldown = 10;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }

        public override void AI()
        {
            //projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(4, 1, projectile.timeLeft / 120f)));
            float progress = Utils.GetLerpValue(0, 120, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.1f);

            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            //Vector2 end = projectile.Center + projectile.velocity * projectile.height;
            Vector2 end = Projectile.Center + Projectile.velocity * TRay.CastLength(Projectile.Center, Projectile.velocity, Projectile.height);

            float width = Projectile.width * Projectile.scale;
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(95, 73, 50);
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0v2").Value);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(180, 128, 70);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            for (int i = 0; i < Main.rand.Next(7, 10); i++)
            {
                Vector2 RandomPosition = end + new Vector2(Main.rand.Next(-10, 10), 5) - Main.screenPosition;
                Vector2 RandomVelocity = -Vector2.One.RotatedByRandom(MathHelper.Pi) * Main.rand.Next(1, 3);
                Particle.CreateParticle(Particle.ParticleType<Smoke2>(), RandomPosition, RandomVelocity, new Color(182, 128, 70), Main.rand.NextFloat(0.15f, 0.35f));
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class GiantBeam : ForbiddenBeam
    {
        private NPC ParentNPC;
        public override string Texture => AssetDirectory.Empty;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sussy Beam Test 2");
        }
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 2000;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 1)
            {
                Projectile.friendly = true;
                Projectile.hostile = false;
            }

            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)Projectile.ai[0]];
                RunOnce = false;
            }

            if (ParentNPC.active)
            {
                Projectile.Center = ParentNPC.Center + Vector2.UnitY * -750;
            }
            else
            {
                Projectile.Kill();
            }

            //projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(4, 1, projectile.timeLeft / 120f)));
            float progress = Utils.GetLerpValue(0, 120, Projectile.timeLeft);
            Projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            float mult = (0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f);
            float scale = Projectile.scale * 4 * mult;
            Texture2D texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "PulseCircle").Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0);

            BeamPacket packet = new BeamPacket();
            packet.Pass = "Texture";
            Vector2 start = Projectile.Center;
            Vector2 end = Projectile.Center + Projectile.velocity * Projectile.height;
            float width = Projectile.width * Projectile.scale;
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail0v2").Value);
            float off = -Main.GlobalTimeWrappedHourly % 1;
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            BeamPacket packet2 = new BeamPacket();
            packet2.Pass = "Texture";
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail7").Value);
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            BeamPacket packet3 = new BeamPacket();
            packet3.Pass = "Texture";
            BeamPacket.SetTexture(0, ModContent.Request<Texture2D>(AssetDirectory.FullTrail + "Trail1").Value);
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0);

            texture = ModContent.Request<Texture2D>(AssetDirectory.Textures + "Sunlight").Value;
            for (int i = 0; i < 5; i++)
                Main.EntitySpriteDraw(texture, end - Main.screenPosition, null, BeamColor, Projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}