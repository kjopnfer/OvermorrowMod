using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using OvermorrowMod.Effects.Prim;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class ForbiddenBeam : ModProjectile
    {
        protected bool RunOnce = true;
        protected int RotationDirection = 1;
        public Player Target;

        protected const float MAX_TIME = 240;

        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)MAX_TIME;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
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
                Target = Main.player[(int)projectile.ai[0]];
                RunOnce = false;
            }

            Vector2 end = projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000);
            RotationDirection = isLeft(projectile.Center, end, Target.Center) ? 1 : -1;

            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(0.5f, 0, projectile.timeLeft / MAX_TIME)) * RotationDirection);

            float progress = Utils.InverseLerp(0, MAX_TIME, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.height, projectile.width, ref a);
        }
        public Color BeamColor = Color.Yellow;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // texture used for flash
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 4 * mult;
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //float scale = projectile.scale * 2 * mult;
            PrimitivePacket packet = new PrimitivePacket();
            packet.Pass = "Texture";
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000);
            float width = projectile.width * projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail0"));
            float off = -Main.GlobalTime % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail7"));
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            PrimitivePacket packet3 = new PrimitivePacket();
            packet3.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail1"));
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, BeamColor, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000), projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
        }
    }

    public class ForbiddenBeamFriendly : ForbiddenBeam
    {
        public Vector2 CrossHairTarget;
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)MAX_TIME;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (RunOnce)
            {
                CrossHairTarget = new Vector2(projectile.ai[0], projectile.ai[1]);
                RunOnce = false;
            }

            Vector2 end = projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000);
            RotationDirection = isLeft(projectile.Center, end, CrossHairTarget) ? 1 : -1;

            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(0.5f, 0, projectile.timeLeft / MAX_TIME)) * RotationDirection);

            float progress = Utils.InverseLerp(0, MAX_TIME, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }
    }

    public class BeamBuildup : ModProjectile
    {
        private bool RunOnce = true;
        public Projectile Target;

        private const float MAX_TIME = 15;

        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)MAX_TIME;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
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
                Target = Main.projectile[(int)projectile.ai[0]];
                RunOnce = false;
            }

            float progress = Utils.InverseLerp(0, MAX_TIME, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.height, projectile.width, ref a);
        }

        public override void Kill(int timeLeft)
        {
            base.Kill(timeLeft);
        }

        public Color BeamColor = Color.Yellow;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // texture used for flash
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 4 * mult;
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //float scale = projectile.scale * 2 * mult;
            PrimitivePacket packet = new PrimitivePacket();
            packet.Pass = "Texture";
            Vector2 start = projectile.Center;
            Vector2 end = Target.Center;

            float width = projectile.width * projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail0"));
            float off = -Main.GlobalTime % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail7"));
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            PrimitivePacket packet3 = new PrimitivePacket();
            packet3.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail1"));
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, BeamColor, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000), projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
        }
    }

    public class ForbiddenBurst : ModProjectile
    {
        private bool RunOnce = true;
        public Projectile Target;

        private const float MAX_TIME = 20;

        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = (int)MAX_TIME;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }


        // Cross Product: Where a = line point 1; b = line point 2; c = point to check against.
        public bool isLeft(Vector2 a, Vector2 b, Vector2 c)
        {
            return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) > 0;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1)
            {
                projectile.friendly = true;
                projectile.hostile = false;
            }

            if (RunOnce)
            {
                Target = Main.projectile[(int)projectile.ai[0]];
                RunOnce = false;
            }

            float progress = Utils.InverseLerp(0, MAX_TIME, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.height, projectile.width, ref a);
        }
        public Color BeamColor = Color.Yellow;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // texture used for flash
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 4 * mult;
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //float scale = projectile.scale * 2 * mult;
            PrimitivePacket packet = new PrimitivePacket();
            packet.Pass = "Texture";
            Vector2 start = projectile.Center;
            Vector2 end = Target.Center;

            float width = projectile.width * projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail0"));
            float off = -Main.GlobalTime % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail7"));
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            PrimitivePacket packet3 = new PrimitivePacket();
            packet3.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail1"));
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, BeamColor, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000), projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
        }
    }

    public class GiantBeam : ModProjectile
    {
        private bool RunOnce = true;
        private NPC ParentNPC;
        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test 2");
        }
        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 2000;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (projectile.ai[1] == 1)
            {
                projectile.friendly = true;
                projectile.hostile = false;
            }

            if (RunOnce)
            {
                ParentNPC = Main.npc[(int)projectile.ai[0]];
                RunOnce = false;
            }

            if (ParentNPC.active)
            {
                projectile.Center = ParentNPC.Center + Vector2.UnitY * -750;
            }
            else
            {
                projectile.Kill();
            }

            //projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(4, 1, projectile.timeLeft / 120f)));
            float progress = Utils.InverseLerp(0, 120, projectile.timeLeft);
            projectile.scale = MathHelper.Clamp((float)Math.Sin(progress * Math.PI) * 2, 0, 1);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float a = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center, projectile.Center + projectile.velocity * projectile.height, projectile.width, ref a);
        }
        public Color BeamColor = Color.Yellow;
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            projectile.rotation += 0.3f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            // texture used for flash
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime * 2) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 4 * mult;
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/PulseCircle");
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Yellow * 0.5f, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.5f, SpriteEffects.None, 0f);

            //float scale = projectile.scale * 2 * mult;
            PrimitivePacket packet = new PrimitivePacket();
            packet.Pass = "Texture";
            Vector2 start = projectile.Center;
            Vector2 end = projectile.Center + projectile.velocity * projectile.height;
            float width = projectile.width * projectile.scale;
            // offset so i can make the triangles
            Vector2 offset = (start - end).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * width;

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail0v2"));
            float off = -Main.GlobalTime % 1;
            // draw the flame part of the beam
            packet.Add(start + offset * 3 * mult, BeamColor, new Vector2(0 + off, 0));
            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));

            packet.Add(start - offset * 3 * mult, BeamColor, new Vector2(0 + off, 1));
            packet.Add(end - offset * 3 * mult, BeamColor, new Vector2(1 + off, 1));
            packet.Add(end + offset * 3 * mult, BeamColor, new Vector2(1 + off, 0));
            packet.Send();

            BeamColor = new Color(240, 231, 113);
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail7"));
            packet2.Add(start + offset * 2 * mult, BeamColor, new Vector2(0 + off, 0));
            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));

            packet2.Add(start - offset * 2 * mult, BeamColor, new Vector2(0 + off, 1));
            packet2.Add(end - offset * 2 * mult, BeamColor, new Vector2(1 + off, 1));
            packet2.Add(end + offset * 2 * mult, BeamColor, new Vector2(1 + off, 0));
            packet2.Send();

            BeamColor = Color.White;
            PrimitivePacket packet3 = new PrimitivePacket();
            packet3.Pass = "Texture";
            PrimitivePacket.SetTexture(0, ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail1"));
            float alpha = 1f;
            packet3.Add(start + offset * mult, BeamColor * alpha, new Vector2(0 + -off, 0));
            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));

            packet3.Add(start - offset * mult, BeamColor * alpha, new Vector2(0 + -off, 1));
            packet3.Add(end - offset * mult, BeamColor * alpha, new Vector2(1 + -off, 1));
            packet3.Add(end + offset * mult, BeamColor * alpha, new Vector2(1 + -off, 0));
            packet3.Send();

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0f);

            texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, BeamColor, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);



            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DelegateMethods.v3_1 = new Color(240, 231, 113).ToVector3();
            Terraria.Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * TRay.CastLength(projectile.Center, projectile.velocity, 5000), projectile.width * projectile.scale, new Terraria.Utils.PerLinePoint(DelegateMethods.CastLight));
        }
    }

}