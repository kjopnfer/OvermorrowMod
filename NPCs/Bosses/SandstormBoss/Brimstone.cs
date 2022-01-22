using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using OvermorrowMod.Effects.Prim;

namespace OvermorrowMod.NPCs.Bosses.SandstormBoss
{
    public class Brimstone : ModProjectile
    {
        public override string Texture => "OvermorrowMod/Textures/Empty";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sussy Beam Test");
        }
        public override void SetDefaults()
        {
            projectile.width = 25;
            projectile.height = 5000;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            projectile.velocity = projectile.velocity.SafeNormalize(-Vector2.UnitY).RotatedBy(MathHelper.ToRadians(MathHelper.SmoothStep(4, 1, projectile.timeLeft / 120f)));
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
            //Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Circle");
            Texture2D texture = ModContent.GetTexture("OvermorrowMod/Textures/Sunlight");
            // make the beam slightly change scale with time
            float mult = (0.55f + (float)Math.Sin(Main.GlobalTime/* * 2*/) * 0.1f);
            // base scale for the flash so it actually connects with beam
            float scale = projectile.scale * 4 * mult;
            // draw flash
            //for (int i = 0; i < 25; i++)
            //    Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor * 0.04f * (25f - (float)i), 0, new Vector2(texture.Width, texture.Height) / 2, scale * ((float)i / 25f), SpriteEffects.None, 0f);
            
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, BeamColor, 0, new Vector2(texture.Width, texture.Height) / 2, scale * 0.25f, SpriteEffects.None, 0f);

            scale = projectile.scale * 2 * mult;
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

            BeamColor = new Color(255, 205, 3);
            PrimitivePacket packet2 = new PrimitivePacket();
            packet2.Pass = "Texture";
            PrimitivePacket.SetTexture(0,  ModContent.GetTexture("OvermorrowMod/Effects/TrailTextures/Trail7"));
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
            for (int i = 0; i < 5; i++)
                Main.spriteBatch.Draw(texture, end - Main.screenPosition, null, BeamColor, projectile.rotation, new Vector2(texture.Width, texture.Height) / 2, scale * 0.15f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}